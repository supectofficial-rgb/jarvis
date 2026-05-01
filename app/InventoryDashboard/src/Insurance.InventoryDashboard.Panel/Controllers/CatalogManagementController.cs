using System.Globalization;
using System.Text;
using System.Text.Json;
using Insurance.InventoryDashboard.Panel.Models;
using Insurance.InventoryDashboard.Panel.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Insurance.InventoryDashboard.Panel.Controllers;

public abstract partial class CatalogManagementController : Controller
{
    private static readonly HashSet<string> AllowedAttributeDataTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        "Text", "Number", "Boolean", "Date", "Option", "String", "Enum", "Select"
    };

    private static readonly HashSet<string> OptionSupportedAttributeDataTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        "Option", "Enum", "Select"
    };

    private static readonly HashSet<string> AllowedAttributeScopes = new(StringComparer.OrdinalIgnoreCase)
    {
        "Both", "Product", "Variant"
    };

    private static readonly HashSet<string> AllowedTrackingPolicies = new(StringComparer.OrdinalIgnoreCase)
    {
        "None", "Batch", "Serial"
    };

    private static readonly int[] PageSizeOptions = [10, 25, 50];

    protected readonly ICatalogApiService _apiService;
    private readonly IDashboardConfigService _dashboardConfigService;

    protected CatalogManagementController(ICatalogApiService apiService, IDashboardConfigService dashboardConfigService)
    {
        _apiService = apiService;
        _dashboardConfigService = dashboardConfigService;
    }

    [HttpGet]
    protected async Task<IActionResult> Categories(
        string? categoryId,
        string? searchTerm,
        string? statusFilter,
        string? sort,
        bool createNew = false,
        int page = 1,
        int pageSize = 10,
        string activeItemId = "categories",
        CancellationToken cancellationToken = default)
    {
        if (!TryGetToken(out var token))
        {
            return RedirectToAction("Login", "Auth");
        }

        var roles = ResolveRolesFromSession(token);
        var permissions = ResolvePermissionsFromSession();
        if (!IsAuthorizedFor(token, "Catalog.Category.View", "Category.Read", "Category.Search"))
        {
            TempData["CatalogError"] = "شما دسترسی مشاهده مدیریت دسته‌بندی را ندارید.";
            return RedirectToAction("Index", "Dashboard");
        }
        var modules = await _dashboardConfigService.GetMenuByRolesAsync(roles, cancellationToken);
        var menu = ResolveMenu(modules, "category_management", activeItemId);

        var categoriesResult = await _apiService.GetCategoryTreeAsync(token);
        var categories = categoriesResult.Data ?? new List<CategoryNodeModel>();
        var flatCategories = FlattenCategories(categories).ToList();
        var filteredFlatCategories = ApplyCategoryFilters(flatCategories, searchTerm, statusFilter, sort);
        var pagedCategories = Paginate(filteredFlatCategories, page, pageSize, out var normalizedPage, out var normalizedPageSize, out var totalCount, out var totalPages);

        var selectedCategoryId = !string.IsNullOrWhiteSpace(categoryId) &&
                                 flatCategories.Any(x => string.Equals(x.Id, categoryId, StringComparison.OrdinalIgnoreCase))
            ? categoryId
            : filteredFlatCategories.FirstOrDefault()?.Id ?? flatCategories.FirstOrDefault()?.Id;
        var isCategoryCreateMode = createNew && string.Equals(activeItemId, "categories", StringComparison.OrdinalIgnoreCase);
        var selectedCategory = flatCategories.FirstOrDefault(c =>
            string.Equals(c.Id, selectedCategoryId, StringComparison.OrdinalIgnoreCase));
        var categoryFormSource = isCategoryCreateMode ? null : selectedCategory;

        var attributesResult = !string.IsNullOrWhiteSpace(selectedCategoryId)
            ? await _apiService.GetCategoryAttributesAsync(selectedCategoryId, token, includeInherited: true, includeInactive: true)
            : new ApiResponse<List<AttributeDefinitionModel>> { IsSuccess = true, Data = new List<AttributeDefinitionModel>() };

        var allAttributesResult = await _apiService.GetActiveAttributeDefinitionsAsync(token);

        var rulesResult = !string.IsNullOrWhiteSpace(selectedCategoryId)
            ? await _apiService.GetCategoryAttributeRulesAsync(selectedCategoryId, token, includeInherited: true, includeInactive: true)
            : new ApiResponse<List<CategoryAttributeRuleModel>> { IsSuccess = true, Data = new List<CategoryAttributeRuleModel>() };

        var selectedAttribute = (attributesResult.Data ?? new List<AttributeDefinitionModel>())
            .FirstOrDefault();
        var selectedRule = (rulesResult.Data ?? new List<CategoryAttributeRuleModel>())
            .FirstOrDefault(x => !x.IsInherited) ?? rulesResult.Data?.FirstOrDefault();

        var model = new CategoryManagementPageViewModel
        {
            UserName = HttpContext.Session.GetString("UserName") ?? "کاربر",
            Roles = roles,
            Permissions = permissions,
            Modules = modules,
            ActiveModule = menu.Module,
            ActiveItem = menu.Item,
            IsCategoryCreateMode = isCategoryCreateMode,
            SelectedCategoryId = selectedCategoryId,
            Categories = categories,
            FlatCategories = flatCategories,
            FilteredFlatCategories = pagedCategories,
            AllAttributes = allAttributesResult.Data ?? new List<AttributeDefinitionModel>(),
            CategoryAttributes = attributesResult.Data ?? new List<AttributeDefinitionModel>(),
            CategoryAttributeRules = rulesResult.Data ?? new List<CategoryAttributeRuleModel>(),
            CategorySearchTerm = searchTerm,
            CategoryStatusFilter = statusFilter,
            CategorySort = sort,
            CategoryPage = normalizedPage,
            CategoryPageSize = normalizedPageSize,
            CategoryTotalCount = totalCount,
            CategoryTotalPages = totalPages,
            CategoryPageSizeOptions = PageSizeOptions,
            ErrorMessage = JoinErrors(categoriesResult.ErrorMessage, attributesResult.ErrorMessage, allAttributesResult.ErrorMessage, rulesResult.ErrorMessage),
            CategoryForm = new CategoryUpsertForm
            {
                CategoryId = categoryFormSource?.Id,
                Code = categoryFormSource?.Code ?? string.Empty,
                Name = categoryFormSource?.Name ?? string.Empty,
                DisplayOrder = categoryFormSource?.DisplayOrder ?? 0,
                ParentCategoryId = categoryFormSource?.ParentCategoryId
            },
            MoveCategoryForm = new MoveCategoryForm
            {
                CategoryId = selectedCategoryId ?? string.Empty,
                ParentCategoryId = selectedCategory?.ParentCategoryId
            },
            AttributeForm = new AttributeDefinitionForm { CategoryId = selectedCategoryId ?? string.Empty },
            AttributeUpdateForm = new AttributeDefinitionUpdateForm
            {
                CategoryId = selectedCategoryId ?? string.Empty,
                AttributeId = selectedAttribute?.Id ?? string.Empty,
                Code = selectedAttribute?.Code ?? string.Empty,
                Name = selectedAttribute?.Name ?? string.Empty,
                DataType = selectedAttribute?.DataType ?? "Text",
                Scope = selectedAttribute?.Scope ?? "Both",
                DisplayOrder = selectedAttribute?.DisplayOrder ?? 0,
                IsRequired = selectedAttribute?.IsRequired ?? false,
                IsVariant = selectedAttribute?.IsVariant ?? false,
                IsActive = selectedAttribute?.IsActive ?? true
            },
            OptionForm = new AttributeOptionForm { CategoryId = selectedCategoryId ?? string.Empty },
            OptionUpdateForm = new AttributeOptionUpdateForm
            {
                CategoryId = selectedCategoryId ?? string.Empty,
                AttributeId = selectedAttribute?.Id ?? string.Empty
            },
            AssignForm = new AssignAttributeForm
            {
                CategoryId = selectedCategoryId ?? string.Empty,
                IsRequired = selectedRule?.RuleIsRequired ?? false,
                IsVariant = selectedRule?.RuleIsVariant ?? false,
                DisplayOrder = selectedRule?.RuleDisplayOrder ?? 0,
                IsOverridden = selectedRule?.RuleIsOverridden ?? false,
                IsActive = selectedRule?.RuleIsActive ?? true
            },
            RuleForm = new CategoryAttributeRuleForm
            {
                CategoryId = selectedCategoryId ?? string.Empty,
                AttributeId = selectedRule?.AttributeId ?? string.Empty,
                IsRequired = selectedRule?.RuleIsRequired ?? false,
                IsVariant = selectedRule?.RuleIsVariant ?? false,
                DisplayOrder = selectedRule?.RuleDisplayOrder ?? 0,
                IsOverridden = selectedRule?.RuleIsOverridden ?? false,
                IsActive = selectedRule?.RuleIsActive ?? true
            }
        };

        SetLayoutViewBag(model.Modules, model.ActiveModule?.ModuleId, model.ActiveItem?.ItemId, model.UserName);
        return View("~/Views/CatalogManagement/Categories.cshtml", model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    protected async Task<IActionResult> SaveCategory(CategoryUpsertForm form)
    {
        if (!TryGetToken(out var token))
        {
            return RedirectToAction("Login", "Auth");
        }

        var isCreateMode = string.IsNullOrWhiteSpace(form.CategoryId);
        var canSaveCategory = isCreateMode
            ? IsAuthorizedFor(token, "Inventory.Category.Create", "Catalog.Category.Create", "Category.Create")
            : IsAuthorizedFor(token, "Inventory.Category.Update", "Catalog.Category.Update", "Category.Update");

        if (!canSaveCategory)
        {
            TempData["CatalogError"] = isCreateMode
                ? "شما دسترسی ایجاد دسته‌بندی را ندارید."
                : "شما دسترسی ویرایش دسته‌بندی را ندارید.";
            return RedirectToAction(nameof(Categories), new { categoryId = form.CategoryId ?? form.ParentCategoryId });
        }

        if (!TryValidateModel(form))
        {
            TempData["CatalogError"] = ExtractModelError(ModelState);
            return RedirectToAction(nameof(Categories), new { categoryId = form.CategoryId ?? form.ParentCategoryId });
        }

        var categoriesResult = await _apiService.GetCategoryTreeAsync(token);
        var flatCategories = FlattenCategories(categoriesResult.Data ?? new List<CategoryNodeModel>()).ToList();

        if (!string.IsNullOrWhiteSpace(form.ParentCategoryId) &&
            !flatCategories.Any(c => string.Equals(c.Id, form.ParentCategoryId, StringComparison.OrdinalIgnoreCase)))
        {
            TempData["CatalogError"] = "دسته والد انتخاب‌شده معتبر نیست.";
            return RedirectToAction(nameof(Categories), new { categoryId = form.CategoryId });
        }

        if (!string.IsNullOrWhiteSpace(form.CategoryId))
        {
            if (string.Equals(form.CategoryId, form.ParentCategoryId, StringComparison.OrdinalIgnoreCase))
            {
                TempData["CatalogError"] = "دسته نمی‌تواند والد خودش باشد.";
                return RedirectToAction(nameof(Categories), new { categoryId = form.CategoryId });
            }

            if (CreatesCategoryCycle(flatCategories, form.CategoryId, form.ParentCategoryId))
            {
                TempData["CatalogError"] = "انتخاب والد باعث ایجاد چرخه در درخت دسته‌بندی می‌شود.";
                return RedirectToAction(nameof(Categories), new { categoryId = form.CategoryId });
            }
        }

        var request = new UpsertCategoryRequest
        {
            Id = form.CategoryId,
            Code = form.Code.Trim(),
            Name = form.Name.Trim(),
            DisplayOrder = form.DisplayOrder,
            ParentCategoryId = string.IsNullOrWhiteSpace(form.ParentCategoryId) ? null : form.ParentCategoryId.Trim()
        };

        ApiResponse<bool> result;
        if (string.IsNullOrWhiteSpace(form.CategoryId))
        {
            result = await _apiService.CreateCategoryAsync(request, token);
        }
        else
        {
            result = await _apiService.UpdateCategoryAsync(form.CategoryId, request, token);
        }

        if (!result.IsSuccess)
        {
            TempData["CatalogError"] = result.ErrorMessage ?? "عملیات با خطا مواجه شد.";
        }
        else
        {
            TempData["CatalogSuccess"] = string.IsNullOrWhiteSpace(form.CategoryId)
                ? "دسته‌بندی با موفقیت ایجاد شد."
                : "دسته‌بندی با موفقیت به‌روزرسانی شد.";
        }

        var redirectCategoryId = string.IsNullOrWhiteSpace(form.CategoryId)
            ? form.ParentCategoryId
            : form.CategoryId;

        return RedirectToAction(nameof(Categories), new { categoryId = redirectCategoryId, createNew = string.IsNullOrWhiteSpace(form.CategoryId) });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    protected async Task<IActionResult> CreateCategoryAttribute(AttributeDefinitionForm form)
    {
        if (!TryGetToken(out var token))
        {
            return RedirectToAction("Login", "Auth");
        }

        if (!IsAuthorizedFor(token, "Catalog.AttributeDefinition.Create", "AttributeDefinition.Create"))
        {
            TempData["CatalogError"] = "شما دسترسی ایجاد اتریبیوت را ندارید.";
            return RedirectToAction(nameof(Categories), new { categoryId = form.CategoryId });
        }

        if (!TryValidateModel(form))
        {
            TempData["CatalogError"] = ExtractModelError(ModelState);
            return RedirectToAction(nameof(Categories), new { categoryId = form.CategoryId });
        }

        if (!AllowedAttributeDataTypes.Contains(form.DataType))
        {
            TempData["CatalogError"] = "نوع داده اتریبیوت نامعتبر است.";
            return RedirectToAction(nameof(Categories), new { categoryId = form.CategoryId });
        }

        var request = new CreateAttributeDefinitionRequest
        {
            Code = form.Code.Trim(),
            Name = form.Name.Trim(),
            DataType = form.DataType.Trim(),
            Scope = form.Scope.Trim(),
            IsRequired = form.IsRequired,
            IsVariant = form.IsVariant,
            DisplayOrder = form.DisplayOrder
        };

        var result = await _apiService.CreateAttributeDefinitionAsync(form.CategoryId, request, token);
        if (!result.IsSuccess)
        {
            TempData["CatalogError"] = result.ErrorMessage ?? "عملیات با خطا مواجه شد.";
        }

        return RedirectToAction(nameof(Categories), new { categoryId = form.CategoryId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    protected async Task<IActionResult> AddAttributeOption(AttributeOptionForm form)
    {
        if (!TryGetToken(out var token))
        {
            return RedirectToAction("Login", "Auth");
        }

        if (!IsAuthorizedFor(token, "Catalog.AttributeOption.Create", "AttributeOption.Create"))
        {
            TempData["CatalogError"] = "شما دسترسی افزودن گزینه اتریبیوت را ندارید.";
            return RedirectToAction(nameof(Categories), new { categoryId = form.CategoryId });
        }

        if (!TryValidateModel(form))
        {
            TempData["CatalogError"] = ExtractModelError(ModelState);
            return RedirectToAction(nameof(Categories), new { categoryId = form.CategoryId });
        }

        var attributesResult = await _apiService.GetActiveAttributeDefinitionsAsync(token);
        var selectedAttribute = (attributesResult.Data ?? new List<AttributeDefinitionModel>())
            .FirstOrDefault(x => string.Equals(x.Id, form.AttributeId, StringComparison.OrdinalIgnoreCase));

        if (selectedAttribute is null)
        {
            TempData["CatalogError"] = "اتریبیوت انتخاب‌شده یافت نشد.";
            return RedirectToAction(nameof(Categories), new { categoryId = form.CategoryId });
        }

        if (!SupportsOptions(selectedAttribute.DataType))
        {
            TempData["CatalogError"] = "برای این نوع اتریبیوت امکان تعریف Option وجود ندارد.";
            return RedirectToAction(nameof(Categories), new { categoryId = form.CategoryId });
        }

        var normalizedOptionName = form.OptionName.Trim();
        var normalizedOptionValue = form.OptionValue.Trim();
        if (selectedAttribute.Options.Any(x =>
                string.Equals(x.OptionName, normalizedOptionName, StringComparison.OrdinalIgnoreCase)
                || string.Equals(x.OptionValue, normalizedOptionValue, StringComparison.OrdinalIgnoreCase)))
        {
            TempData["CatalogError"] = "Option با همین نام یا مقدار قبلا برای اتریبیوت انتخاب‌شده ثبت شده است.";
            return RedirectToAction(nameof(Categories), new { categoryId = form.CategoryId });
        }

        var result = await _apiService.AddAttributeOptionAsync(
            form.AttributeId,
            new AddAttributeOptionRequest
            {
                OptionName = normalizedOptionName,
                OptionValue = normalizedOptionValue,
                DisplayOrder = form.DisplayOrder
            },
            token);

        if (!result.IsSuccess)
        {
            TempData["CatalogError"] = result.ErrorMessage ?? "عملیات با خطا مواجه شد.";
        }

        return RedirectToAction(nameof(Categories), new { categoryId = form.CategoryId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    protected async Task<IActionResult> AssignAttributeToCategory(AssignAttributeForm form)
    {
        if (!TryGetToken(out var token))
        {
            return RedirectToAction("Login", "Auth");
        }

        if (!IsAuthorizedFor(token, "Catalog.Category.Rule.Assign", "CategoryAttributeRule.Assign", "CategoryAttributeRule.Create"))
        {
            TempData["CatalogError"] = "شما دسترسی انتساب اتریبیوت به دسته‌بندی را ندارید.";
            return RedirectToAction(nameof(Categories), new { categoryId = form.CategoryId });
        }

        if (!TryValidateModel(form))
        {
            TempData["CatalogError"] = ExtractModelError(ModelState);
            return RedirectToAction(nameof(Categories), new { categoryId = form.CategoryId });
        }

        var existingRulesResult = await _apiService.GetCategoryAttributeRulesAsync(
            form.CategoryId,
            token,
            includeInherited: false,
            includeInactive: true);

        var alreadyAssigned = (existingRulesResult.Data ?? new List<CategoryAttributeRuleModel>())
            .Any(x => string.Equals(x.AttributeId, form.AttributeId, StringComparison.OrdinalIgnoreCase));

        if (alreadyAssigned)
        {
            TempData["CatalogError"] = "این اتریبیوت قبلا برای دسته‌بندی انتخاب‌شده rule دارد.";
            return RedirectToAction(nameof(Categories), new { categoryId = form.CategoryId });
        }

        var result = await _apiService.AddCategoryAttributeRuleAsync(
            form.CategoryId,
            new AddCategoryAttributeRuleRequest
            {
                AttributeId = form.AttributeId,
                IsRequired = form.IsRequired,
                IsVariant = form.IsVariant,
                DisplayOrder = form.DisplayOrder,
                IsOverridden = form.IsOverridden,
                IsActive = form.IsActive
            },
            token);

        if (!result.IsSuccess)
        {
            TempData["CatalogError"] = result.ErrorMessage ?? "عملیات با خطا مواجه شد.";
        }
        else
        {
            TempData["CatalogSuccess"] = "Rule اتریبیوت با موفقیت ثبت شد.";
        }

        return RedirectToAction(nameof(Categories), new { categoryId = form.CategoryId });
    }

    [HttpGet]
    protected async Task<IActionResult> Products(
        string? categoryId,
        string? productId,
        string? searchTerm,
        string? categoryFilterId,
        string? statusFilter,
        string? sort,
        bool createNew = false,
        int page = 1,
        int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        if (!TryGetToken(out var token))
        {
            return RedirectToAction("Login", "Auth");
        }

        var roles = ResolveRolesFromSession(token);
        if (!IsAuthorizedFor(token, "Catalog.Product.View", "Product.Read", "Product.Search"))
        {
            TempData["CatalogError"] = "شما دسترسی مشاهده مدیریت محصولات را ندارید.";
            return RedirectToAction("Index", "Dashboard");
        }
        var modules = await _dashboardConfigService.GetMenuByRolesAsync(roles, cancellationToken);
        var menu = ResolveMenu(modules, "product_management", "products");

        var categoriesResult = await _apiService.GetCategoryTreeAsync(token);
        var categories = categoriesResult.Data ?? new List<CategoryNodeModel>();
        var flatCategories = FlattenCategories(categories).ToList();

        var productsResult = await _apiService.SearchProductsAsync(token, searchTerm, categoryFilterId);
        var allProducts = productsResult.Data ?? new List<ProductSummaryModel>();
        var uomLookupResult = await _apiService.GetUnitOfMeasureLookupAsync(token);
        var unitOfMeasures = uomLookupResult.Data ?? new List<UnitOfMeasureLookupModel>();
        var uomById = unitOfMeasures.ToDictionary(x => x.Id, x => x, StringComparer.OrdinalIgnoreCase);

        foreach (var product in allProducts)
        {
            if (string.IsNullOrWhiteSpace(product.CategoryName))
            {
                product.CategoryName = flatCategories
                    .FirstOrDefault(x => string.Equals(x.Id, product.CategoryId, StringComparison.OrdinalIgnoreCase))
                    ?.Name;
            }
        }

        var filteredProducts = ApplyProductFilters(allProducts, searchTerm, categoryFilterId, statusFilter, sort);
        var pagedProducts = Paginate(filteredProducts, page, pageSize, out var normalizedPage, out var normalizedPageSize, out var totalCount, out var totalPages);

        var isProductCreateMode = createNew;
        var selectedProductId = isProductCreateMode
            ? null
            : ResolveSelectedProductId(productId, filteredProducts, pagedProducts);
        var productDetailsResult = !string.IsNullOrWhiteSpace(selectedProductId)
            ? await _apiService.GetProductDetailsWithAttributesAsync(selectedProductId, token)
            : new ApiResponse<ProductDetailsModel> { IsSuccess = true };

        var productVariantsResult = !string.IsNullOrWhiteSpace(selectedProductId)
            ? await _apiService.GetProductVariantsByProductIdAsync(selectedProductId, token, includeInactive: true)
            : new ApiResponse<List<ProductVariantSummaryModel>> { IsSuccess = true, Data = new List<ProductVariantSummaryModel>() };
        var productVariants = productVariantsResult.Data ?? new List<ProductVariantSummaryModel>();
        foreach (var variant in productVariants)
        {
            if (uomById.TryGetValue(variant.BaseUomRef, out var uom))
            {
                variant.BaseUom = $"{uom.Code} - {uom.Name}";
            }
        }

        var selectedCategoryId = categoryId
            ?? productDetailsResult.Data?.CategoryId
            ?? allProducts.FirstOrDefault(p => string.Equals(p.Id, selectedProductId, StringComparison.OrdinalIgnoreCase))?.CategoryId
            ?? flatCategories.FirstOrDefault()?.Id;

        var (attributeGroups, effectiveAttributes, attributesError) =
            await LoadEffectiveCategoryAttributesAsync(selectedCategoryId, flatCategories, token);
        var effectiveProductAttributes = FilterEffectiveAttributesForProduct(effectiveAttributes);

        var missingRequired = !string.IsNullOrWhiteSpace(selectedProductId)
            ? FindMissingRequiredAttributes(
                effectiveProductAttributes,
                productDetailsResult.Data?.Attributes?.ToDictionary(
                    x => x.AttributeId,
                    x => x.Value,
                    StringComparer.OrdinalIgnoreCase)
                ?? new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase))
            : Array.Empty<EffectiveAttributeViewModel>();

        var permissions = ResolvePermissionsFromSession();

        var model = new ProductManagementPageViewModel
        {
            UserName = HttpContext.Session.GetString("UserName") ?? "کاربر",
            Roles = roles,
            Permissions = permissions,
            Modules = modules,
            ActiveModule = menu.Module,
            ActiveItem = menu.Item,

            IsProductCreateMode = isProductCreateMode,
            SelectedCategoryId = selectedCategoryId,
            SelectedProductId = selectedProductId,
            Categories = categories,
            FlatCategories = flatCategories,

            Products = pagedProducts,
            SelectedProductDetails = productDetailsResult.Data,
            ProductVariants = productVariants,
            ProductVariantTotalCount = productVariants.Count,
            UnitOfMeasures = unitOfMeasures,

            CategoryAttributeGroups = attributeGroups,
            EffectiveCategoryAttributes = effectiveProductAttributes,
            MissingRequiredProductAttributes = missingRequired,

            ProductSearchTerm = searchTerm,
            ProductCategoryFilterId = categoryFilterId,
            ProductStatusFilter = statusFilter,
            ProductSort = sort,
            ProductPage = normalizedPage,
            ProductPageSize = normalizedPageSize,
            ProductTotalCount = totalCount,
            ProductTotalPages = totalPages,
            ProductPageSizeOptions = PageSizeOptions,

            ErrorMessage = JoinErrors(
                categoriesResult.ErrorMessage,
                productsResult.ErrorMessage,
                uomLookupResult.ErrorMessage,
                productDetailsResult.ErrorMessage,
                productVariantsResult.ErrorMessage,
                attributesError),

            ProductForm = new ProductUpsertForm
            {
                ProductId = productDetailsResult.Data?.Id,
                Name = productDetailsResult.Data?.Name ?? string.Empty,
                BaseSku = productDetailsResult.Data?.BaseSku ?? string.Empty,
                CategoryId = selectedCategoryId ?? string.Empty,
                DefaultUomRef = productDetailsResult.Data?.DefaultUomRef ?? string.Empty,
                TaxCategoryRef = productDetailsResult.Data?.TaxCategoryRef,
                IsActive = productDetailsResult.Data?.IsActive ?? true,
                Description = productDetailsResult.Data?.Description
            },
            ProductAttributeForm = new ProductAttributeValueForm
            {
                ProductId = selectedProductId ?? string.Empty,
                CategoryId = selectedCategoryId ?? string.Empty
            },
            ProductCategoryChangeForm = new ProductCategoryChangeForm
            {
                ProductId = selectedProductId ?? string.Empty,
                CurrentCategoryId = selectedCategoryId ?? string.Empty,
                NewCategoryId = selectedCategoryId ?? string.Empty
            }
        };

        SetLayoutViewBag(model.Modules, model.ActiveModule?.ModuleId, model.ActiveItem?.ItemId, model.UserName);
        return View("~/Views/CatalogManagement/Products.cshtml", model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    protected async Task<IActionResult> SaveProduct(ProductUpsertForm form)
    {
        if (!TryGetToken(out var token))
        {
            return RedirectToAction("Login", "Auth");
        }

        if (!IsAuthorizedFor(token, "Catalog.Product.Create", "Catalog.Product.Update", "Product.Create", "Product.Update"))
        {
            TempData["CatalogError"] = "شما دسترسی ذخیره محصول را ندارید.";
            return RedirectToAction(nameof(Products), new { categoryId = form.CategoryId, productId = form.ProductId });
        }

        var categoriesResult = await _apiService.GetCategoryTreeAsync(token);
        var flatCategories = FlattenCategories(categoriesResult.Data ?? new List<CategoryNodeModel>()).ToList();
        var selectedCategory = flatCategories.FirstOrDefault(c => string.Equals(c.Id, form.CategoryId, StringComparison.OrdinalIgnoreCase));
        if (selectedCategory is null)
        {
            TempData["CatalogError"] = "دسته‌بندی انتخاب‌شده معتبر نیست.";
            return RedirectToAction(nameof(Products), new { categoryId = form.CategoryId, productId = form.ProductId });
        }

        if (string.IsNullOrWhiteSpace(form.ProductId))
        {
            var categoryProductsResult = await _apiService.SearchProductsAsync(token, categoryId: form.CategoryId);
            form.BaseSku = GenerateProductNumber(selectedCategory, categoryProductsResult.Data ?? new List<ProductSummaryModel>());
        }

        if (!TryValidateModel(form))
        {
            TempData["CatalogError"] = ExtractModelError(ModelState);
            return RedirectToAction(nameof(Products), new { categoryId = form.CategoryId, productId = form.ProductId });
        }

        if (!string.IsNullOrWhiteSpace(form.ProductId) && string.IsNullOrWhiteSpace(form.BaseSku))
        {
            TempData["CatalogError"] = "شماره محصول برای ویرایش معتبر نیست. صفحه را بازخوانی و دوباره تلاش کنید.";
            return RedirectToAction(nameof(Products), new { categoryId = form.CategoryId, productId = form.ProductId });
        }

        var uomLookupResult = await _apiService.GetUnitOfMeasureLookupAsync(token);
        var validUom = (uomLookupResult.Data ?? new List<UnitOfMeasureLookupModel>())
            .Any(x => string.Equals(x.Id, form.DefaultUomRef, StringComparison.OrdinalIgnoreCase));
        if (!validUom)
        {
            TempData["CatalogError"] = "واحد پایه انتخاب‌شده معتبر نیست.";
            return RedirectToAction(nameof(Products), new { categoryId = form.CategoryId, productId = form.ProductId });
        }

        var (_, effectiveAttributes, attributesError) =
            await LoadEffectiveCategoryAttributesAsync(form.CategoryId, flatCategories, token);
        var effectiveProductAttributes = FilterEffectiveAttributesForProduct(effectiveAttributes)
            .OrderBy(x => x.DisplayOrder)
            .ThenBy(x => x.Name)
            .ToList();
        var effectiveVariantAttributes = FilterEffectiveAttributesForVariant(effectiveAttributes)
            .Where(x => x.IsVariantLevel)
            .OrderBy(x => x.DisplayOrder)
            .ThenBy(x => x.Name)
            .ToList();

        if (!string.IsNullOrWhiteSpace(attributesError))
        {
            TempData["CatalogError"] = attributesError;
            return RedirectToAction(nameof(Products), new { categoryId = form.CategoryId, productId = form.ProductId });
        }

        var productCreateAttributes = new List<CatalogAttributeValueInputModel>();
        var generatedVariantPlans = new List<GeneratedVariantPlan>();

        if (string.IsNullOrWhiteSpace(form.ProductId))
        {
            if (!TryParseProductAutoVariantPayload(form.AutoVariantPayload, out var autoPayload))
            {
                TempData["CatalogError"] = "ساختار اطلاعات ویژگی‌های ارسالی معتبر نیست. صفحه را بازخوانی و دوباره تلاش کنید.";
                return RedirectToAction(nameof(Products), new { categoryId = form.CategoryId });
            }

            var productPayloadError = BuildProductCreateAttributeInputs(autoPayload, effectiveProductAttributes, productCreateAttributes);
            if (!string.IsNullOrWhiteSpace(productPayloadError))
            {
                TempData["CatalogError"] = productPayloadError;
                return RedirectToAction(nameof(Products), new { categoryId = form.CategoryId });
            }

            var variantDimensionError = BuildVariantGenerationDimensions(autoPayload, effectiveVariantAttributes, out var variantDimensions);
            if (!string.IsNullOrWhiteSpace(variantDimensionError))
            {
                TempData["CatalogError"] = variantDimensionError;
                return RedirectToAction(nameof(Products), new { categoryId = form.CategoryId });
            }

            var missingRequiredProduct = effectiveProductAttributes
                .Where(x => x.IsRequired)
                .Where(x => productCreateAttributes.All(v => !string.Equals(v.AttributeId, x.AttributeId, StringComparison.OrdinalIgnoreCase)))
                .ToList();
            if (missingRequiredProduct.Count > 0)
            {
                TempData["CatalogError"] = "ویژگی‌های الزامی محصول کامل نشده‌اند: " +
                    string.Join("، ", missingRequiredProduct.Select(x => x.Name));
                return RedirectToAction(nameof(Products), new { categoryId = form.CategoryId });
            }

            var missingRequiredVariant = effectiveVariantAttributes
                .Where(x => x.IsRequired)
                .Where(x => variantDimensions.All(d => !string.Equals(d.AttributeId, x.AttributeId, StringComparison.OrdinalIgnoreCase)))
                .ToList();
            if (missingRequiredVariant.Count > 0)
            {
                TempData["CatalogError"] = "برای ویژگی‌های واریانت‌ساز الزامی باید حداقل یک گزینه انتخاب شود: " +
                    string.Join("، ", missingRequiredVariant.Select(x => x.Name));
                return RedirectToAction(nameof(Products), new { categoryId = form.CategoryId });
            }

            generatedVariantPlans = BuildGeneratedVariantPlans(
                form.Name.Trim(),
                form.BaseSku.Trim(),
                variantDimensions);

            if (generatedVariantPlans.Count == 0)
            {
                generatedVariantPlans.Add(new GeneratedVariantPlan
                {
                    GeneratedName = form.Name.Trim(),
                    GeneratedSku = NormalizeSkuSegment(form.BaseSku.Trim()),
                    AttributeValues = new List<CatalogAttributeValueInputModel>()
                });
            }
        }

        if (!string.IsNullOrWhiteSpace(form.ProductId))
        {
            var detailsResult = await _apiService.GetProductDetailsWithAttributesAsync(form.ProductId, token);
            var existingValues = detailsResult.Data?.Attributes?.ToDictionary(
                x => x.AttributeId,
                x => x.Value,
                StringComparer.OrdinalIgnoreCase)
                ?? new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase);

            var missingRequired = FindMissingRequiredAttributes(effectiveProductAttributes, existingValues);
            if (missingRequired.Count > 0)
            {
                TempData["CatalogError"] = "ابتدا مقدار Attributeهای الزامی را کامل کنید: " +
                    string.Join("، ", missingRequired.Select(x => x.Name));
                return RedirectToAction(nameof(Products), new { categoryId = form.CategoryId, productId = form.ProductId });
            }
        }
        var request = new UpsertProductRequest
        {
            Id = form.ProductId,
            Name = form.Name.Trim(),
            CategoryId = form.CategoryId.Trim(),
            BaseSku = form.BaseSku.Trim(),
            DefaultUomRef = form.DefaultUomRef,
            TaxCategoryRef = string.IsNullOrWhiteSpace(form.TaxCategoryRef) ? null : form.TaxCategoryRef.Trim(),
            IsActive = form.IsActive,
            Description = string.IsNullOrWhiteSpace(form.Description) ? null : form.Description.Trim(),
            AttributeValues = productCreateAttributes
        };

        ApiResponse<bool> result;
        string? redirectProductId = form.ProductId;
        if (string.IsNullOrWhiteSpace(form.ProductId))
        {
            var createProductResult = await _apiService.CreateProductWithResultAsync(request, token);
            result = new ApiResponse<bool>
            {
                IsSuccess = createProductResult.IsSuccess,
                Data = createProductResult.IsSuccess,
                ErrorMessage = createProductResult.ErrorMessage
            };

            if (createProductResult.IsSuccess)
            {
                redirectProductId = createProductResult.Data?.ProductId;
            }

            if (result.IsSuccess && !string.IsNullOrWhiteSpace(redirectProductId) && generatedVariantPlans.Count > 0)
            {
                var createdCount = 0;
                var variantErrors = new List<string>();
                foreach (var plan in generatedVariantPlans)
                {
                    var variantCreateRequest = new UpsertVariantRequest
                    {
                        ProductId = redirectProductId,
                        Sku = plan.GeneratedSku,
                        Barcode = null,
                        BaseUomRef = form.DefaultUomRef,
                        TrackingPolicy = "None",
                        IsActive = true,
                        AttributeValues = plan.AttributeValues
                    };

                    var variantCreateResult = await _apiService.CreateProductVariantAsync(redirectProductId, variantCreateRequest, token);
                    if (variantCreateResult.IsSuccess)
                    {
                        createdCount++;
                        continue;
                    }

                    variantErrors.Add($"{plan.GeneratedName} ({plan.GeneratedSku}): {variantCreateResult.ErrorMessage ?? "خطای نامشخص"}");
                }

                if (createdCount > 0)
                {
                    TempData["CatalogSuccess"] = $"محصول با موفقیت ایجاد شد و {createdCount} واریانت به‌صورت خودکار تولید شد.";
                }
                else
                {
                    TempData["CatalogSuccess"] = "محصول با موفقیت ایجاد شد.";
                }

                if (variantErrors.Count > 0)
                {
                    TempData["CatalogError"] = "برخی واریانت‌ها ایجاد نشدند: " + string.Join(" | ", variantErrors.Take(3));
                }
            }
            else if (result.IsSuccess)
            {
                TempData["CatalogSuccess"] = "محصول با موفقیت ایجاد شد.";
            }
        }
        else
        {
            result = await _apiService.UpdateProductAsync(form.ProductId, request, token);
        }

        if (!result.IsSuccess)
        {
            TempData["CatalogError"] = result.ErrorMessage ?? "عملیات با خطا مواجه شد.";
        }

        return RedirectToAction(nameof(Products), new { categoryId = form.CategoryId, productId = redirectProductId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    protected async Task<IActionResult> SetProductAttributeValue(ProductAttributeValueForm form)
    {
        if (!TryGetToken(out var token))
        {
            return RedirectToAction("Login", "Auth");
        }

        if (!IsAuthorizedFor(token, "Catalog.ProductAttributeValue.Set", "ProductAttributeValue.Set"))
        {
            TempData["CatalogError"] = "شما دسترسی ثبت مقدار اتریبیوت محصول را ندارید.";
            return RedirectToAction(nameof(Products), new { categoryId = form.CategoryId, productId = form.ProductId });
        }

        if (!TryValidateModel(form))
        {
            TempData["CatalogError"] = ExtractModelError(ModelState);
            return RedirectToAction(nameof(Products), new { categoryId = form.CategoryId, productId = form.ProductId });
        }

        var categoriesResult = await _apiService.GetCategoryTreeAsync(token);
        var flatCategories = FlattenCategories(categoriesResult.Data ?? new List<CategoryNodeModel>()).ToList();
        var (_, effectiveAttributes, attributesError) =
            await LoadEffectiveCategoryAttributesAsync(form.CategoryId, flatCategories, token);
        var effectiveProductAttributes = FilterEffectiveAttributesForProduct(effectiveAttributes);

        if (!string.IsNullOrWhiteSpace(attributesError))
        {
            TempData["CatalogError"] = attributesError;
            return RedirectToAction(nameof(Products), new { categoryId = form.CategoryId, productId = form.ProductId });
        }

        var selectedAttribute = effectiveProductAttributes
            .FirstOrDefault(x => string.Equals(x.AttributeId, form.AttributeId, StringComparison.OrdinalIgnoreCase));

        if (selectedAttribute is null)
        {
            TempData["CatalogError"] = "اتریبیوت انتخاب‌شده معتبر نیست.";
            return RedirectToAction(nameof(Products), new { categoryId = form.CategoryId, productId = form.ProductId });
        }

        if (!TryValidateAttributeValue(selectedAttribute, form.Value, out var errorMessage))
        {
            TempData["CatalogError"] = errorMessage;
            return RedirectToAction(nameof(Products), new { categoryId = form.CategoryId, productId = form.ProductId });
        }

        var normalizedIncomingValue = form.Value?.Trim() ?? string.Empty;
        var selectedOption = selectedAttribute.Options.FirstOrDefault(x =>
            string.Equals(x.OptionValue, normalizedIncomingValue, StringComparison.OrdinalIgnoreCase)
            || string.Equals(x.OptionName, normalizedIncomingValue, StringComparison.OrdinalIgnoreCase));
        var selectedOptionId = selectedOption?.Id;
        var normalizedOptionValue = selectedOption?.OptionValue ?? normalizedIncomingValue;

        var productAttributeValuesResult = await _apiService.GetProductAttributeValuesByProductIdAsync(form.ProductId, token);
        var existingValue = (productAttributeValuesResult.Data ?? new List<ProductAttributeValueModel>())
            .FirstOrDefault(x => string.Equals(x.AttributeId, form.AttributeId, StringComparison.OrdinalIgnoreCase));
        if (!string.IsNullOrWhiteSpace(selectedOptionId))
        {
            if (string.Equals(existingValue?.OptionId, selectedOptionId, StringComparison.OrdinalIgnoreCase))
            {
                TempData["CatalogNotice"] = "مقدار اتریبیوت تغییری نکرده است.";
                return RedirectToAction(nameof(Products), new { categoryId = form.CategoryId, productId = form.ProductId });
            }
        }
        else
        {
            var normalizedExistingValue = existingValue?.OptionValue?.Trim();
            if (string.IsNullOrWhiteSpace(normalizedExistingValue))
            {
                normalizedExistingValue = existingValue?.Value?.Trim();
            }

            if (!string.IsNullOrWhiteSpace(normalizedExistingValue) &&
                string.Equals(normalizedExistingValue, normalizedIncomingValue, StringComparison.OrdinalIgnoreCase))
            {
                TempData["CatalogNotice"] = "مقدار اتریبیوت تغییری نکرده است.";
                return RedirectToAction(nameof(Products), new { categoryId = form.CategoryId, productId = form.ProductId });
            }
        }

        var result = await _apiService.SetProductAttributeValueAsync(
            form.ProductId,
            new SetProductAttributeValueRequest
            {
                AttributeId = form.AttributeId,
                Value = normalizedOptionValue,
                OptionId = selectedOptionId
            },
            token);

        if (!result.IsSuccess)
        {
            TempData["CatalogError"] = result.ErrorMessage ?? "عملیات با خطا مواجه شد.";
        }

        return RedirectToAction(nameof(Products), new { categoryId = form.CategoryId, productId = form.ProductId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    protected async Task<IActionResult> RemoveProductAttributeValue(string productId, string categoryId, string attributeId)
    {
        if (!TryGetToken(out var token))
        {
            return RedirectToAction("Login", "Auth");
        }

        if (!IsAuthorizedFor(token, "Catalog.ProductAttributeValue.Remove", "ProductAttributeValue.Remove"))
        {
            TempData["CatalogError"] = "شما دسترسی حذف مقدار اتریبیوت محصول را ندارید.";
            return RedirectToAction(nameof(Products), new { categoryId, productId });
        }

        if (string.IsNullOrWhiteSpace(productId) || string.IsNullOrWhiteSpace(attributeId))
        {
            TempData["CatalogError"] = "شناسه محصول یا اتریبیوت معتبر نیست.";
            return RedirectToAction(nameof(Products), new { categoryId, productId });
        }

        var result = await _apiService.RemoveProductAttributeValueAsync(productId, attributeId, token);
        if (!result.IsSuccess)
        {
            TempData["CatalogError"] = result.ErrorMessage ?? "حذف مقدار اتریبیوت محصول انجام نشد.";
        }

        return RedirectToAction(nameof(Products), new { categoryId, productId });
    }

    [HttpGet]
    protected async Task<IActionResult> Variants(
        string? productId,
        string? variantId,
        string? categoryId,
        string? searchTerm,
        string? attributeOptionIds,
        string? trackingPolicy,
        string? statusFilter,
        string? attributeTypeFilter,
        string? sort,
        bool createNew = false,
        int page = 1,
        int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        if (!TryGetToken(out var token))
        {
            return RedirectToAction("Login", "Auth");
        }

        var roles = ResolveRolesFromSession(token);
        if (!IsAuthorizedFor(token, "Catalog.Variant.View", "ProductVariant.Read", "ProductVariant.Search"))
        {
            TempData["CatalogError"] = "شما دسترسی مشاهده مدیریت واریانت‌ها را ندارید.";
            return RedirectToAction("Index", "Dashboard");
        }
        var modules = await _dashboardConfigService.GetMenuByRolesAsync(roles, cancellationToken);
        var menu = ResolveMenu(modules, "product_management", "product_variants");

        var categoriesResult = await _apiService.GetCategoryTreeAsync(token);
        var categories = categoriesResult.Data ?? new List<CategoryNodeModel>();
        var flatCategories = FlattenCategories(categories).ToList();
        var productsResult = await _apiService.SearchProductsAsync(token);
        var products = productsResult.Data ?? new List<ProductSummaryModel>();
        var uomLookupResult = await _apiService.GetUnitOfMeasureLookupAsync(token);
        var unitOfMeasures = uomLookupResult.Data ?? new List<UnitOfMeasureLookupModel>();
        var uomById = unitOfMeasures.ToDictionary(x => x.Id, x => x, StringComparer.OrdinalIgnoreCase);

        var selectedCategoryId = categoryId;
        var selectedProductId = productId;
        bool? isActiveFilter = TryParseStatusFilter(statusFilter, out var parsedActive) ? parsedActive : null;

        var variantsSearchResult = await _apiService.SearchProductVariantsAsync(
            token,
            searchTerm,
            selectedProductId,
            selectedCategoryId,
            attributeOptionIds,
            isActiveFilter,
            page,
            pageSize);

        var variants = variantsSearchResult.Data?.Items ?? new List<ProductVariantSummaryModel>();
        foreach (var variant in variants)
        {
            if (uomById.TryGetValue(variant.BaseUomRef, out var uom))
            {
                variant.BaseUom = $"{uom.Code} - {uom.Name}";
            }
        }

        var filteredVariants = ApplyVariantFilters(variants, null, trackingPolicy, null, sort);
        var normalizedPage = variantsSearchResult.Data?.Page ?? Math.Max(page, 1);
        var normalizedPageSize = variantsSearchResult.Data?.PageSize ?? (PageSizeOptions.Contains(pageSize) ? pageSize : PageSizeOptions[0]);
        var totalCount = variantsSearchResult.Data?.TotalCount ?? filteredVariants.Count;
        var totalPages = Math.Max(1, (int)Math.Ceiling(totalCount / (double)Math.Max(normalizedPageSize, 1)));
        var pagedVariants = filteredVariants;

        var isVariantCreateMode = createNew;
        var selectedVariantId = isVariantCreateMode
            ? null
            : ResolveSelectedVariantId(variantId, variants, pagedVariants);

        var variantDetailsResult = !string.IsNullOrWhiteSpace(selectedVariantId)
            ? await _apiService.GetProductVariantFullDetailsAsync(selectedVariantId, token)
            : new ApiResponse<ProductVariantDetailsModel> { IsSuccess = true };

        if (variantDetailsResult.Data is not null &&
            uomById.TryGetValue(variantDetailsResult.Data.BaseUomRef, out var detailUom))
        {
            variantDetailsResult.Data.BaseUom = $"{detailUom.Code} - {detailUom.Name}";
        }

        var variantConversionsResult = !string.IsNullOrWhiteSpace(selectedVariantId)
            ? await _apiService.GetVariantUomConversionsByVariantIdAsync(selectedVariantId, token)
            : new ApiResponse<List<VariantUomConversionModel>> { IsSuccess = true, Data = new List<VariantUomConversionModel>() };

        var selectedProductCategoryId =
            variantDetailsResult.Data?.CategoryId ??
            products.FirstOrDefault(p => string.Equals(p.Id, selectedProductId, StringComparison.OrdinalIgnoreCase))?.CategoryId ??
            selectedCategoryId;

        var (attributeGroups, effectiveAttributes, attributesError) =
            await LoadEffectiveCategoryAttributesAsync(selectedProductCategoryId, flatCategories, token);
        var effectiveVariantAttributes = FilterEffectiveAttributesForVariant(effectiveAttributes);
        var filteredVariantAttributes = ApplyAttributeTypeFilter(effectiveVariantAttributes, attributeTypeFilter);

        var missingRequired = !string.IsNullOrWhiteSpace(selectedVariantId)
            ? FindMissingRequiredAttributes(
                effectiveVariantAttributes,
                variantDetailsResult.Data?.Attributes?.ToDictionary(
                    x => x.AttributeId,
                    x => x.Value,
                    StringComparer.OrdinalIgnoreCase)
                ?? new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase))
            : Array.Empty<EffectiveAttributeViewModel>();

        var permissions = ResolvePermissionsFromSession();

        var model = new VariantManagementPageViewModel
        {
            UserName = HttpContext.Session.GetString("UserName") ?? "کاربر",
            Roles = roles,
            Permissions = permissions,
            Modules = modules,
            ActiveModule = menu.Module,
            ActiveItem = menu.Item,
            IsVariantCreateMode = isVariantCreateMode,
            SelectedProductId = selectedProductId,
            SelectedVariantId = selectedVariantId,
            SelectedCategoryId = selectedProductCategoryId,
            Categories = categories,
            FlatCategories = flatCategories,
            Products = products,
            Variants = pagedVariants,
            SelectedVariantDetails = variantDetailsResult.Data,
            UnitOfMeasures = unitOfMeasures,
            VariantUomConversions = variantConversionsResult.Data ?? new List<VariantUomConversionModel>(),

            ProductAttributeGroups = attributeGroups,
            EffectiveProductAttributes = filteredVariantAttributes,
            MissingRequiredVariantAttributes = missingRequired,

            VariantSearchTerm = searchTerm,
            VariantCategoryFilterId = selectedCategoryId,
            VariantAttributeOptionFilterIds = attributeOptionIds,
            VariantTrackingFilter = trackingPolicy,
            VariantStatusFilter = statusFilter,
            SelectedAttributeTypeFilter = attributeTypeFilter,
            VariantSort = sort,
            VariantPage = normalizedPage,
            VariantPageSize = normalizedPageSize,
            VariantTotalCount = totalCount,
            VariantTotalPages = totalPages,
            VariantPageSizeOptions = PageSizeOptions,

            ErrorMessage = JoinErrors(
                productsResult.ErrorMessage,
                uomLookupResult.ErrorMessage,
                variantsSearchResult.ErrorMessage,
                variantDetailsResult.ErrorMessage,
                variantConversionsResult.ErrorMessage,
                categoriesResult.ErrorMessage,
                attributesError),

            VariantForm = new VariantUpsertForm
            {
                ProductId = selectedProductId ?? string.Empty,
                VariantId = variantDetailsResult.Data?.Id,
                Sku = variantDetailsResult.Data?.Sku ?? string.Empty,
                Barcode = variantDetailsResult.Data?.Barcode,
                BaseUomRef = variantDetailsResult.Data?.BaseUomRef ?? string.Empty,
                TrackingPolicy = variantDetailsResult.Data?.TrackingPolicy ?? "None",
                IsActive = variantDetailsResult.Data?.IsActive ?? true
            },
            VariantAttributeForm = new VariantAttributeValueForm
            {
                ProductId = selectedProductId ?? string.Empty,
                VariantId = selectedVariantId ?? string.Empty
            },
            VariantUomConversionForm = new VariantUomConversionForm
            {
                ProductId = selectedProductId ?? string.Empty,
                VariantId = selectedVariantId ?? string.Empty,
                RoundingMode = "None",
                Factor = 1m
            }
        };

        SetLayoutViewBag(model.Modules, model.ActiveModule?.ModuleId, model.ActiveItem?.ItemId, model.UserName);
        return View("~/Views/CatalogManagement/Variants.cshtml", model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    protected async Task<IActionResult> SaveVariant(VariantUpsertForm form)
    {
        if (!TryGetToken(out var token))
        {
            return RedirectToAction("Login", "Auth");
        }

        if (!IsAuthorizedFor(token, "Catalog.Variant.Create", "Catalog.Variant.Update", "ProductVariant.Create", "ProductVariant.Update"))
        {
            TempData["CatalogError"] = "شما دسترسی ذخیره واریانت را ندارید.";
            return RedirectToAction(nameof(Variants), new { productId = form.ProductId, variantId = form.VariantId });
        }

        if (!TryValidateModel(form))
        {
            TempData["CatalogError"] = ExtractModelError(ModelState);
            return RedirectToAction(nameof(Variants), new { productId = form.ProductId, variantId = form.VariantId });
        }

        if (!AllowedTrackingPolicies.Contains(form.TrackingPolicy))
        {
            TempData["CatalogError"] = "Tracking Policy نامعتبر است.";
            return RedirectToAction(nameof(Variants), new { productId = form.ProductId, variantId = form.VariantId });
        }

        var productsResult = await _apiService.SearchProductsAsync(token);
        var selectedProduct = (productsResult.Data ?? new List<ProductSummaryModel>())
            .FirstOrDefault(x => string.Equals(x.Id, form.ProductId, StringComparison.OrdinalIgnoreCase));

        if (selectedProduct is null)
        {
            TempData["CatalogError"] = "محصول انتخاب‌شده معتبر نیست.";
            return RedirectToAction(nameof(Variants), new { productId = form.ProductId, variantId = form.VariantId });
        }

        var uomLookupResult = await _apiService.GetUnitOfMeasureLookupAsync(token);
        var validUom = (uomLookupResult.Data ?? new List<UnitOfMeasureLookupModel>())
            .Any(x => string.Equals(x.Id, form.BaseUomRef, StringComparison.OrdinalIgnoreCase));
        if (!validUom)
        {
            TempData["CatalogError"] = "واحد پایه انتخاب‌شده معتبر نیست.";
            return RedirectToAction(nameof(Variants), new { productId = form.ProductId, variantId = form.VariantId });
        }

        var categoriesResult = await _apiService.GetCategoryTreeAsync(token);
        var flatCategories = FlattenCategories(categoriesResult.Data ?? new List<CategoryNodeModel>()).ToList();
        var (_, effectiveAttributes, attributesError) =
            await LoadEffectiveCategoryAttributesAsync(selectedProduct.CategoryId, flatCategories, token);
        var effectiveVariantAttributes = FilterEffectiveAttributesForVariant(effectiveAttributes);

        if (!string.IsNullOrWhiteSpace(attributesError))
        {
            TempData["CatalogError"] = attributesError;
            return RedirectToAction(nameof(Variants), new { productId = form.ProductId, variantId = form.VariantId });
        }

        if (!string.IsNullOrWhiteSpace(form.VariantId))
        {
            var detailsResult = await _apiService.GetProductVariantFullDetailsAsync(form.VariantId, token);
            var existingValues = detailsResult.Data?.Attributes?.ToDictionary(
                x => x.AttributeId,
                x => x.Value,
                StringComparer.OrdinalIgnoreCase)
                ?? new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase);

            var missingRequired = FindMissingRequiredAttributes(effectiveVariantAttributes, existingValues);
            if (missingRequired.Count > 0)
            {
                TempData["CatalogError"] = "ابتدا مقدار Attributeهای الزامی Variant را کامل کنید: " +
                    string.Join("، ", missingRequired.Select(x => x.Name));
                return RedirectToAction(nameof(Variants), new { productId = form.ProductId, variantId = form.VariantId });
            }
        }
        else
        {
            var requiredCount = effectiveVariantAttributes.Count(x => x.IsRequired);
            if (requiredCount > 0)
            {
                TempData["CatalogNotice"] = "برای این محصول Attribute الزامی تعریف شده است. پس از ایجاد Variant، مقادیر الزامی را ثبت کنید.";
            }
        }

        var request = new UpsertVariantRequest
        {
            ProductId = form.ProductId,
            Sku = form.Sku.Trim(),
            Barcode = string.IsNullOrWhiteSpace(form.Barcode) ? null : form.Barcode.Trim(),
            BaseUomRef = form.BaseUomRef,
            TrackingPolicy = form.TrackingPolicy.Trim(),
            IsActive = form.IsActive
        };

        ApiResponse<bool> result;
        if (string.IsNullOrWhiteSpace(form.VariantId))
        {
            result = await _apiService.CreateProductVariantAsync(form.ProductId, request, token);
        }
        else
        {
            result = await _apiService.UpdateProductVariantAsync(form.ProductId, form.VariantId, request, token);
        }

        if (!result.IsSuccess)
        {
            TempData["CatalogError"] = result.ErrorMessage ?? "عملیات با خطا مواجه شد.";
        }
        else
        {
            TempData["CatalogSuccess"] = string.IsNullOrWhiteSpace(form.VariantId)
                ? "واریانت با موفقیت ایجاد شد."
                : "واریانت با موفقیت به‌روزرسانی شد.";
        }

        return RedirectToAction(nameof(Variants), new { productId = form.ProductId, variantId = form.VariantId, createNew = string.IsNullOrWhiteSpace(form.VariantId) });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    protected async Task<IActionResult> SetVariantAttributeValue(VariantAttributeValueForm form)
    {
        if (!TryGetToken(out var token))
        {
            return RedirectToAction("Login", "Auth");
        }

        if (!IsAuthorizedFor(token, "Catalog.VariantAttributeValue.Set", "VariantAttributeValue.Set"))
        {
            TempData["CatalogError"] = "شما دسترسی ثبت مقدار اتریبیوت واریانت را ندارید.";
            return RedirectToAction(nameof(Variants), new { productId = form.ProductId, variantId = form.VariantId });
        }

        if (!TryValidateModel(form))
        {
            TempData["CatalogError"] = ExtractModelError(ModelState);
            return RedirectToAction(nameof(Variants), new { productId = form.ProductId, variantId = form.VariantId });
        }

        var productsResult = await _apiService.SearchProductsAsync(token);
        var selectedProduct = (productsResult.Data ?? new List<ProductSummaryModel>())
            .FirstOrDefault(x => string.Equals(x.Id, form.ProductId, StringComparison.OrdinalIgnoreCase));

        if (selectedProduct is null)
        {
            TempData["CatalogError"] = "محصول انتخاب‌شده معتبر نیست.";
            return RedirectToAction(nameof(Variants), new { productId = form.ProductId, variantId = form.VariantId });
        }

        var categoriesResult = await _apiService.GetCategoryTreeAsync(token);
        var flatCategories = FlattenCategories(categoriesResult.Data ?? new List<CategoryNodeModel>()).ToList();
        var (_, effectiveAttributes, attributesError) =
            await LoadEffectiveCategoryAttributesAsync(selectedProduct.CategoryId, flatCategories, token);
        var effectiveVariantAttributes = FilterEffectiveAttributesForVariant(effectiveAttributes);

        if (!string.IsNullOrWhiteSpace(attributesError))
        {
            TempData["CatalogError"] = attributesError;
            return RedirectToAction(nameof(Variants), new { productId = form.ProductId, variantId = form.VariantId });
        }

        var selectedAttribute = effectiveVariantAttributes
            .FirstOrDefault(x => string.Equals(x.AttributeId, form.AttributeId, StringComparison.OrdinalIgnoreCase));

        if (selectedAttribute is null)
        {
            TempData["CatalogError"] = "اتریبیوت انتخاب‌شده معتبر نیست.";
            return RedirectToAction(nameof(Variants), new { productId = form.ProductId, variantId = form.VariantId });
        }

        if (!TryValidateAttributeValue(selectedAttribute, form.Value, out var errorMessage))
        {
            TempData["CatalogError"] = errorMessage;
            return RedirectToAction(nameof(Variants), new { productId = form.ProductId, variantId = form.VariantId });
        }

        var normalizedIncomingValue = form.Value?.Trim() ?? string.Empty;
        var selectedOption = selectedAttribute.Options.FirstOrDefault(x =>
            string.Equals(x.OptionValue, normalizedIncomingValue, StringComparison.OrdinalIgnoreCase)
            || string.Equals(x.OptionName, normalizedIncomingValue, StringComparison.OrdinalIgnoreCase));
        var selectedOptionId = selectedOption?.Id;
        var normalizedOptionValue = selectedOption?.OptionValue ?? normalizedIncomingValue;

        var variantAttributeValuesResult = await _apiService.GetVariantAttributeValuesByVariantIdAsync(form.VariantId, token);
        var existingValue = (variantAttributeValuesResult.Data ?? new List<VariantAttributeValueModel>())
            .FirstOrDefault(x => string.Equals(x.AttributeId, form.AttributeId, StringComparison.OrdinalIgnoreCase));
        if (!string.IsNullOrWhiteSpace(selectedOptionId))
        {
            if (string.Equals(existingValue?.OptionId, selectedOptionId, StringComparison.OrdinalIgnoreCase))
            {
                TempData["CatalogNotice"] = "مقدار اتریبیوت تغییری نکرده است.";
                return RedirectToAction(nameof(Variants), new { productId = form.ProductId, variantId = form.VariantId });
            }
        }
        else
        {
            var normalizedExistingValue = existingValue?.OptionValue?.Trim();
            if (string.IsNullOrWhiteSpace(normalizedExistingValue))
            {
                normalizedExistingValue = existingValue?.Value?.Trim();
            }

            if (!string.IsNullOrWhiteSpace(normalizedExistingValue) &&
                string.Equals(normalizedExistingValue, normalizedIncomingValue, StringComparison.OrdinalIgnoreCase))
            {
                TempData["CatalogNotice"] = "مقدار اتریبیوت تغییری نکرده است.";
                return RedirectToAction(nameof(Variants), new { productId = form.ProductId, variantId = form.VariantId });
            }
        }

        var result = await _apiService.SetVariantAttributeValueAsync(
            form.VariantId,
            new SetVariantAttributeValueRequest
            {
                AttributeId = form.AttributeId,
                Value = normalizedOptionValue,
                OptionId = selectedOptionId
            },
            token);

        if (!result.IsSuccess)
        {
            TempData["CatalogError"] = result.ErrorMessage ?? "عملیات با خطا مواجه شد.";
        }

        return RedirectToAction(nameof(Variants), new { productId = form.ProductId, variantId = form.VariantId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    protected async Task<IActionResult> RemoveVariantAttributeValue(string productId, string variantId, string attributeId)
    {
        if (!TryGetToken(out var token))
        {
            return RedirectToAction("Login", "Auth");
        }

        if (!IsAuthorizedFor(token, "Catalog.VariantAttributeValue.Remove", "VariantAttributeValue.Remove"))
        {
            TempData["CatalogError"] = "شما دسترسی حذف مقدار اتریبیوت واریانت را ندارید.";
            return RedirectToAction(nameof(Variants), new { productId, variantId });
        }

        if (string.IsNullOrWhiteSpace(variantId) || string.IsNullOrWhiteSpace(attributeId))
        {
            TempData["CatalogError"] = "شناسه واریانت یا اتریبیوت معتبر نیست.";
            return RedirectToAction(nameof(Variants), new { productId, variantId });
        }

        var result = await _apiService.RemoveVariantAttributeValueAsync(variantId, attributeId, token);
        if (!result.IsSuccess)
        {
            TempData["CatalogError"] = result.ErrorMessage ?? "حذف مقدار اتریبیوت واریانت انجام نشد.";
        }

        return RedirectToAction(nameof(Variants), new { productId, variantId });
    }

    private async Task<(IReadOnlyList<CategoryAttributeGroupViewModel> Groups, IReadOnlyList<EffectiveAttributeViewModel> Effective, string? Error)> LoadEffectiveCategoryAttributesAsync(
        string? categoryId,
        IReadOnlyList<CategoryNodeModel> flatCategories,
        string token)
    {
        if (string.IsNullOrWhiteSpace(categoryId))
        {
            return (Array.Empty<CategoryAttributeGroupViewModel>(), Array.Empty<EffectiveAttributeViewModel>(), null);
        }

        var lineage = BuildCategoryLineage(flatCategories, categoryId);
        if (lineage.Count == 0)
        {
            return (Array.Empty<CategoryAttributeGroupViewModel>(), Array.Empty<EffectiveAttributeViewModel>(), "دسته‌بندی انتخاب‌شده در ساختار فعلی یافت نشد.");
        }

        var attributeTasks = lineage
            .Select(node => _apiService.GetCategoryAttributesAsync(node.Id, token, includeInherited: false, includeInactive: false))
            .ToList();

        await Task.WhenAll(attributeTasks);

        var groups = new List<CategoryAttributeGroupViewModel>();
        var errors = new List<string>();

        for (var i = 0; i < lineage.Count; i++)
        {
            var node = lineage[i];
            var result = attributeTasks[i].Result;

            if (!string.IsNullOrWhiteSpace(result.ErrorMessage))
            {
                errors.Add(result.ErrorMessage);
            }

            groups.Add(new CategoryAttributeGroupViewModel
            {
                CategoryId = node.Id,
                CategoryName = node.Name,
                IsCurrentCategory = string.Equals(node.Id, categoryId, StringComparison.OrdinalIgnoreCase),
                Attributes = result.Data ?? new List<AttributeDefinitionModel>()
            });
        }

        var effective = BuildEffectiveAttributes(groups);
        var errorMessage = errors.Count == 0 ? null : string.Join(" | ", errors.Distinct());

        return (groups, effective, errorMessage);
    }

    private static IReadOnlyList<EffectiveAttributeViewModel> BuildEffectiveAttributes(IReadOnlyList<CategoryAttributeGroupViewModel> groups)
    {
        var indexByAttributeId = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        var effective = new List<EffectiveAttributeViewModel>();

        foreach (var group in groups)
        {
            foreach (var attribute in group.Attributes)
            {
                var mapped = new EffectiveAttributeViewModel
                {
                    AttributeId = attribute.Id,
                    Name = attribute.Name,
                    DataType = attribute.DataType,
                    Scope = attribute.Scope,
                    IsVariantLevel = attribute.IsVariant,
                    IsRequired = attribute.IsRequired,
                    DisplayOrder = attribute.DisplayOrder,
                    Options = attribute.Options,
                    SourceCategoryId = group.CategoryId,
                    SourceCategoryName = group.CategoryName,
                    IsInherited = !group.IsCurrentCategory
                };

                if (indexByAttributeId.TryGetValue(attribute.Id, out var existingIndex))
                {
                    effective[existingIndex] = mapped;
                }
                else
                {
                    indexByAttributeId[attribute.Id] = effective.Count;
                    effective.Add(mapped);
                }
            }
        }

        return effective;
    }

    private static IReadOnlyList<EffectiveAttributeViewModel> FindMissingRequiredAttributes(
        IReadOnlyList<EffectiveAttributeViewModel> effectiveAttributes,
        IReadOnlyDictionary<string, string?> assignedValues)
    {
        var missing = new List<EffectiveAttributeViewModel>();

        foreach (var required in effectiveAttributes.Where(x => x.IsRequired))
        {
            var found = assignedValues.TryGetValue(required.AttributeId, out var value);
            if (!found || string.IsNullOrWhiteSpace(value))
            {
                missing.Add(required);
            }
        }

        return missing;
    }

    private static bool TryParseProductAutoVariantPayload(string? rawPayload, out ProductAutoVariantPayload payload)
    {
        payload = new ProductAutoVariantPayload();
        if (string.IsNullOrWhiteSpace(rawPayload))
        {
            return true;
        }

        try
        {
            payload = JsonSerializer.Deserialize<ProductAutoVariantPayload>(rawPayload, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }) ?? new ProductAutoVariantPayload();
            payload.ProductAttributes ??= new List<ProductAutoAttributeItem>();
            payload.VariantAttributes ??= new List<ProductAutoVariantAttributeItem>();
            return true;
        }
        catch
        {
            payload = new ProductAutoVariantPayload();
            return false;
        }
    }

    private static string? BuildProductCreateAttributeInputs(
        ProductAutoVariantPayload payload,
        IReadOnlyList<EffectiveAttributeViewModel> effectiveProductAttributes,
        List<CatalogAttributeValueInputModel> output)
    {
        output.Clear();

        var incoming = payload.ProductAttributes
            .Where(x => !string.IsNullOrWhiteSpace(x.AttributeId))
            .GroupBy(x => x.AttributeId, StringComparer.OrdinalIgnoreCase)
            .Select(x => x.Last())
            .ToDictionary(x => x.AttributeId, StringComparer.OrdinalIgnoreCase);

        foreach (var attribute in effectiveProductAttributes)
        {
            if (!incoming.TryGetValue(attribute.AttributeId, out var item))
            {
                continue;
            }

            var optionId = string.IsNullOrWhiteSpace(item.OptionId) ? null : item.OptionId.Trim();
            var value = string.IsNullOrWhiteSpace(item.Value) ? null : item.Value.Trim();

            if (SupportsOptions(attribute.DataType))
            {
                if (string.IsNullOrWhiteSpace(optionId) && string.IsNullOrWhiteSpace(value))
                {
                    if (attribute.IsRequired)
                    {
                        return $"ویژگی الزامی \"{attribute.Name}\" باید مقدار داشته باشد.";
                    }

                    continue;
                }

                var selectedOption = attribute.Options.FirstOrDefault(o =>
                    (!string.IsNullOrWhiteSpace(optionId) && string.Equals(o.Id, optionId, StringComparison.OrdinalIgnoreCase))
                    || (!string.IsNullOrWhiteSpace(value) &&
                        (string.Equals(o.OptionValue, value, StringComparison.OrdinalIgnoreCase)
                         || string.Equals(o.OptionName, value, StringComparison.OrdinalIgnoreCase))));

                if (selectedOption is null)
                {
                    return $"گزینه انتخابی برای ویژگی \"{attribute.Name}\" معتبر نیست.";
                }

                output.Add(new CatalogAttributeValueInputModel
                {
                    AttributeId = attribute.AttributeId,
                    OptionId = selectedOption.Id,
                    Value = selectedOption.OptionValue
                });

                continue;
            }

            if (attribute.IsRequired && string.IsNullOrWhiteSpace(value))
            {
                return $"ویژگی الزامی \"{attribute.Name}\" باید مقدار داشته باشد.";
            }

            if (!string.IsNullOrWhiteSpace(value))
            {
                output.Add(new CatalogAttributeValueInputModel
                {
                    AttributeId = attribute.AttributeId,
                    Value = value
                });
            }
        }

        return null;
    }

    private static string? BuildVariantGenerationDimensions(
        ProductAutoVariantPayload payload,
        IReadOnlyList<EffectiveAttributeViewModel> effectiveVariantAttributes,
        out List<VariantDimensionSelection> dimensions)
    {
        dimensions = new List<VariantDimensionSelection>();
        var incoming = payload.VariantAttributes
            .Where(x => !string.IsNullOrWhiteSpace(x.AttributeId))
            .GroupBy(x => x.AttributeId, StringComparer.OrdinalIgnoreCase)
            .Select(x => x.Last())
            .ToDictionary(x => x.AttributeId, StringComparer.OrdinalIgnoreCase);

        foreach (var attribute in effectiveVariantAttributes.OrderBy(x => x.DisplayOrder).ThenBy(x => x.Name))
        {
            incoming.TryGetValue(attribute.AttributeId, out var item);
            var incomingOptionIds = (item?.OptionIds ?? new List<string>())
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(x => x.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            if (!SupportsOptions(attribute.DataType))
            {
                if (attribute.IsRequired)
                {
                    return $"ویژگی واریانت‌ساز \"{attribute.Name}\" باید از نوع Option باشد تا تولید خودکار واریانت ممکن شود.";
                }

                continue;
            }

            if (incomingOptionIds.Count == 0)
            {
                if (attribute.IsRequired)
                {
                    return $"برای ویژگی واریانت‌ساز \"{attribute.Name}\" باید حداقل یک گزینه انتخاب شود.";
                }

                continue;
            }

            var selectedOptions = new List<VariantDimensionOptionSelection>();
            foreach (var optionId in incomingOptionIds)
            {
                var option = attribute.Options.FirstOrDefault(o => string.Equals(o.Id, optionId, StringComparison.OrdinalIgnoreCase));
                if (option is null)
                {
                    return $"گزینه انتخابی برای ویژگی \"{attribute.Name}\" معتبر نیست.";
                }

                selectedOptions.Add(new VariantDimensionOptionSelection
                {
                    AttributeId = attribute.AttributeId,
                    OptionId = option.Id,
                    OptionName = option.OptionName,
                    OptionValue = option.OptionValue,
                    OptionDisplayOrder = option.DisplayOrder
                });
            }

            if (selectedOptions.Count == 0)
            {
                continue;
            }

            dimensions.Add(new VariantDimensionSelection
            {
                AttributeId = attribute.AttributeId,
                AttributeName = attribute.Name,
                AttributeDisplayOrder = attribute.DisplayOrder,
                Options = selectedOptions
                    .OrderBy(x => x.OptionDisplayOrder)
                    .ThenBy(x => x.OptionName)
                    .ToList()
            });
        }

        return null;
    }

    private static List<GeneratedVariantPlan> BuildGeneratedVariantPlans(
        string productName,
        string baseSku,
        IReadOnlyList<VariantDimensionSelection> dimensions)
    {
        var plans = new List<GeneratedVariantPlan>();
        if (dimensions.Count == 0)
        {
            return plans;
        }

        var combinations = BuildVariantCombinations(dimensions);
        var usedSkus = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var combination in combinations)
        {
            var orderedSelection = combination
                .OrderBy(x => x.AttributeDisplayOrder)
                .ThenBy(x => x.AttributeName)
                .ThenBy(x => x.OptionDisplayOrder)
                .ThenBy(x => x.OptionName)
                .ToList();

            var skuSuffix = string.Join("-", orderedSelection
                .Select(x => NormalizeSkuSegment(x.OptionValue))
                .Where(x => !string.IsNullOrWhiteSpace(x)));

            var generatedSku = string.IsNullOrWhiteSpace(skuSuffix)
                ? NormalizeSkuSegment(baseSku)
                : $"{NormalizeSkuSegment(baseSku)}-{skuSuffix}";

            if (string.IsNullOrWhiteSpace(generatedSku))
            {
                generatedSku = $"VAR-{DateTime.UtcNow:yyyyMMddHHmmssfff}";
            }

            var candidate = generatedSku;
            var sequence = 2;
            while (!usedSkus.Add(candidate))
            {
                candidate = $"{generatedSku}-{sequence}";
                sequence++;
            }

            var generatedNameSuffix = string.Join(" / ", orderedSelection.Select(x => x.OptionName));
            var generatedName = string.IsNullOrWhiteSpace(generatedNameSuffix)
                ? productName
                : $"{productName} - {generatedNameSuffix}";

            plans.Add(new GeneratedVariantPlan
            {
                GeneratedName = generatedName,
                GeneratedSku = candidate,
                AttributeValues = orderedSelection
                    .Select(x => new CatalogAttributeValueInputModel
                    {
                        AttributeId = x.AttributeId,
                        OptionId = x.OptionId,
                        Value = x.OptionValue
                    })
                    .ToList()
            });
        }

        return plans;
    }

    private static List<List<VariantCombinationSelection>> BuildVariantCombinations(IReadOnlyList<VariantDimensionSelection> dimensions)
    {
        var combinations = new List<List<VariantCombinationSelection>> { new() };

        foreach (var dimension in dimensions)
        {
            var next = new List<List<VariantCombinationSelection>>();
            foreach (var existing in combinations)
            {
                foreach (var option in dimension.Options)
                {
                    var clone = new List<VariantCombinationSelection>(existing)
                    {
                        new VariantCombinationSelection
                        {
                            AttributeId = dimension.AttributeId,
                            AttributeName = dimension.AttributeName,
                            AttributeDisplayOrder = dimension.AttributeDisplayOrder,
                            OptionId = option.OptionId,
                            OptionName = option.OptionName,
                            OptionValue = option.OptionValue,
                            OptionDisplayOrder = option.OptionDisplayOrder
                        }
                    };

                    next.Add(clone);
                }
            }

            combinations = next;
        }

        return combinations;
    }

    private static string NormalizeSkuSegment(string? rawValue)
    {
        if (string.IsNullOrWhiteSpace(rawValue))
        {
            return string.Empty;
        }

        var builder = new StringBuilder(rawValue.Length);
        foreach (var ch in rawValue.Trim().ToUpperInvariant())
        {
            if (char.IsLetterOrDigit(ch))
            {
                builder.Append(ch);
                continue;
            }

            if (ch is '-' or '_')
            {
                builder.Append(ch);
                continue;
            }

            builder.Append('-');
        }

        var normalized = builder.ToString();
        while (normalized.Contains("--", StringComparison.Ordinal))
        {
            normalized = normalized.Replace("--", "-", StringComparison.Ordinal);
        }

        normalized = normalized.Trim('-', '_');
        return normalized;
    }

    private static string GenerateProductNumber(CategoryNodeModel category, IReadOnlyList<ProductSummaryModel> existingProducts)
    {
        var prefix = NormalizeSkuSegment(category.Code);
        if (string.IsNullOrWhiteSpace(prefix))
        {
            prefix = NormalizeSkuSegment(category.Name);
        }

        if (string.IsNullOrWhiteSpace(prefix))
        {
            prefix = "PRD";
        }

        var nextSequence = existingProducts
            .Select(x => TryReadProductNumberSequence(x.BaseSku, prefix))
            .Where(x => x.HasValue)
            .Select(x => x!.Value)
            .DefaultIfEmpty(0)
            .Max() + 1;

        return $"{prefix}-{nextSequence:000000}";
    }

    private static int? TryReadProductNumberSequence(string? productNumber, string prefix)
    {
        if (string.IsNullOrWhiteSpace(productNumber))
        {
            return null;
        }

        var normalized = NormalizeSkuSegment(productNumber);
        var normalizedPrefix = NormalizeSkuSegment(prefix);
        var expectedStart = normalizedPrefix + "-";
        if (!normalized.StartsWith(expectedStart, StringComparison.OrdinalIgnoreCase))
        {
            return null;
        }

        var suffix = normalized[expectedStart.Length..];
        var firstSegment = suffix.Split('-', StringSplitOptions.RemoveEmptyEntries).FirstOrDefault();
        return int.TryParse(firstSegment, out var sequence) ? sequence : null;
    }

    private static bool TryValidateAttributeValue(EffectiveAttributeViewModel attribute, string rawValue, out string errorMessage)
    {
        var value = rawValue?.Trim() ?? string.Empty;
        if (attribute.IsRequired && string.IsNullOrWhiteSpace(value))
        {
            errorMessage = $"مقدار اتریبیوت الزامی \"{attribute.Name}\" نمی‌تواند خالی باشد.";
            return false;
        }

        if (string.IsNullOrWhiteSpace(value))
        {
            errorMessage = string.Empty;
            return true;
        }

        if (attribute.Options.Count > 0 &&
            !attribute.Options.Any(o =>
                string.Equals(o.OptionValue, value, StringComparison.OrdinalIgnoreCase)
                || string.Equals(o.OptionName, value, StringComparison.OrdinalIgnoreCase)))
        {
            errorMessage = $"مقدار واردشده برای \"{attribute.Name}\" در بین Optionهای مجاز نیست.";
            return false;
        }

        var dataType = NormalizeDataType(attribute.DataType);

        if (dataType.Equals("Number", StringComparison.OrdinalIgnoreCase))
        {
            var numberValid = decimal.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out _)
                              || decimal.TryParse(value, NumberStyles.Any, CultureInfo.CurrentCulture, out _);
            if (!numberValid)
            {
                errorMessage = $"مقدار اتریبیوت \"{attribute.Name}\" باید عددی باشد.";
                return false;
            }
        }

        if (dataType.Equals("Boolean", StringComparison.OrdinalIgnoreCase))
        {
            if (!IsBooleanLike(value))
            {
                errorMessage = $"مقدار اتریبیوت \"{attribute.Name}\" باید بولین باشد (true/false).";
                return false;
            }
        }

        if (dataType.Equals("Date", StringComparison.OrdinalIgnoreCase))
        {
            if (!DateTime.TryParse(value, CultureInfo.CurrentCulture, DateTimeStyles.None, out _) &&
                !DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.None, out _))
            {
                errorMessage = $"مقدار اتریبیوت \"{attribute.Name}\" باید تاریخ معتبر باشد.";
                return false;
            }
        }

        errorMessage = string.Empty;
        return true;
    }

    private static bool IsBooleanLike(string value)
    {
        if (bool.TryParse(value, out _))
        {
            return true;
        }

        var normalized = value.Trim().ToLowerInvariant();
        return normalized is "1" or "0" or "yes" or "no" or "بله" or "خیر";
    }

    private static string NormalizeDataType(string? dataType) =>
        string.IsNullOrWhiteSpace(dataType) ? string.Empty : dataType.Trim();

    private static bool SupportsOptions(string? dataType) =>
        OptionSupportedAttributeDataTypes.Contains(NormalizeDataType(dataType));

    private static bool CreatesCategoryCycle(IReadOnlyList<CategoryNodeModel> flatCategories, string categoryId, string? parentCategoryId)
    {
        if (string.IsNullOrWhiteSpace(parentCategoryId))
        {
            return false;
        }

        var map = flatCategories.ToDictionary(x => x.Id, x => x, StringComparer.OrdinalIgnoreCase);
        var current = parentCategoryId;
        var safetyCounter = 0;

        while (!string.IsNullOrWhiteSpace(current) && safetyCounter < 2000)
        {
            if (string.Equals(current, categoryId, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            if (!map.TryGetValue(current, out var node))
            {
                return false;
            }

            current = node.ParentCategoryId;
            safetyCounter++;
        }

        return false;
    }

    private static IReadOnlyList<CategoryNodeModel> BuildCategoryLineage(IReadOnlyList<CategoryNodeModel> flatCategories, string categoryId)
    {
        var map = flatCategories.ToDictionary(x => x.Id, x => x, StringComparer.OrdinalIgnoreCase);
        if (!map.TryGetValue(categoryId, out var start))
        {
            return Array.Empty<CategoryNodeModel>();
        }

        var lineage = new List<CategoryNodeModel>();
        var current = start;
        var visited = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        while (current is not null)
        {
            if (!visited.Add(current.Id))
            {
                break;
            }

            lineage.Insert(0, current);

            if (string.IsNullOrWhiteSpace(current.ParentCategoryId) ||
                !map.TryGetValue(current.ParentCategoryId, out var parent))
            {
                break;
            }

            current = parent;
        }

        return lineage;
    }

    private static IReadOnlyList<CategoryNodeModel> ApplyCategoryFilters(
        IReadOnlyList<CategoryNodeModel> categories,
        string? searchTerm,
        string? statusFilter,
        string? sort)
    {
        IEnumerable<CategoryNodeModel> query = categories;

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = searchTerm.Trim();
            query = query.Where(c =>
                ContainsText(c.Name, term) ||
                ContainsText(c.Code, term) ||
                ContainsText(c.Id, term));
        }

        if (TryParseStatusFilter(statusFilter, out var isActive))
        {
            query = query.Where(c => c.IsActive == isActive);
        }

        var normalizedSort = (sort ?? string.Empty).Trim().ToLowerInvariant();
        query = normalizedSort switch
        {
            "name_desc" => query.OrderByDescending(c => c.Name).ThenBy(c => c.Id),
            "code_asc" => query.OrderBy(c => c.Code).ThenBy(c => c.Id),
            "code_desc" => query.OrderByDescending(c => c.Code).ThenBy(c => c.Id),
            "displayorder_asc" => query.OrderBy(c => c.DisplayOrder).ThenBy(c => c.Name),
            "displayorder_desc" => query.OrderByDescending(c => c.DisplayOrder).ThenBy(c => c.Name),
            "status_asc" => query.OrderBy(c => c.IsActive ? 0 : 1).ThenBy(c => c.Name),
            "status_desc" => query.OrderBy(c => c.IsActive ? 1 : 0).ThenBy(c => c.Name),
            _ => query.OrderBy(c => c.Name).ThenBy(c => c.Id)
        };

        return query.ToList();
    }

    private static IReadOnlyList<EffectiveAttributeViewModel> FilterEffectiveAttributesForProduct(
        IReadOnlyList<EffectiveAttributeViewModel> attributes)
    {
        return attributes
            .Where(IsProductScope)
            .ToList();
    }

    private static IReadOnlyList<EffectiveAttributeViewModel> FilterEffectiveAttributesForVariant(
        IReadOnlyList<EffectiveAttributeViewModel> attributes)
    {
        return attributes
            .Where(IsVariantScope)
            .ToList();
    }

    private static IReadOnlyList<EffectiveAttributeViewModel> ApplyAttributeTypeFilter(
        IReadOnlyList<EffectiveAttributeViewModel> attributes,
        string? attributeTypeFilter)
    {
        if (string.IsNullOrWhiteSpace(attributeTypeFilter))
        {
            return attributes;
        }

        var normalized = attributeTypeFilter.Trim();
        return attributes
            .Where(x => string.Equals(x.DataType, normalized, StringComparison.OrdinalIgnoreCase))
            .ToList();
    }

    private static bool IsProductScope(EffectiveAttributeViewModel attribute)
    {
        var scope = (attribute.Scope ?? string.Empty).Trim().ToLowerInvariant();
        if (scope is "product" or "both")
        {
            return true;
        }

        if (scope == "variant")
        {
            return false;
        }

        return !attribute.IsVariantLevel;
    }

    private static bool IsVariantScope(EffectiveAttributeViewModel attribute)
    {
        var scope = (attribute.Scope ?? string.Empty).Trim().ToLowerInvariant();
        if (scope is "variant" or "both")
        {
            return true;
        }

        if (scope == "product")
        {
            return false;
        }

        return attribute.IsVariantLevel;
    }

    private static IReadOnlyList<ProductSummaryModel> ApplyProductFilters(
        IReadOnlyList<ProductSummaryModel> products,
        string? searchTerm,
        string? categoryFilterId,
        string? statusFilter,
        string? sort)
    {
        IEnumerable<ProductSummaryModel> query = products;

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = searchTerm.Trim();
            query = query.Where(p =>
                ContainsText(p.Name, term) ||
                ContainsText(p.BaseSku, term) ||
                ContainsText(p.Description, term) ||
                ContainsText(p.CategoryName, term) ||
                ContainsText(p.Id, term));
        }

        if (!string.IsNullOrWhiteSpace(categoryFilterId))
        {
            query = query.Where(p => string.Equals(p.CategoryId, categoryFilterId, StringComparison.OrdinalIgnoreCase));
        }

        if (TryParseStatusFilter(statusFilter, out var isActive))
        {
            query = query.Where(p => p.IsActive == isActive);
        }

        var normalizedSort = (sort ?? string.Empty).Trim().ToLowerInvariant();
        query = normalizedSort switch
        {
            "name_desc" => query.OrderByDescending(p => p.Name).ThenBy(p => p.Id),
            "basesku_asc" => query.OrderBy(p => p.BaseSku).ThenBy(p => p.Id),
            "basesku_desc" => query.OrderByDescending(p => p.BaseSku).ThenBy(p => p.Id),
            "status_asc" => query.OrderBy(p => p.IsActive ? 0 : 1).ThenBy(p => p.Name),
            "status_desc" => query.OrderBy(p => p.IsActive ? 1 : 0).ThenBy(p => p.Name),
            _ => query.OrderBy(p => p.Name).ThenBy(p => p.Id)
        };

        return query.ToList();
    }

    private static IReadOnlyList<ProductVariantSummaryModel> ApplyVariantFilters(
        IReadOnlyList<ProductVariantSummaryModel> variants,
        string? searchTerm,
        string? trackingPolicy,
        string? statusFilter,
        string? sort)
    {
        IEnumerable<ProductVariantSummaryModel> query = variants;

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = searchTerm.Trim();
            query = query.Where(v =>
                ContainsText(v.Sku, term) ||
                ContainsText(v.Barcode, term) ||
                ContainsText(v.Id, term));
        }

        if (!string.IsNullOrWhiteSpace(trackingPolicy))
        {
            query = query.Where(v => string.Equals(v.TrackingPolicy, trackingPolicy, StringComparison.OrdinalIgnoreCase));
        }

        if (TryParseStatusFilter(statusFilter, out var isActive))
        {
            query = query.Where(v => v.IsActive == isActive);
        }

        var normalizedSort = (sort ?? string.Empty).Trim().ToLowerInvariant();
        query = normalizedSort switch
        {
            "sku_desc" => query.OrderByDescending(v => v.Sku).ThenBy(v => v.Id),
            "tracking_asc" => query.OrderBy(v => v.TrackingPolicy).ThenBy(v => v.Sku),
            "tracking_desc" => query.OrderByDescending(v => v.TrackingPolicy).ThenBy(v => v.Sku),
            "status_asc" => query.OrderBy(v => v.IsActive ? 0 : 1).ThenBy(v => v.Sku),
            "status_desc" => query.OrderBy(v => v.IsActive ? 1 : 0).ThenBy(v => v.Sku),
            _ => query.OrderBy(v => v.Sku).ThenBy(v => v.Id)
        };

        return query.ToList();
    }

    private static bool TryParseStatusFilter(string? statusFilter, out bool isActive)
    {
        isActive = false;
        if (string.IsNullOrWhiteSpace(statusFilter))
        {
            return false;
        }

        var normalized = statusFilter.Trim().ToLowerInvariant();
        if (normalized is "active" or "true" or "1")
        {
            isActive = true;
            return true;
        }

        if (normalized is "inactive" or "false" or "0")
        {
            isActive = false;
            return true;
        }

        return false;
    }

    private static IReadOnlyList<T> Paginate<T>(
        IReadOnlyList<T> source,
        int page,
        int pageSize,
        out int normalizedPage,
        out int normalizedPageSize,
        out int totalCount,
        out int totalPages)
    {
        normalizedPageSize = PageSizeOptions.Contains(pageSize) ? pageSize : PageSizeOptions[0];
        totalCount = source.Count;
        totalPages = Math.Max(1, (int)Math.Ceiling(totalCount / (double)normalizedPageSize));
        normalizedPage = Math.Clamp(page, 1, totalPages);

        if (totalCount == 0)
        {
            return Array.Empty<T>();
        }

        var skip = (normalizedPage - 1) * normalizedPageSize;
        return source.Skip(skip).Take(normalizedPageSize).ToList();
    }

    private static string? ResolveSelectedProductId(
        string? requestedProductId,
        IReadOnlyList<ProductSummaryModel> filteredProducts,
        IReadOnlyList<ProductSummaryModel> pagedProducts)
    {
        if (!string.IsNullOrWhiteSpace(requestedProductId) &&
            filteredProducts.Any(p => string.Equals(p.Id, requestedProductId, StringComparison.OrdinalIgnoreCase)))
        {
            return requestedProductId;
        }

        return pagedProducts.FirstOrDefault()?.Id ?? filteredProducts.FirstOrDefault()?.Id;
    }

    private static string? ResolveSelectedVariantId(
        string? requestedVariantId,
        IReadOnlyList<ProductVariantSummaryModel> filteredVariants,
        IReadOnlyList<ProductVariantSummaryModel> pagedVariants)
    {
        if (!string.IsNullOrWhiteSpace(requestedVariantId) &&
            filteredVariants.Any(v => string.Equals(v.Id, requestedVariantId, StringComparison.OrdinalIgnoreCase)))
        {
            return requestedVariantId;
        }

        return pagedVariants.FirstOrDefault()?.Id ?? filteredVariants.FirstOrDefault()?.Id;
    }

    private static bool ContainsText(string? source, string term)
    {
        if (string.IsNullOrWhiteSpace(source) || string.IsNullOrWhiteSpace(term))
        {
            return false;
        }

        return source.Contains(term, StringComparison.OrdinalIgnoreCase);
    }

    private static string ExtractModelError(ModelStateDictionary modelState)
    {
        var first = modelState
            .Values
            .SelectMany(v => v.Errors)
            .Select(e => string.IsNullOrWhiteSpace(e.ErrorMessage) ? e.Exception?.Message : e.ErrorMessage)
            .FirstOrDefault(message => !string.IsNullOrWhiteSpace(message));

        return first ?? "ورودی نامعتبر است.";
    }

    private static (DashboardMenuModule? Module, DashboardMenuItem? Item) ResolveMenu(
        IReadOnlyList<DashboardMenuModule> modules,
        string moduleId,
        string itemId)
    {
        var module = modules.FirstOrDefault(m =>
            string.Equals(m.ModuleId, moduleId, StringComparison.OrdinalIgnoreCase));
        var item = module?.Items.FirstOrDefault(i =>
            string.Equals(i.ItemId, itemId, StringComparison.OrdinalIgnoreCase));
        return (module, item);
    }

    private static IEnumerable<CategoryNodeModel> FlattenCategories(IEnumerable<CategoryNodeModel> roots)
    {
        foreach (var root in roots)
        {
            yield return root;
            if (root.Children.Count == 0)
            {
                continue;
            }

            foreach (var child in FlattenCategories(root.Children))
            {
                yield return child;
            }
        }
    }

    private static string? JoinErrors(params string?[] errors)
    {
        var clean = errors
            .Where(message => !string.IsNullOrWhiteSpace(message))
            .Distinct()
            .ToList();
        return clean.Count == 0 ? null : string.Join(" | ", clean);
    }

    private sealed class ProductAutoVariantPayload
    {
        public List<ProductAutoAttributeItem> ProductAttributes { get; set; } = new();
        public List<ProductAutoVariantAttributeItem> VariantAttributes { get; set; } = new();
    }

    private sealed class ProductAutoAttributeItem
    {
        public string AttributeId { get; set; } = string.Empty;
        public string? Value { get; set; }
        public string? OptionId { get; set; }
    }

    private sealed class ProductAutoVariantAttributeItem
    {
        public string AttributeId { get; set; } = string.Empty;
        public List<string> OptionIds { get; set; } = new();
    }

    private sealed class VariantDimensionSelection
    {
        public string AttributeId { get; set; } = string.Empty;
        public string AttributeName { get; set; } = string.Empty;
        public int AttributeDisplayOrder { get; set; }
        public List<VariantDimensionOptionSelection> Options { get; set; } = new();
    }

    private sealed class VariantDimensionOptionSelection
    {
        public string AttributeId { get; set; } = string.Empty;
        public string OptionId { get; set; } = string.Empty;
        public string OptionName { get; set; } = string.Empty;
        public string OptionValue { get; set; } = string.Empty;
        public int OptionDisplayOrder { get; set; }
    }

    private sealed class VariantCombinationSelection
    {
        public string AttributeId { get; set; } = string.Empty;
        public string AttributeName { get; set; } = string.Empty;
        public int AttributeDisplayOrder { get; set; }
        public string OptionId { get; set; } = string.Empty;
        public string OptionName { get; set; } = string.Empty;
        public string OptionValue { get; set; } = string.Empty;
        public int OptionDisplayOrder { get; set; }
    }

    private sealed class GeneratedVariantPlan
    {
        public string GeneratedName { get; set; } = string.Empty;
        public string GeneratedSku { get; set; } = string.Empty;
        public List<CatalogAttributeValueInputModel> AttributeValues { get; set; } = new();
    }

    private void SetLayoutViewBag(
        IReadOnlyList<DashboardMenuModule> modules,
        string? activeModuleId,
        string? activeItemId,
        string userName)
    {
        ViewBag.UserDisplayName = userName;
        ViewBag.MenuModules = modules;
        ViewBag.ActiveModuleId = activeModuleId;
        ViewBag.ActiveItemId = activeItemId;
        ViewBag.CatalogPermissions = ResolvePermissionsFromSession();
    }

    protected bool TryGetToken(out string token)
    {
        token = HttpContext.Session.GetString("Token") ?? string.Empty;
        return !string.IsNullOrWhiteSpace(token);
    }

    private IReadOnlyList<string> ResolveRolesFromSession(string token)
    {
        var rolesJson = HttpContext.Session.GetString("Roles");
        if (!string.IsNullOrWhiteSpace(rolesJson))
        {
            try
            {
                var roles = JsonSerializer.Deserialize<List<string>>(rolesJson) ?? new List<string>();
                if (roles.Count > 0)
                {
                    return roles
                        .Where(role => !string.IsNullOrWhiteSpace(role))
                        .Select(role => role.Trim())
                        .Distinct(StringComparer.OrdinalIgnoreCase)
                        .ToList();
                }
            }
            catch
            {
                // Ignore malformed session value.
            }
        }

        var extractedRoles = JwtRoleExtractor.ExtractRoles(token)
            .Where(role => !string.IsNullOrWhiteSpace(role))
            .Select(role => role.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        HttpContext.Session.SetString("Roles", JsonSerializer.Serialize(extractedRoles));
        return extractedRoles;
    }

    private IReadOnlyList<string> ResolvePermissionsFromSession()
    {
        var permissionsJson = HttpContext.Session.GetString("Permissions");
        if (string.IsNullOrWhiteSpace(permissionsJson))
        {
            return Array.Empty<string>();
        }

        try
        {
            var permissions = JsonSerializer.Deserialize<List<string>>(permissionsJson) ?? new List<string>();
            return permissions
                .Where(permission => !string.IsNullOrWhiteSpace(permission))
                .Select(permission => permission.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();
        }
        catch
        {
            return Array.Empty<string>();
        }
    }
}

