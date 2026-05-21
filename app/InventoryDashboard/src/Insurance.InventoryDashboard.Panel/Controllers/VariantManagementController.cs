using System.Text.Json;
using Insurance.InventoryDashboard.Panel.Models;
using Insurance.InventoryDashboard.Panel.Services;
using Insurance.InventoryDashboard.Panel.Services.Localization;
using Microsoft.AspNetCore.Mvc;

namespace Insurance.InventoryDashboard.Panel.Controllers;

public sealed class VariantManagementController : CatalogManagementController
{
    private static readonly int[] PageSizeOptions = [10, 25, 50];
    private readonly IDashboardConfigService _dashboardConfigServiceLocal;
    private readonly IApiService _inventoryApiService;

    public VariantManagementController(
        IApiService inventoryApiService,
        ICatalogApiService apiService,
        IDashboardConfigService dashboardConfigService,
        IUiTextService uiTextService,
        ILogger<CatalogManagementController> logger)
        : base(apiService, dashboardConfigService, uiTextService, logger)
    {
        _dashboardConfigServiceLocal = dashboardConfigService;
        _inventoryApiService = inventoryApiService;
    }

    [HttpGet]
    public IActionResult Index() => RedirectToAction(nameof(Variants));

    [HttpGet]
    public new async Task<IActionResult> Variants(
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

        var permissions = ResolvePermissionsFromSession();
        var modules = await _dashboardConfigServiceLocal.GetMenuByRolesAsync(roles, cancellationToken);
        var menu = ResolveMenu(modules, "product_management", "product_variants");

        var categoriesResult = await _apiService.GetCategoryTreeAsync(token);
        var categories = categoriesResult.Data ?? new List<CategoryNodeModel>();
        var flatCategories = FlattenCategories(categories).ToList();
        var leafCategories = GetLeafCategories(flatCategories);
        var activeAttributesResult = await _apiService.GetActiveAttributeDefinitionsAsync(token);

        var model = new VariantManagementPageViewModel
        {
            UserName = HttpContext.Session.GetString("UserName") ?? "کاربر",
            Roles = roles,
            Permissions = permissions,
            Modules = modules,
            ActiveModule = menu.Module,
            ActiveItem = menu.Item,
            SelectedProductId = productId,
            SelectedVariantId = variantId,
            SelectedCategoryId = categoryId,
            Categories = categories,
            FlatCategories = flatCategories,
            LeafCategories = leafCategories,
            RelatedVariantLookup = new List<ProductVariantSummaryModel>(),
            ProductAttributeGroups = new List<CategoryAttributeGroupViewModel>(),
            EffectiveProductAttributes = (activeAttributesResult.Data ?? new List<AttributeDefinitionModel>())
                .Select(x => new EffectiveAttributeViewModel
                {
                    AttributeId = x.Id,
                    Name = x.Name,
                    DataType = x.DataType,
                    Scope = x.Scope,
                    IsVariantLevel = x.IsVariant,
                    IsVariantCodeCovered = x.IsVariantCodeCovered,
                    IsRequired = x.IsRequired,
                    DisplayOrder = x.DisplayOrder,
                    Options = x.Options ?? new List<AttributeOptionModel>(),
                    SourceCategoryId = x.SourceCategoryId ?? string.Empty,
                    SourceCategoryName = x.SourceCategoryName ?? string.Empty,
                    IsInherited = x.IsInherited
                })
                .ToList(),
            MissingRequiredVariantAttributes = new List<EffectiveAttributeViewModel>(),
            VariantSearchTerm = searchTerm,
            VariantCategoryFilterId = categoryId,
            VariantAttributeOptionFilterIds = attributeOptionIds,
            VariantTrackingFilter = trackingPolicy,
            VariantStatusFilter = statusFilter,
            SelectedAttributeTypeFilter = attributeTypeFilter,
            VariantSort = sort,
            VariantPage = page,
            VariantPageSize = pageSize,
            VariantTotalCount = 0,
            VariantTotalPages = 1,
            ErrorMessage = string.Join(" | ", new[]
            {
                categoriesResult.ErrorMessage,
                activeAttributesResult.ErrorMessage
            }.Where(x => !string.IsNullOrWhiteSpace(x))),
            VariantForm = new VariantUpsertForm(),
            VariantAttributeForm = new VariantAttributeValueForm(),
            VariantUomConversionForm = new VariantUomConversionForm(),
            VariantComponentForm = new VariantComponentForm(),
            VariantAddOnForm = new VariantAddOnForm(),
            VariantTagForm = new VariantTagForm(),
            TagDefinitionForm = new TagDefinitionForm(),
            VariantAssemblyOperationForm = new VariantAssemblyOperationForm(),
            BulkVariantAddOnForm = new BulkVariantAddOnForm(),
            BulkVariantImageForm = new BulkVariantImageForm(),
            BulkVariantTagForm = new BulkVariantTagForm()
        };

        SetLayoutViewBag(model.Modules, model.ActiveModule?.ModuleId, model.ActiveItem?.ItemId, model.UserName);
        return View("~/Views/CatalogManagement/Variants.cshtml", model);
    }

    [HttpGet]
    public async Task<IActionResult> VariantList(
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
            return PartialView("~/Views/Shared/_VariantListBody.cshtml", new VariantManagementPageViewModel
            {
                ErrorMessage = "برای مشاهده لیست واریانت‌ها باید دوباره وارد شوید."
            });
        }

        var roles = ResolveRolesFromSession(token);
        if (!IsAuthorizedFor(token, "Catalog.Variant.View", "ProductVariant.Read", "ProductVariant.Search"))
        {
            return PartialView("~/Views/Shared/_VariantListBody.cshtml", new VariantManagementPageViewModel
            {
                ErrorMessage = "شما دسترسی مشاهده لیست واریانت‌ها را ندارید."
            });
        }

        bool? isActiveFilter = TryParseStatusFilter(statusFilter, out var parsedActive) ? parsedActive : null;
        var variantsSearchResult = await _apiService.SearchProductVariantsAsync(
            token,
            searchTerm,
            productId,
            categoryId,
            attributeOptionIds,
            isActiveFilter,
            page,
            pageSize);

        var uomLookupResult = await _apiService.GetUnitOfMeasureLookupAsync(token);
        var uomById = (uomLookupResult.Data ?? new List<UnitOfMeasureLookupModel>())
            .ToDictionary(x => x.Id, x => x, StringComparer.OrdinalIgnoreCase);

        var variants = variantsSearchResult.Data?.Items ?? new List<ProductVariantSummaryModel>();
        foreach (var variant in variants)
        {
            if (uomById.TryGetValue(variant.BaseUomRef, out var uom))
            {
                variant.BaseUom = $"{uom.Code} - {uom.Name}";
            }
        }

        var normalizedPage = variantsSearchResult.Data?.Page ?? Math.Max(page, 1);
        var normalizedPageSize = variantsSearchResult.Data?.PageSize ?? (PageSizeOptions.Contains(pageSize) ? pageSize : PageSizeOptions[0]);
        var totalCount = variantsSearchResult.Data?.TotalCount ?? variants.Count;
        var totalPages = Math.Max(1, (int)Math.Ceiling(totalCount / (double)Math.Max(normalizedPageSize, 1)));

        var relatedVariantsResult = await _apiService.SearchProductVariantsAsync(token, isActive: true, pageSize: 2000);
        var relatedVariants = (relatedVariantsResult.Data?.Items ?? new List<ProductVariantSummaryModel>())
            .Where(x => !string.Equals(x.Id, variantId, StringComparison.OrdinalIgnoreCase))
            .OrderBy(x => x.Name)
            .ThenBy(x => x.Sku)
            .ToList();

        var model = new VariantManagementPageViewModel
        {
            UserName = HttpContext.Session.GetString("UserName") ?? "کاربر",
            Roles = roles,
            Permissions = ResolvePermissionsFromSession(),
            SelectedProductId = productId,
            SelectedVariantId = variantId,
            VariantSearchTerm = searchTerm,
            VariantCategoryFilterId = categoryId,
            VariantAttributeOptionFilterIds = attributeOptionIds,
            VariantTrackingFilter = trackingPolicy,
            VariantStatusFilter = statusFilter,
            SelectedAttributeTypeFilter = attributeTypeFilter,
            VariantSort = sort,
            VariantPage = normalizedPage,
            VariantPageSize = normalizedPageSize,
            VariantTotalCount = totalCount,
            VariantTotalPages = totalPages,
            Variants = variants,
            RelatedVariantLookup = relatedVariants,
            ErrorMessage = string.Join(" | ", new[]
            {
                variantsSearchResult.ErrorMessage,
                uomLookupResult.ErrorMessage,
                relatedVariantsResult.ErrorMessage
            }.Where(x => !string.IsNullOrWhiteSpace(x)))
        };

        return PartialView("~/Views/Shared/_VariantListBody.cshtml", model);
    }

    [HttpGet]
    public async Task<IActionResult> VariantComponents(
        string? productId,
        string? variantId,
        string? categoryId = null,
        string? searchTerm = null,
        string? attributeOptionIds = null,
        string? trackingPolicy = null,
        string? statusFilter = null,
        string? attributeTypeFilter = null,
        string? sort = null,
        bool createNew = false,
        int page = 1,
        int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        if (!TryGetToken(out var token))
        {
            return PartialView("~/Views/Shared/_VariantComponentsListBody.cshtml", new VariantManagementPageViewModel
            {
                SelectedProductId = productId,
                SelectedVariantId = variantId,
                ErrorMessage = "برای مشاهده کامپوننت‌های واریانت باید دوباره وارد شوید."
            });
        }

        var roles = ResolveRolesFromSession(token);
        if (!IsAuthorizedFor(token, "Catalog.Variant.View", "ProductVariant.Read", "ProductVariant.Search"))
        {
            return PartialView("~/Views/Shared/_VariantComponentsListBody.cshtml", new VariantManagementPageViewModel
            {
                SelectedProductId = productId,
                SelectedVariantId = variantId,
                ErrorMessage = "شما دسترسی مشاهده کامپوننت‌های واریانت را ندارید."
            });
        }

        var isValidVariantId = Guid.TryParse(variantId, out _);
        var componentsResult = isValidVariantId
            ? await _apiService.GetVariantComponentsByVariantIdAsync(variantId!, token)
            : new ApiResponse<List<VariantComponentModel>>
            {
                IsSuccess = false,
                ErrorMessage = _uiText["catalog.variants.images.invalidVariantId"],
                Data = new List<VariantComponentModel>()
            };

        var relatedVariantsResult = isValidVariantId
            ? await _apiService.SearchProductVariantsAsync(token, isActive: true, pageSize: 2000)
            : new ApiResponse<ProductVariantSearchResultModel>
            {
                IsSuccess = true,
                Data = new ProductVariantSearchResultModel
                {
                    Items = new List<ProductVariantSummaryModel>()
                }
            };
        var relatedVariants = (relatedVariantsResult.Data?.Items ?? new List<ProductVariantSummaryModel>())
            .Where(x => !string.Equals(x.Id, variantId, StringComparison.OrdinalIgnoreCase))
            .OrderBy(x => x.Name)
            .ThenBy(x => x.Sku)
            .ToList();

        var model = new VariantManagementPageViewModel
        {
            UserName = HttpContext.Session.GetString("UserName") ?? "کاربر",
            Roles = roles,
            Permissions = ResolvePermissionsFromSession(),
            SelectedProductId = productId,
            SelectedVariantId = variantId,
            VariantComponents = componentsResult.IsSuccess
                ? componentsResult.Data ?? new List<VariantComponentModel>()
                : new List<VariantComponentModel>(),
            RelatedVariantLookup = relatedVariants,
            ErrorMessage = string.Join(" | ", new[]
            {
                componentsResult.ErrorMessage,
                relatedVariantsResult.ErrorMessage
            }.Where(x => !string.IsNullOrWhiteSpace(x)))
        };

        return PartialView("~/Views/Shared/_VariantComponentsListBody.cshtml", model);
    }

    [HttpGet]
    public async Task<IActionResult> VariantStocks(string variantId)
    {
        if (!TryGetToken(out var token))
        {
            return Unauthorized();
        }

        if (!Guid.TryParse(variantId, out var variantRef))
        {
            return BadRequest(new { error = _uiText["catalog.variants.images.invalidVariantId"] });
        }

        var result = await _apiService.GetAvailableStockBucketsAsync(token, variantRef: variantRef);
        var warehouseLookupResult = await _apiService.GetWarehouseLookupAsync(token, includeInactive: true);
        var locationLookupResult = await _apiService.GetLocationLookupAsync(token, warehouseId: null, includeInactive: true);
        var qualityStatusLookupResult = await _apiService.GetQualityStatusLookupAsync(token, includeInactive: true);

        if (!result.IsSuccess || !warehouseLookupResult.IsSuccess || !locationLookupResult.IsSuccess || !qualityStatusLookupResult.IsSuccess)
        {
            var errorMessage = string.Join(" | ", new[]
            {
                result.ErrorMessage,
                warehouseLookupResult.ErrorMessage,
                locationLookupResult.ErrorMessage,
                qualityStatusLookupResult.ErrorMessage
            }.Where(x => !string.IsNullOrWhiteSpace(x)));

            return Json(new
            {
                error = errorMessage
            });
        }

        var warehouseLookup = (warehouseLookupResult.Data ?? new List<WarehouseLookupItemModel>())
            .ToDictionary(x => x.WarehouseBusinessKey, x => x, StringComparer.OrdinalIgnoreCase);
        var locationLookup = (locationLookupResult.Data ?? new List<LocationLookupItemModel>())
            .ToDictionary(x => x.LocationBusinessKey, x => x, StringComparer.OrdinalIgnoreCase);
        var qualityStatusLookup = (qualityStatusLookupResult.Data ?? new List<QualityStatusLookupItemModel>())
            .ToDictionary(x => x.QualityStatusBusinessKey, x => x, StringComparer.OrdinalIgnoreCase);

        var items = (result.Data?.Items ?? new List<StockDetailBucketModel>())
            .OrderBy(x => x.WarehouseRef)
            .ThenBy(x => x.LocationRef)
            .ThenBy(x => x.QualityStatusRef)
            .ThenBy(x => x.LotBatchNo)
            .Select(x =>
            {
                warehouseLookup.TryGetValue(x.WarehouseRef.ToString("D"), out var warehouse);
                locationLookup.TryGetValue(x.LocationRef.ToString("D"), out var location);
                qualityStatusLookup.TryGetValue(x.QualityStatusRef.ToString("D"), out var qualityStatus);

                var warehouseLabel = warehouse is null
                    ? x.WarehouseRef.ToString("D")
                    : $"{warehouse.Code} - {warehouse.Name}";

                var locationLabel = location is null
                    ? x.LocationRef.ToString("D")
                    : string.IsNullOrWhiteSpace(location.LocationType)
                        ? location.LocationCode
                        : $"{location.LocationCode} ({location.LocationType})";

                var qualityStatusLabel = qualityStatus is null
                    ? x.QualityStatusRef.ToString("D")
                    : $"{qualityStatus.Code} - {qualityStatus.Name}";

                return new
                {
                    stockDetailBusinessKey = x.StockDetailBusinessKey.ToString("D"),
                    variantRef = x.VariantRef.ToString("D"),
                    warehouseRef = x.WarehouseRef.ToString("D"),
                    warehouseCode = warehouse?.Code,
                    warehouseName = warehouse?.Name,
                    warehouseLabel,
                    locationRef = x.LocationRef.ToString("D"),
                    locationCode = location?.LocationCode,
                    locationType = location?.LocationType,
                    locationLabel,
                    qualityStatusRef = x.QualityStatusRef.ToString("D"),
                    qualityStatusCode = qualityStatus?.Code,
                    qualityStatusName = qualityStatus?.Name,
                    qualityStatusLabel,
                    lotBatchNo = x.LotBatchNo,
                    quantityOnHand = x.QuantityOnHand
                };
                })
            .ToList();

        var summary = items
            .GroupBy(x => x.warehouseRef, StringComparer.OrdinalIgnoreCase)
            .Select(group => new
            {
                warehouseRef = group.Key,
                warehouseCode = group.First().warehouseCode,
                warehouseName = group.First().warehouseName,
                warehouseLabel = group.First().warehouseLabel,
                quantityOnHand = group.Sum(x => x.quantityOnHand),
                bucketCount = group.Count(),
                locations = group
                    .Select(x => new
                    {
                        locationRef = x.locationRef,
                        locationCode = x.locationCode,
                        locationType = x.locationType,
                        locationLabel = x.locationLabel,
                        qualityStatusLabel = x.qualityStatusLabel,
                        lotBatchNo = x.lotBatchNo,
                        quantityOnHand = x.quantityOnHand
                    })
                    .OrderBy(x => x.locationLabel)
                    .ThenBy(x => x.qualityStatusLabel)
                    .ToList()
            })
            .OrderBy(x => x.warehouseLabel)
            .ToList();

        return Json(new
        {
            isSuccess = true,
            items = summary
        });
    }

    [HttpGet]
    public async Task<IActionResult> VariantPrices(string variantId)
    {
        if (!TryGetToken(out var token))
        {
            return Unauthorized();
        }

        if (!Guid.TryParse(variantId, out var variantRef))
        {
            return BadRequest(new { error = _uiText["catalog.variants.images.invalidVariantId"] });
        }

        var pricesTask = _inventoryApiService.SearchSellerVariantPricesAsync(token, variantRef: variantRef, pageSize: 100);
        var sellersTask = _inventoryApiService.GetSellerLookupAsync(token, includeInactive: true);
        var priceTypesTask = _inventoryApiService.GetPriceTypeLookupAsync(token, includeInactive: true);
        var priceChannelsTask = _inventoryApiService.GetPriceChannelLookupAsync(token, includeInactive: true);

        await Task.WhenAll(pricesTask, sellersTask, priceTypesTask, priceChannelsTask);

        if (!pricesTask.Result.IsSuccess || !sellersTask.Result.IsSuccess || !priceTypesTask.Result.IsSuccess || !priceChannelsTask.Result.IsSuccess)
        {
            return Json(new
            {
                isSuccess = false,
                error = string.Join(" | ", new[]
                {
                    pricesTask.Result.ErrorMessage,
                    sellersTask.Result.ErrorMessage,
                    priceTypesTask.Result.ErrorMessage,
                    priceChannelsTask.Result.ErrorMessage
                }.Where(x => !string.IsNullOrWhiteSpace(x)))
            });
        }

        var sellers = (sellersTask.Result.Data ?? new List<SellerLookupItemModel>())
            .ToDictionary(x => x.SellerBusinessKey, x => x);
        var priceTypes = (priceTypesTask.Result.Data?.Items ?? new List<PriceTypeLookupModel>())
            .ToDictionary(x => x.PriceTypeBusinessKey, x => x);
        var priceChannels = (priceChannelsTask.Result.Data?.Items ?? new List<PriceChannelLookupModel>())
            .ToDictionary(x => x.PriceChannelBusinessKey, x => x);

        var items = (pricesTask.Result.Data?.Items ?? new List<SellerVariantPriceModel>())
            .OrderBy(x => x.SellerRef)
            .ThenBy(x => x.PriceTypeRef)
            .ThenBy(x => x.PriceChannelRef)
            .ThenBy(x => x.Priority)
            .Select(x =>
            {
                sellers.TryGetValue(x.SellerRef.ToString("D"), out var seller);
                priceTypes.TryGetValue(x.PriceTypeRef, out var priceType);
                priceChannels.TryGetValue(x.PriceChannelRef, out var priceChannel);

                var sellerLabel = seller is null ? x.SellerRef.ToString("D") : $"{seller.Code} - {seller.Name}";
                var priceTypeLabel = priceType is null ? x.PriceTypeRef.ToString("D") : $"{priceType.Code} - {priceType.Name}";
                var priceChannelLabel = priceChannel is null ? x.PriceChannelRef.ToString("D") : $"{priceChannel.Code} - {priceChannel.Name}";

                return new
                {
                    sellerRef = x.SellerRef.ToString("D"),
                    sellerLabel,
                    priceTypeRef = x.PriceTypeRef.ToString("D"),
                    priceTypeLabel,
                    priceChannelRef = x.PriceChannelRef.ToString("D"),
                    priceChannelLabel,
                    amount = x.Amount,
                    currency = x.Currency,
                    minQty = x.MinQty,
                    priority = x.Priority,
                    effectiveFrom = x.EffectiveFrom,
                    effectiveTo = x.EffectiveTo,
                    isActive = x.IsActive
                };
            })
            .ToList();

        return Json(new
        {
            isSuccess = true,
            items
        });
    }

    [HttpGet]
    public async Task<IActionResult> VariantMediaLibrary(string variantId)
    {
        if (!TryGetToken(out var token))
        {
            return Unauthorized();
        }

        if (!Guid.TryParse(variantId, out _))
        {
            return BadRequest(_uiText["catalog.variants.images.invalidVariantId"]);
        }

        var variantResult = await _apiService.GetProductVariantFullDetailsAsync(variantId, token);
        if (!variantResult.IsSuccess || variantResult.Data is null)
        {
            return PartialView("_VariantMediaLibraryBody", new ProductVariantDetailsModel
            {
                Id = variantId,
                Images = new List<VariantImageModel>()
            });
        }

        return PartialView("_VariantMediaLibraryBody", variantResult.Data);
    }

    [HttpGet]
    public async Task<IActionResult> VariantAddOnsList(string variantId, string? productId = null)
    {
        if (!TryGetToken(out var token))
        {
            return PartialView("_VariantAddOnsListBody", new VariantAddOnsFragmentViewModel
            {
                SelectedProductId = productId,
                SelectedVariantId = variantId,
                ErrorMessage = "برای مشاهده Add-on ها باید دوباره وارد شوید."
            });
        }

        if (!Guid.TryParse(variantId, out _))
        {
            return PartialView("_VariantAddOnsListBody", new VariantAddOnsFragmentViewModel
            {
                SelectedProductId = productId,
                SelectedVariantId = variantId,
                ErrorMessage = _uiText["catalog.variants.images.invalidVariantId"]
            });
        }

        var relatedVariantsResult = await _apiService.SearchProductVariantsAsync(token, isActive: true, pageSize: 2000);
        var addOnsResult = await _apiService.GetVariantAddOnsByVariantIdAsync(variantId, token);

        var fragmentModel = new VariantAddOnsFragmentViewModel
        {
            SelectedProductId = productId,
            SelectedVariantId = variantId,
            RelatedVariantLookup = relatedVariantsResult.Data?.Items ?? new List<ProductVariantSummaryModel>(),
            Items = addOnsResult.IsSuccess
                ? addOnsResult.Data ?? new List<VariantAddOnModel>()
                : new List<VariantAddOnModel>(),
            ErrorMessage = string.Join(" | ", new[]
            {
                relatedVariantsResult.ErrorMessage,
                addOnsResult.ErrorMessage
            }.Where(x => !string.IsNullOrWhiteSpace(x)))
        };

        return PartialView("_VariantAddOnsListBody", fragmentModel);
    }

    [HttpGet]
    public async Task<IActionResult> VariantDocuments(string variantId)
    {
        if (!TryGetToken(out var token))
        {
            return Unauthorized();
        }

        var result = await _apiService.GetInventoryTransactionsByVariantAsync(variantId, token);
        if (!result.IsSuccess)
        {
            return Json(new { error = result.ErrorMessage });
        }

        return Json(result.Data ?? new List<InventoryTransactionListItemModel>());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public new Task<IActionResult> SaveVariant([Bind(Prefix = "VariantForm")] VariantUpsertForm form)
        => base.SaveVariant(form);

    [HttpPost]
    [ValidateAntiForgeryToken]
    public new Task<IActionResult> ActivateVariant(string productId, string variantId)
        => base.ActivateVariant(productId, variantId);

    [HttpPost]
    [ValidateAntiForgeryToken]
    public new Task<IActionResult> DeactivateVariant(string productId, string variantId)
        => base.DeactivateVariant(productId, variantId);

    [HttpPost]
    [ValidateAntiForgeryToken]
    public new Task<IActionResult> DeleteVariant(string productId, string variantId)
        => base.DeleteVariant(productId, variantId);

    [HttpPost]
    [ValidateAntiForgeryToken]
    public new Task<IActionResult> ApplyVariantBulkAction(
        string selectedIds,
        string bulkAction,
        string? productId,
        string? searchTerm,
        string? trackingPolicy,
        string? statusFilter,
        string? attributeTypeFilter,
        string? sort,
        int page = 1,
        int pageSize = 10)
        => base.ApplyVariantBulkAction(selectedIds, bulkAction, productId, searchTerm, trackingPolicy, statusFilter, attributeTypeFilter, sort, page, pageSize);

    [HttpPost]
    [ValidateAntiForgeryToken]
    public new Task<IActionResult> LockVariantInventoryMovement(string productId, string variantId)
        => base.LockVariantInventoryMovement(productId, variantId);

    [HttpPost]
    [ValidateAntiForgeryToken]
    public new Task<IActionResult> ChangeVariantTrackingPolicy(string productId, string variantId, string trackingPolicy)
        => base.ChangeVariantTrackingPolicy(productId, variantId, trackingPolicy);

    [HttpPost]
    [ValidateAntiForgeryToken]
    public new Task<IActionResult> ChangeVariantBaseUom(string productId, string variantId, string baseUomRef)
        => base.ChangeVariantBaseUom(productId, variantId, baseUomRef);

    [HttpPost]
    [ValidateAntiForgeryToken]
    public new Task<IActionResult> SetVariantAttributeValue([Bind(Prefix = "VariantAttributeForm")] VariantAttributeValueForm form)
        => base.SetVariantAttributeValue(form);

    [HttpPost]
    [ValidateAntiForgeryToken]
    public new Task<IActionResult> RemoveVariantAttributeValue(string productId, string variantId, string attributeId)
        => base.RemoveVariantAttributeValue(productId, variantId, attributeId);

    [HttpPost]
    [ValidateAntiForgeryToken]
    public new Task<IActionResult> UpsertVariantUomConversion([Bind(Prefix = "VariantUomConversionForm")] VariantUomConversionForm form)
        => base.UpsertVariantUomConversion(form);

    [HttpPost]
    [ValidateAntiForgeryToken]
    public new Task<IActionResult> RemoveVariantUomConversion(string productId, string variantId, string fromUomRef, string toUomRef)
        => base.RemoveVariantUomConversion(productId, variantId, fromUomRef, toUomRef);

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpsertVariantComponent([Bind(Prefix = "VariantComponentForm")] VariantComponentForm form)
    {
        if (!TryGetToken(out var token))
        {
            return RedirectToAction("Login", "Auth");
        }

        if (!TryValidateModel(form))
        {
            TempData["CatalogError"] = ExtractModelError(ModelState);
            return RedirectToAction(nameof(Variants), new { productId = form.ProductId, variantId = form.VariantId });
        }

        var result = await _apiService.UpsertVariantComponentAsync(
            form.VariantId,
            new UpsertVariantComponentRequest
            {
                ComponentId = form.ComponentId,
                ComponentVariantRef = form.ComponentVariantId,
                WarehouseRef = form.WarehouseId,
                LocationRef = form.LocationId,
                Quantity = form.Quantity
            },
            token);

        if (!result.IsSuccess)
        {
            TempData["CatalogError"] = result.ErrorMessage ?? "Ã˜Â«Ã˜Â¨Ã˜Âª Ã˜Â¬Ã˜Â²Ã˜Â¡ Ã™Ë†Ã˜Â§Ã˜Â±Ã›Å’Ã˜Â§Ã™â€ Ã˜Âª Ã˜Â§Ã™â€ Ã˜Â¬Ã˜Â§Ã™â€¦ Ã™â€ Ã˜Â´Ã˜Â¯.";
        }
        else
        {
            TempData["CatalogSuccess"] = "Ã˜Â¬Ã˜Â²Ã˜Â¡/Ã™â€¦Ã˜Â¹Ã˜Â§Ã˜Â¯Ã™â€ž Ã™Ë†Ã˜Â§Ã˜Â±Ã›Å’Ã˜Â§Ã™â€ Ã˜Âª Ã˜Â¨Ã˜Â§ Ã™â€¦Ã™Ë†Ã™ÂÃ™â€šÃ›Å’Ã˜Âª Ã˜Â°Ã˜Â®Ã›Å’Ã˜Â±Ã™â€¡ Ã˜Â´Ã˜Â¯.";
        }

        return RedirectToAction(nameof(Variants), new { productId = form.ProductId, variantId = form.VariantId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RemoveVariantComponent(string productId, string variantId, string componentVariantId, string? componentId)
    {
        if (!TryGetToken(out var token))
        {
            return RedirectToAction("Login", "Auth");
        }

        var result = await _apiService.RemoveVariantComponentAsync(variantId, componentVariantId, componentId, token);
        if (!result.IsSuccess)
        {
            TempData["CatalogError"] = result.ErrorMessage ?? "Ã˜Â­Ã˜Â°Ã™Â Ã˜Â¬Ã˜Â²Ã˜Â¡ Ã™Ë†Ã˜Â§Ã˜Â±Ã›Å’Ã˜Â§Ã™â€ Ã˜Âª Ã˜Â§Ã™â€ Ã˜Â¬Ã˜Â§Ã™â€¦ Ã™â€ Ã˜Â´Ã˜Â¯.";
        }

        return RedirectToAction(nameof(Variants), new { productId, variantId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpsertVariantAddOn([Bind(Prefix = "VariantAddOnForm")] VariantAddOnForm form)
    {
        if (!TryGetToken(out var token))
        {
            return RedirectToAction("Login", "Auth");
        }

        if (!TryValidateModel(form))
        {
            TempData["CatalogError"] = ExtractModelError(ModelState);
            return RedirectToAction(nameof(Variants), new { productId = form.ProductId, variantId = form.VariantId });
        }

        var result = await _apiService.UpsertVariantAddOnAsync(
            form.VariantId,
            new UpsertVariantAddOnRequest
            {
                AddOnVariantRef = form.AddOnVariantId
            },
            token);

        if (!result.IsSuccess)
        {
            TempData["CatalogError"] = result.ErrorMessage ?? "Ã˜Â«Ã˜Â¨Ã˜Âª Add-on Ã™Ë†Ã˜Â§Ã˜Â±Ã›Å’Ã˜Â§Ã™â€ Ã˜Âª Ã˜Â§Ã™â€ Ã˜Â¬Ã˜Â§Ã™â€¦ Ã™â€ Ã˜Â´Ã˜Â¯.";
        }
        else
        {
            TempData["CatalogSuccess"] = "Add-on Ã™Ë†Ã˜Â§Ã˜Â±Ã›Å’Ã˜Â§Ã™â€ Ã˜Âª Ã˜Â¨Ã˜Â§ Ã™â€¦Ã™Ë†Ã™ÂÃ™â€šÃ›Å’Ã˜Âª Ã˜Â°Ã˜Â®Ã›Å’Ã˜Â±Ã™â€¡ Ã˜Â´Ã˜Â¯.";
        }

        return RedirectToAction(nameof(Variants), new { productId = form.ProductId, variantId = form.VariantId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RemoveVariantAddOn(string productId, string variantId, string addOnVariantId)
    {
        var isAjaxRequest = string.Equals(Request.Headers["X-Requested-With"], "XMLHttpRequest", StringComparison.OrdinalIgnoreCase) ||
                            Request.Headers.Accept.Any(x => x.Contains("application/json", StringComparison.OrdinalIgnoreCase));

        if (!TryGetToken(out var token))
        {
            if (isAjaxRequest)
            {
                return Unauthorized(new { isSuccess = false, error = "Authentication required." });
            }

            return RedirectToAction("Login", "Auth");
        }

        if (string.IsNullOrWhiteSpace(variantId) || string.IsNullOrWhiteSpace(addOnVariantId))
        {
            var errorMessage = "شناسه واریانت یا ادزآن معتبر نیست.";
            if (isAjaxRequest)
            {
                return BadRequest(new { isSuccess = false, error = errorMessage });
            }

            TempData["CatalogError"] = errorMessage;
            return RedirectToAction(nameof(Variants), new { productId, variantId });
        }

        var result = await _apiService.RemoveVariantAddOnAsync(variantId, addOnVariantId, token);
        if (!result.IsSuccess)
        {
            if (isAjaxRequest)
            {
                return BadRequest(new { isSuccess = false, error = result.ErrorMessage ?? "حذف Add-on انجام نشد." });
            }

            TempData["CatalogError"] = result.ErrorMessage ?? "Ã˜Â­Ã˜Â°Ã™Â Add-on Ã™Ë†Ã˜Â§Ã˜Â±Ã›Å’Ã˜Â§Ã™â€ Ã˜Âª Ã˜Â§Ã™â€ Ã˜Â¬Ã˜Â§Ã™â€¦ Ã™â€ Ã˜Â´Ã˜Â¯.";
        }
        else if (isAjaxRequest)
        {
            return Json(new
            {
                isSuccess = true,
                message = "Add-on با موفقیت حذف شد.",
                productId,
                variantId,
                addOnVariantId
            });
        }

        return RedirectToAction(nameof(Variants), new { productId, variantId });
    }

    [HttpGet]
    public async Task<IActionResult> VariantTags(string variantId)
    {
        if (!TryGetToken(out var token))
        {
            return Unauthorized();
        }

        var result = await _apiService.GetVariantTagsByVariantIdAsync(variantId, token);
        if (!result.IsSuccess)
        {
            return Json(new { error = result.ErrorMessage });
        }

        return Json(result.Data ?? new List<VariantTagModel>());
    }

    [HttpGet]
    public async Task<IActionResult> VariantTagLookup(string? term = null, int take = 50)
    {
        if (!TryGetToken(out var token))
        {
            return Unauthorized();
        }

        var result = await _apiService.GetVariantTagLookupAsync(token, term, take);
        if (!result.IsSuccess)
        {
            return Json(new { error = result.ErrorMessage });
        }

        return Json((result.Data ?? new List<VariantTagLookupModel>()).Select(x => new
        {
            id = x.TagId,
            text = string.IsNullOrWhiteSpace(x.TagColor)
                ? x.TagName
                : $"{x.TagName} ({x.TagColor})",
            tagId = x.TagId,
            tagName = x.TagName,
            tagColor = x.TagColor,
            usageCount = x.UsageCount
        }));
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
        foreach (var root in roots
                     .OrderBy(x => x.DisplayOrder)
                     .ThenBy(x => x.Name)
                     .ThenBy(x => x.Id))
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

    private static IReadOnlyList<CategoryNodeModel> GetLeafCategories(IReadOnlyList<CategoryNodeModel> flatCategories)
        => flatCategories.Where(x => x.Children.Count == 0).ToList();

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

    [HttpGet]
    public async Task<IActionResult> SearchVariantComponentLookup(string? term, CancellationToken cancellationToken = default)
        => await base.SearchVariantComponentLookup(term, cancellationToken);

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpsertVariantTag([Bind(Prefix = "VariantTagForm")] VariantTagForm form)
    {
        var isAjaxRequest = string.Equals(Request.Headers["X-Requested-With"], "XMLHttpRequest", StringComparison.OrdinalIgnoreCase) ||
                            Request.Headers.Accept.Any(x => x.Contains("application/json", StringComparison.OrdinalIgnoreCase));

        if (!TryGetToken(out var token))
        {
            if (isAjaxRequest)
            {
                return Unauthorized(new { isSuccess = false, error = "Authentication required." });
            }

            return RedirectToAction("Login", "Auth");
        }

        if (!TryValidateModel(form))
        {
            var errorMessage = ExtractModelError(ModelState);
            if (isAjaxRequest)
            {
                return BadRequest(new { isSuccess = false, error = errorMessage });
            }

            TempData["CatalogError"] = errorMessage;
            return RedirectToAction(nameof(Variants), new { productId = form.ProductId, variantId = form.VariantId });
        }

        var result = await _apiService.UpsertVariantTagAsync(
            form.VariantId,
            new UpsertVariantTagRequest
            {
                TagId = form.TagId,
                DisplayOrder = form.DisplayOrder
            },
            token);

        if (!result.IsSuccess)
        {
            if (isAjaxRequest)
            {
                return BadRequest(new { isSuccess = false, error = result.ErrorMessage ?? "ثبت برچسب انجام نشد." });
            }

            TempData["CatalogError"] = result.ErrorMessage ?? "ثبت برچسب انجام نشد.";
        }
        else
        {
            if (isAjaxRequest)
            {
                return Json(new
                {
                    isSuccess = true,
                    message = "برچسب با موفقیت ذخیره شد.",
                    variantId = form.VariantId,
                    productId = form.ProductId,
                    tagId = form.TagId
                });
            }

            TempData["CatalogSuccess"] = "برچسب با موفقیت ذخیره شد.";
        }

        return RedirectToAction(nameof(Variants), new { productId = form.ProductId, variantId = form.VariantId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateTagDefinition([Bind(Prefix = "TagDefinitionForm")] TagDefinitionForm form)
    {
        var isAjaxRequest = string.Equals(Request.Headers["X-Requested-With"], "XMLHttpRequest", StringComparison.OrdinalIgnoreCase) ||
                            Request.Headers.Accept.Any(x => x.Contains("application/json", StringComparison.OrdinalIgnoreCase));

        if (!TryGetToken(out var token))
        {
            if (isAjaxRequest)
            {
                return Unauthorized(new { isSuccess = false, error = "Authentication required." });
            }

            return RedirectToAction("Login", "Auth");
        }

        if (!TryValidateModel(form))
        {
            var errorMessage = ExtractModelError(ModelState);
            if (isAjaxRequest)
            {
                return BadRequest(new { isSuccess = false, error = errorMessage });
            }

            TempData["CatalogError"] = errorMessage;
            return RedirectToAction(nameof(Variants), new { productId = string.Empty, variantId = string.Empty });
        }

        var result = await _inventoryApiService.CreateTagDefinitionAsync(new CreateTagDefinitionRequest
        {
            TagName = form.TagName,
            TagColor = form.TagColor
        }, token);

        if (!result.IsSuccess)
        {
            if (isAjaxRequest)
            {
                return BadRequest(new { isSuccess = false, error = result.ErrorMessage ?? "ثبت برچسب انجام نشد." });
            }

            TempData["CatalogError"] = result.ErrorMessage ?? "ثبت برچسب انجام نشد.";
        }
        else
        {
            if (isAjaxRequest)
            {
                return Json(new
                {
                    isSuccess = true,
                    message = "برچسب با موفقیت ایجاد شد.",
                    tagId = result.Data?.TagId,
                    tagName = result.Data?.TagName,
                    tagColor = result.Data?.TagColor
                });
            }

            TempData["CatalogSuccess"] = "برچسب با موفقیت ایجاد شد.";
        }

        return RedirectToAction(nameof(Variants), new { productId = string.Empty, variantId = string.Empty });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RemoveVariantTag(string productId, string variantId, string? variantTagBusinessKey, string? tagId, string? tagName)
    {
        var isAjaxRequest = string.Equals(Request.Headers["X-Requested-With"], "XMLHttpRequest", StringComparison.OrdinalIgnoreCase) ||
                            Request.Headers.Accept.Any(x => x.Contains("application/json", StringComparison.OrdinalIgnoreCase));

        if (!TryGetToken(out var token))
        {
            if (isAjaxRequest)
            {
                return Unauthorized(new { isSuccess = false, error = "Authentication required." });
            }

            return RedirectToAction("Login", "Auth");
        }

        var resolvedVariantTagBusinessKey = !string.IsNullOrWhiteSpace(variantTagBusinessKey)
            ? variantTagBusinessKey
            : tagId;

        var result = await _apiService.RemoveVariantTagAsync(variantId, resolvedVariantTagBusinessKey, tagName, token);
        if (!result.IsSuccess)
        {
            if (isAjaxRequest)
            {
                return BadRequest(new { isSuccess = false, error = result.ErrorMessage ?? "حذف برچسب انجام نشد." });
            }

            TempData["CatalogError"] = result.ErrorMessage ?? "حذف برچسب انجام نشد.";
        }
        else
        {
            if (isAjaxRequest)
            {
                return Json(new
                {
                    isSuccess = true,
                    message = "برچسب با موفقیت حذف شد.",
                    variantId,
                    productId,
                    variantTagBusinessKey = resolvedVariantTagBusinessKey,
                    tagName
                });
            }

            TempData["CatalogSuccess"] = "برچسب با موفقیت حذف شد.";
        }

        return RedirectToAction(nameof(Variants), new { productId, variantId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> BulkAssignVariantTags([Bind(Prefix = "BulkVariantTagForm")] BulkVariantTagForm form)
    {
        var isAjaxRequest = string.Equals(Request.Headers["X-Requested-With"], "XMLHttpRequest", StringComparison.OrdinalIgnoreCase) ||
                            Request.Headers.Accept.Any(x => x.Contains("application/json", StringComparison.OrdinalIgnoreCase));

        if (!TryGetToken(out var token))
        {
            if (isAjaxRequest)
            {
                return Unauthorized(new { isSuccess = false, error = "Authentication required." });
            }

            return RedirectToAction("Login", "Auth");
        }

        if (!TryValidateModel(form))
        {
            var errorMessage = ExtractModelError(ModelState);
            if (isAjaxRequest)
            {
                return BadRequest(new { isSuccess = false, error = errorMessage });
            }

            TempData["CatalogError"] = errorMessage;
            return RedirectToAction(nameof(Variants), new { productId = form.ProductId });
        }

        var selectedVariantIds = (form.SelectedVariantIds ?? string.Empty)
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        var tags = (form.TagIds ?? string.Empty)
            .Split(new[] { ',', ';', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        if (selectedVariantIds.Count == 0)
        {
            var errorMessage = "حداقل یک واریانت را برای ثبت برچسب انتخاب کنید.";
            if (isAjaxRequest)
            {
                return BadRequest(new { isSuccess = false, error = errorMessage });
            }

            TempData["CatalogError"] = errorMessage;
            return RedirectToAction(nameof(Variants), new { productId = form.ProductId });
        }

        if (tags.Count == 0)
        {
            var errorMessage = "حداقل یک برچسب وارد کنید.";
            if (isAjaxRequest)
            {
                return BadRequest(new { isSuccess = false, error = errorMessage });
            }

            TempData["CatalogError"] = errorMessage;
            return RedirectToAction(nameof(Variants), new { productId = form.ProductId });
        }

        var successCount = 0;
        var failures = new List<string>();
        foreach (var variantId in selectedVariantIds)
        {
            var failed = false;
            var order = 0;
            foreach (var tag in tags)
            {
                var result = await _apiService.UpsertVariantTagAsync(
                    variantId,
                    new UpsertVariantTagRequest
                    {
                        TagId = tag,
                        DisplayOrder = order++
                    },
                    token);

                if (!result.IsSuccess)
                {
                    failures.Add(result.ErrorMessage ?? $"ثبت برچسب برای واریانت {variantId} انجام نشد.");
                    failed = true;
                    break;
                }
            }

            if (!failed)
            {
                successCount++;
            }
        }

        if (successCount > 0)
        {
            if (isAjaxRequest)
            {
                return Json(new
                {
                    isSuccess = true,
                    message = $"برچسب برای {successCount} واریانت ثبت شد.",
                    productId = form.ProductId,
                    variantId = selectedVariantIds.FirstOrDefault(),
                    successCount
                });
            }

            TempData["CatalogSuccess"] = $"برچسب برای {successCount} واریانت ثبت شد.";
        }

        if (failures.Count > 0)
        {
            if (isAjaxRequest)
            {
                return BadRequest(new
                {
                    isSuccess = false,
                    error = string.Join(" | ", failures.Take(3)) + (failures.Count > 3 ? " | ..." : string.Empty)
                });
            }

            TempData["CatalogError"] = string.Join(" | ", failures.Take(3)) + (failures.Count > 3 ? " | ..." : string.Empty);
        }

        return RedirectToAction(nameof(Variants), new { productId = form.ProductId, variantId = selectedVariantIds.FirstOrDefault() });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ExecuteVariantAssembly([Bind(Prefix = "VariantAssemblyOperationForm")] VariantAssemblyOperationForm form)
    {
        if (!TryGetToken(out var token))
        {
            return RedirectToAction("Login", "Auth");
        }

        if (!IsAuthorizedFor(
                token,
                "Inventory.Document.Create",
                "InventoryDocument.Create",
                "Document.Create",
                "Inventory.Document.Approve",
                "InventoryDocument.Approve",
                "Document.Approve",
                "Inventory.Document.Post",
                "InventoryDocument.Post",
                "Document.Post"))
        {
            TempData["CatalogError"] = "Ã˜Â¯Ã˜Â³Ã˜ÂªÃ˜Â±Ã˜Â³Ã›Å’ Ã™â€žÃ˜Â§Ã˜Â²Ã™â€¦ Ã˜Â¨Ã˜Â±Ã˜Â§Ã›Å’ Ã˜Â«Ã˜Â¨Ã˜Âª Ã™Ë† Ã™Â¾Ã˜Â³Ã˜Âª Ã˜Â®Ã™Ë†Ã˜Â¯ÃšÂ©Ã˜Â§Ã˜Â± Ã˜Â³Ã™â€ Ã˜Â¯ Ã˜ÂªÃ˜Â¨Ã˜Â¯Ã›Å’Ã™â€ž Ã˜Â±Ã˜Â§ Ã™â€ Ã˜Â¯Ã˜Â§Ã˜Â±Ã›Å’Ã˜Â¯.";
            return RedirectToAction(nameof(Variants), new { productId = form.ProductId, variantId = form.VariantId });
        }

        if (!TryValidateModel(form))
        {
            TempData["CatalogError"] = ExtractModelError(ModelState);
            return RedirectToAction(nameof(Variants), new { productId = form.ProductId, variantId = form.VariantId });
        }

        var detailsResult = await _apiService.GetProductVariantFullDetailsAsync(form.VariantId, token);
        var details = detailsResult.Data;
        if (!detailsResult.IsSuccess || details is null)
        {
            TempData["CatalogError"] = detailsResult.ErrorMessage ?? "Ã˜Â¬Ã˜Â²Ã˜Â¦Ã›Å’Ã˜Â§Ã˜Âª Ã™Ë†Ã˜Â§Ã˜Â±Ã›Å’Ã˜Â§Ã™â€ Ã˜Âª Ã˜Â¨Ã˜Â±Ã˜Â§Ã›Å’ Ã˜Â¹Ã™â€¦Ã™â€žÃ›Å’Ã˜Â§Ã˜Âª Ã˜ÂªÃ˜Â¨Ã˜Â¯Ã›Å’Ã™â€ž Ã˜Â¨Ã˜Â§Ã˜Â±ÃšÂ¯Ã˜Â°Ã˜Â§Ã˜Â±Ã›Å’ Ã™â€ Ã˜Â´Ã˜Â¯.";
            return RedirectToAction(nameof(Variants), new { productId = form.ProductId, variantId = form.VariantId });
        }

        var recipe = details.Components
            .Where(x =>
                !string.IsNullOrWhiteSpace(x.ComponentVariantId) &&
                !string.IsNullOrWhiteSpace(x.WarehouseId) &&
                !string.IsNullOrWhiteSpace(x.LocationId) &&
                x.Quantity > 0)
            .ToList();

        if (recipe.Count == 0)
        {
            TempData["CatalogError"] = "Ã˜Â¨Ã˜Â±Ã˜Â§Ã›Å’ Ã˜Â§Ã›Å’Ã™â€  Ã™Ë†Ã˜Â§Ã˜Â±Ã›Å’Ã˜Â§Ã™â€ Ã˜Âª recipe Ã›Å’Ã˜Â§ Ã™â€žÃ›Å’Ã˜Â³Ã˜Âª Ã˜Â§Ã˜Â¬Ã˜Â²Ã˜Â§Ã›Å’ Ã˜Â³Ã˜Â§Ã˜Â²Ã™â€ Ã˜Â¯Ã™â€¡ Ã˜Â«Ã˜Â¨Ã˜Âª Ã™â€ Ã˜Â´Ã˜Â¯Ã™â€¡ Ã˜Â§Ã˜Â³Ã˜Âª.";
            return RedirectToAction(nameof(Variants), new { productId = form.ProductId, variantId = form.VariantId });
        }

        var operationIsAssemble = string.Equals(form.OperationType, "Assemble", StringComparison.OrdinalIgnoreCase);
        var contextResult = operationIsAssemble
            ? await BuildAssembleDocumentAsync(token, details, recipe, form.Quantity, form.ReasonCode)
            : await BuildDisassembleDocumentAsync(token, details, recipe, form.Quantity, form.ReasonCode);

        if (!contextResult.Success || contextResult.Form is null)
        {
            TempData["CatalogError"] = contextResult.Error ?? "Ã˜Â§Ã™â€¦ÃšÂ©Ã˜Â§Ã™â€  Ã˜Â¢Ã™â€¦Ã˜Â§Ã˜Â¯Ã™â€¡Ã¢â‚¬Å’Ã˜Â³Ã˜Â§Ã˜Â²Ã›Å’ Ã˜Â³Ã™â€ Ã˜Â¯ Ã˜ÂªÃ˜Â¨Ã˜Â¯Ã›Å’Ã™â€ž Ã™Ë†Ã˜Â¬Ã™Ë†Ã˜Â¯ Ã™â€ Ã˜Â¯Ã˜Â§Ã˜Â´Ã˜Âª.";
            return RedirectToAction(nameof(Variants), new { productId = form.ProductId, variantId = form.VariantId });
        }

        var createResult = await _inventoryApiService.CreateInventoryDocumentAsync(contextResult.Form, token);
        if (!createResult.IsSuccess || createResult.Data is null)
        {
            TempData["CatalogError"] = createResult.ErrorMessage ?? "Ã˜Â§Ã›Å’Ã˜Â¬Ã˜Â§Ã˜Â¯ Ã˜Â³Ã™â€ Ã˜Â¯ Ã˜ÂªÃ˜Â¨Ã˜Â¯Ã›Å’Ã™â€ž Ã˜Â§Ã™â€ Ã˜Â¬Ã˜Â§Ã™â€¦ Ã™â€ Ã˜Â´Ã˜Â¯.";
            return RedirectToAction(nameof(Variants), new { productId = form.ProductId, variantId = form.VariantId });
        }

        var approvedBy = HttpContext.Session.GetString("UserName") ?? "dashboard";
        var approveResult = await _inventoryApiService.ApproveInventoryDocumentAsync(createResult.Data.DocumentId, approvedBy, token);
        if (!approveResult.IsSuccess)
        {
            TempData["CatalogError"] = approveResult.ErrorMessage ?? "Ã˜Â³Ã™â€ Ã˜Â¯ Ã˜ÂªÃ˜Â¨Ã˜Â¯Ã›Å’Ã™â€ž Ã˜Â§Ã›Å’Ã˜Â¬Ã˜Â§Ã˜Â¯ Ã˜Â´Ã˜Â¯ Ã˜Â§Ã™â€¦Ã˜Â§ Ã˜ÂªÃ˜Â§Ã›Å’Ã›Å’Ã˜Â¯ Ã˜Â®Ã™Ë†Ã˜Â¯ÃšÂ©Ã˜Â§Ã˜Â± Ã˜Â¢Ã™â€  Ã˜Â§Ã™â€ Ã˜Â¬Ã˜Â§Ã™â€¦ Ã™â€ Ã˜Â´Ã˜Â¯.";
            return RedirectToAction(nameof(Variants), new { productId = form.ProductId, variantId = form.VariantId });
        }

        var postResult = await _inventoryApiService.PostInventoryDocumentAsync(createResult.Data.DocumentId, approvedBy, token);
        if (!postResult.IsSuccess)
        {
            TempData["CatalogError"] = postResult.ErrorMessage ?? "Ã˜Â³Ã™â€ Ã˜Â¯ Ã˜ÂªÃ˜Â¨Ã˜Â¯Ã›Å’Ã™â€ž Ã˜ÂªÃ˜Â§Ã›Å’Ã›Å’Ã˜Â¯ Ã˜Â´Ã˜Â¯ Ã˜Â§Ã™â€¦Ã˜Â§ Ã™Â¾Ã˜Â³Ã˜Âª Ã˜Â®Ã™Ë†Ã˜Â¯ÃšÂ©Ã˜Â§Ã˜Â± Ã˜Â¢Ã™â€  Ã˜Â§Ã™â€ Ã˜Â¬Ã˜Â§Ã™â€¦ Ã™â€ Ã˜Â´Ã˜Â¯.";
            return RedirectToAction(nameof(Variants), new { productId = form.ProductId, variantId = form.VariantId });
        }

        TempData["CatalogSuccess"] = $"{(operationIsAssemble ? "Ã™â€¦Ã™Ë†Ã™â€ Ã˜ÂªÃ˜Â§ÃšËœ" : "Ã˜Â¯Ã›Å’Ã¢â‚¬Å’Ã˜Â§Ã˜Â³Ã™â€¦Ã˜Â¨Ã™â€ž")} Ã˜Â¨Ã˜Â§ Ã™â€¦Ã™Ë†Ã™ÂÃ™â€šÃ›Å’Ã˜Âª Ã˜Â§Ã™â€ Ã˜Â¬Ã˜Â§Ã™â€¦ Ã˜Â´Ã˜Â¯ Ã™Ë† Ã˜Â³Ã™â€ Ã˜Â¯ {createResult.Data.DocumentNo} Ã˜Â«Ã˜Â¨Ã˜Âª Ã˜Â´Ã˜Â¯.";
        return RedirectToAction(nameof(Variants), new { productId = form.ProductId, variantId = form.VariantId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> BulkAssignVariantAddOn([Bind(Prefix = "BulkVariantAddOnForm")] BulkVariantAddOnForm form)
    {
        var isAjaxRequest = string.Equals(Request.Headers["X-Requested-With"], "XMLHttpRequest", StringComparison.OrdinalIgnoreCase) ||
                            Request.Headers.Accept.Any(x => x.Contains("application/json", StringComparison.OrdinalIgnoreCase));

        if (!TryGetToken(out var token))
        {
            if (isAjaxRequest)
            {
                return Unauthorized(new { isSuccess = false, error = "Authentication required." });
            }

            return RedirectToAction("Login", "Auth");
        }

        if (!TryValidateModel(form))
        {
            var errorMessage = ExtractModelError(ModelState);
            if (isAjaxRequest)
            {
                return Json(new { isSuccess = false, error = errorMessage });
            }

            TempData["CatalogError"] = errorMessage;
            return RedirectToAction(nameof(Variants), new { productId = form.ProductId });
        }

        var selectedVariantIds = (form.SelectedVariantIds ?? string.Empty)
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        var selectedAddOnIds = (form.SelectedAddOnVariantIds ?? string.Empty)
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        if (selectedAddOnIds.Count == 0 && !string.IsNullOrWhiteSpace(form.AddOnVariantId))
            selectedAddOnIds.Add(form.AddOnVariantId);

        if (selectedVariantIds.Count == 0)
        {
            var errorMessage = "حداقل یک واریانت را برای ثبت Add-on انتخاب کنید.";
            if (isAjaxRequest)
            {
                return Json(new { isSuccess = false, error = errorMessage });
            }

            TempData["CatalogError"] = errorMessage;
            return RedirectToAction(nameof(Variants), new { productId = form.ProductId });
        }

        if (selectedAddOnIds.Count == 0)
        {
            var errorMessage = "حداقل یک Add-on را برای انتساب انتخاب کنید.";
            if (isAjaxRequest)
            {
                return Json(new { isSuccess = false, error = errorMessage });
            }

            TempData["CatalogError"] = errorMessage;
            return RedirectToAction(nameof(Variants), new { productId = form.ProductId });
        }

        var failures = new List<string>();
        var successCount = 0;

        foreach (var variantId in selectedVariantIds)
        {
            if (selectedAddOnIds.Count > 0)
            {
                var variantFailed = false;
                foreach (var addOnVariantId in selectedAddOnIds)
                {
                    if (string.Equals(variantId, addOnVariantId, StringComparison.OrdinalIgnoreCase))
                    {
                        failures.Add($"Ã™Ë†Ã˜Â§Ã˜Â±Ã›Å’Ã˜Â§Ã™â€ Ã˜Âª {variantId} Ã™â€ Ã™â€¦Ã›Å’Ã¢â‚¬Å’Ã˜ÂªÃ™Ë†Ã˜Â§Ã™â€ Ã˜Â¯ Add-on Ã˜Â®Ã™Ë†Ã˜Â¯Ã˜Â´ Ã˜Â¨Ã˜Â§Ã˜Â´Ã˜Â¯.");
                        variantFailed = true;
                        break;
                    }

                    var addOnResult = await _apiService.UpsertVariantAddOnAsync(
                        variantId,
                        new UpsertVariantAddOnRequest
                        {
                            AddOnVariantRef = addOnVariantId
                        },
                        token);

                    if (addOnResult.IsSuccess)
                        continue;

                    failures.Add(addOnResult.ErrorMessage ?? $"Ã˜Â«Ã˜Â¨Ã˜Âª Add-on Ã˜Â¨Ã˜Â±Ã˜Â§Ã›Å’ Ã™Ë†Ã˜Â§Ã˜Â±Ã›Å’Ã˜Â§Ã™â€ Ã˜Âª {variantId} Ã˜Â§Ã™â€ Ã˜Â¬Ã˜Â§Ã™â€¦ Ã™â€ Ã˜Â´Ã˜Â¯.");
                    variantFailed = true;
                    break;
                }

                if (!variantFailed)
                    successCount++;

                continue;
            }
            if (string.Equals(variantId, form.AddOnVariantId, StringComparison.OrdinalIgnoreCase))
            {
                failures.Add($"Ã™Ë†Ã˜Â§Ã˜Â±Ã›Å’Ã˜Â§Ã™â€ Ã˜Âª {variantId} Ã™â€ Ã™â€¦Ã›Å’Ã¢â‚¬Å’Ã˜ÂªÃ™Ë†Ã˜Â§Ã™â€ Ã˜Â¯ Add-on Ã˜Â®Ã™Ë†Ã˜Â¯Ã˜Â´ Ã˜Â¨Ã˜Â§Ã˜Â´Ã˜Â¯.");
                continue;
            }

            var result = await _apiService.UpsertVariantAddOnAsync(
                variantId,
                new UpsertVariantAddOnRequest
                {
                    AddOnVariantRef = form.AddOnVariantId
                },
                token);

            if (!result.IsSuccess)
            {
                failures.Add(result.ErrorMessage ?? $"Ã˜Â«Ã˜Â¨Ã˜Âª Add-on Ã˜Â¨Ã˜Â±Ã˜Â§Ã›Å’ Ã™Ë†Ã˜Â§Ã˜Â±Ã›Å’Ã˜Â§Ã™â€ Ã˜Âª {variantId} Ã˜Â§Ã™â€ Ã˜Â¬Ã˜Â§Ã™â€¦ Ã™â€ Ã˜Â´Ã˜Â¯.");
                continue;
            }

            successCount++;
        }

        if (successCount > 0)
        {
            TempData["CatalogSuccess"] = $"Add-on Ã˜Â¨Ã˜Â±Ã˜Â§Ã›Å’ {successCount} Ã™Ë†Ã˜Â§Ã˜Â±Ã›Å’Ã˜Â§Ã™â€ Ã˜Âª Ã˜Â«Ã˜Â¨Ã˜Âª Ã˜Â´Ã˜Â¯.";
        }

        if (isAjaxRequest)
        {
            return Json(new
            {
                isSuccess = failures.Count == 0,
                successCount,
                message = successCount > 0
                    ? $"Add-on برای {successCount} واریانت ثبت شد."
                    : (failures.Count > 0 ? string.Join(" | ", failures.Take(3)) : string.Empty),
                reloadVariantId = selectedVariantIds.FirstOrDefault(),
                errors = failures.Take(3).ToList()
            });
        }

        if (failures.Count > 0)
        {
            TempData["CatalogError"] = string.Join(" | ", failures.Take(3)) + (failures.Count > 3 ? " | ..." : string.Empty);
        }

        return RedirectToAction(nameof(Variants), new { productId = form.ProductId, variantId = selectedVariantIds.FirstOrDefault() });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> BulkAssignVariantImages([Bind(Prefix = "BulkVariantImageForm")] BulkVariantImageForm form)
    {
        if (!TryGetToken(out var token))
        {
            return RedirectToAction("Login", "Auth");
        }

        var isAjaxRequest = string.Equals(Request.Headers["X-Requested-With"], "XMLHttpRequest", StringComparison.OrdinalIgnoreCase) ||
                            Request.Headers.Accept.Any(x => x.Contains("application/json", StringComparison.OrdinalIgnoreCase));

        var selectedVariantIds = (form.SelectedVariantIds ?? string.Empty)
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        if (selectedVariantIds.Count == 0)
        {
            var errorMessage = _uiText["catalog.variants.images.noVariantsSelected"];
            if (isAjaxRequest)
            {
                return BadRequest(new { isSuccess = false, error = errorMessage });
            }

            TempData["CatalogError"] = errorMessage;
            return RedirectToAction(nameof(Variants), new { productId = form.ProductId });
        }

        List<VariantImageModel> images;
        try
        {
            images = JsonSerializer.Deserialize<List<VariantImageModel>>(form.ImagesJson ?? "[]", new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }) ?? new List<VariantImageModel>();
        }
        catch
        {
            var errorMessage = _uiText["catalog.variants.images.invalidPayload"];
            if (isAjaxRequest)
            {
                return BadRequest(new { isSuccess = false, error = errorMessage });
            }

            TempData["CatalogError"] = errorMessage;
            return RedirectToAction(nameof(Variants), new { productId = form.ProductId });
        }

        images = images
            .Where(x => !string.IsNullOrWhiteSpace(x.FileKey))
            .OrderBy(x => x.DisplayOrder)
            .ToList();

        if (images.Count == 0)
        {
            var errorMessage = _uiText["catalog.variants.images.noValidImages"];
            if (isAjaxRequest)
            {
                return BadRequest(new { isSuccess = false, error = errorMessage });
            }

            TempData["CatalogError"] = errorMessage;
            return RedirectToAction(nameof(Variants), new { productId = form.ProductId });
        }

        TempData["BulkVariantImagesJson"] = JsonSerializer.Serialize(images);

        var failures = new List<string>();
        var successCount = 0;

        foreach (var variantId in selectedVariantIds)
        {
            var variantFailed = false;
            foreach (var image in images)
            {
                var result = await _apiService.UpsertVariantImageAsync(
                    variantId,
                    new UpsertVariantImageRequest
                    {
                        FileKey = image.FileKey,
                        OriginalFileName = image.OriginalFileName,
                        ContentType = image.ContentType,
                        OriginalUrl = image.OriginalUrl,
                        ThumbnailUrl = image.ThumbnailUrl,
                        DisplayOrder = image.DisplayOrder,
                        IsPrimary = image.IsPrimary
                    },
                    token);

                if (result.IsSuccess)
                    continue;

                variantFailed = true;
                failures.Add(result.ErrorMessage ?? string.Format(_uiText["catalog.variants.images.uploadFailedForVariant"], variantId));
                break;
            }

            if (!variantFailed)
                successCount++;
        }

        if (successCount > 0)
        {
            var successMessage = string.Format(_uiText["catalog.variants.images.uploadSucceeded"], successCount);
            if (isAjaxRequest)
            {
                return Json(new
                {
                    isSuccess = failures.Count == 0,
                    successCount,
                    message = successMessage,
                    reloadVariantId = selectedVariantIds.FirstOrDefault(),
                    errors = failures.Take(3).ToList()
                });
            }

            TempData["CatalogSuccess"] = successMessage;
        }

        if (isAjaxRequest)
        {
            return Json(new
            {
                isSuccess = failures.Count == 0,
                successCount,
                message = successCount > 0 ? string.Format(_uiText["catalog.variants.images.uploadSucceeded"], successCount) : string.Empty,
                reloadVariantId = selectedVariantIds.FirstOrDefault(),
                errors = failures.Take(3).ToList()
            });
        }

        if (failures.Count > 0)
            TempData["CatalogError"] = string.Join(" | ", failures.Take(3)) + (failures.Count > 3 ? " | ..." : string.Empty);

        return RedirectToAction(nameof(Variants), new { productId = form.ProductId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RemoveVariantImage(string productId, string variantId, string fileKey)
    {
        var isAjaxRequest = string.Equals(Request.Headers["X-Requested-With"], "XMLHttpRequest", StringComparison.OrdinalIgnoreCase) ||
                            Request.Headers.Accept.Any(x => x.Contains("application/json", StringComparison.OrdinalIgnoreCase));

        if (!TryGetToken(out var token))
        {
            if (isAjaxRequest)
            {
                return Unauthorized(new { isSuccess = false, error = "Authentication required." });
            }

            return RedirectToAction("Login", "Auth");
        }

        if (string.IsNullOrWhiteSpace(variantId) || string.IsNullOrWhiteSpace(fileKey))
        {
            var errorMessage = "شناسه واریانت یا فایل تصویر معتبر نیست.";
            if (isAjaxRequest)
            {
                return BadRequest(new { isSuccess = false, error = errorMessage });
            }

            TempData["CatalogError"] = "Ã˜Â´Ã™â€ Ã˜Â§Ã˜Â³Ã™â€¡ Ã™Ë†Ã˜Â§Ã˜Â±Ã›Å’Ã˜Â§Ã™â€ Ã˜Âª Ã›Å’Ã˜Â§ Ã™ÂÃ˜Â§Ã›Å’Ã™â€ž Ã˜ÂªÃ˜ÂµÃ™Ë†Ã›Å’Ã˜Â± Ã™â€¦Ã˜Â¹Ã˜ÂªÃ˜Â¨Ã˜Â± Ã™â€ Ã›Å’Ã˜Â³Ã˜Âª.";
            return RedirectToAction(nameof(Variants), new { productId, variantId });
        }

        var result = await _apiService.RemoveVariantImageAsync(variantId, fileKey, token);
        if (!result.IsSuccess)
        {
            if (isAjaxRequest)
            {
                return BadRequest(new { isSuccess = false, error = result.ErrorMessage ?? "حذف تصویر واریانت انجام نشد." });
            }

            TempData["CatalogError"] = result.ErrorMessage ?? "Ã˜Â­Ã˜Â°Ã™Â Ã˜ÂªÃ˜ÂµÃ™Ë†Ã›Å’Ã˜Â± Ã™Ë†Ã˜Â§Ã˜Â±Ã›Å’Ã˜Â§Ã™â€ Ã˜Âª Ã˜Â§Ã™â€ Ã˜Â¬Ã˜Â§Ã™â€¦ Ã™â€ Ã˜Â´Ã˜Â¯.";
        }
        else if (isAjaxRequest)
        {
            return Json(new
            {
                isSuccess = true,
                message = "تصویر واریانت حذف شد.",
                productId,
                variantId,
                fileKey
            });
        }
        else
        {
            TempData["CatalogSuccess"] = "Ã˜ÂªÃ˜ÂµÃ™Ë†Ã›Å’Ã˜Â± Ã™Ë†Ã˜Â§Ã˜Â±Ã›Å’Ã˜Â§Ã™â€ Ã˜Âª Ã˜Â­Ã˜Â°Ã™Â Ã˜Â´Ã˜Â¯.";
        }

        return RedirectToAction(nameof(Variants), new { productId, variantId });
    }

    private async Task<(bool Success, string? Error, CreateInventoryDocumentForm? Form)> BuildAssembleDocumentAsync(
        string token,
        ProductVariantDetailsModel variant,
        IReadOnlyCollection<VariantComponentModel> recipe,
        decimal quantity,
        string? reasonCode)
    {
        if (recipe.Count > 0)
        {
            if (!TryResolveRecipeWarehouse(recipe, out var recipeWarehouseRef, out var warehouseError))
                return (false, warehouseError, null);

            var recipeLocations = recipe
                .Select(x => x.LocationId)
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            if (recipeLocations.Count != 1)
                return (false, "Ã˜Â¨Ã˜Â±Ã˜Â§Ã›Å’ Ã™â€¦Ã™Ë†Ã™â€ Ã˜ÂªÃ˜Â§ÃšËœ Ã˜Â®Ã™Ë†Ã˜Â¯ÃšÂ©Ã˜Â§Ã˜Â±Ã˜Å’ Ã™â€¡Ã™â€¦Ã™â€¡ Ã˜Â§Ã˜Â¬Ã˜Â²Ã˜Â§Ã›Å’ recipe Ã˜Â¨Ã˜Â§Ã›Å’Ã˜Â¯ Ã›Å’ÃšÂ© Ã™â€žÃ™Ë†ÃšÂ©Ã›Å’Ã˜Â´Ã™â€  Ã™â€¦Ã™â€šÃ˜ÂµÃ˜Â¯ Ã™â€¦Ã˜Â´Ã˜ÂªÃ˜Â±ÃšÂ© Ã˜Â¯Ã˜Â§Ã˜Â´Ã˜ÂªÃ™â€¡ Ã˜Â¨Ã˜Â§Ã˜Â´Ã™â€ Ã˜Â¯.", null);

            var componentBuckets = new Dictionary<string, List<StockDetailBucketModel>>(StringComparer.OrdinalIgnoreCase);
            foreach (var component in recipe)
            {
                var componentVariantRef = Guid.Parse(component.ComponentVariantId);
                var configuredLocationRef = Guid.Parse(component.LocationId);
                var bucketsResult = await _apiService.GetAvailableStockBucketsAsync(token, variantRef: componentVariantRef);
                if (!bucketsResult.IsSuccess)
                    return (false, bucketsResult.ErrorMessage ?? "Ã™â€¦Ã™Ë†Ã˜Â¬Ã™Ë†Ã˜Â¯Ã›Å’ Ã˜Â§Ã˜Â¬Ã˜Â²Ã˜Â§Ã›Å’ Ã˜Â³Ã˜Â§Ã˜Â²Ã™â€ Ã˜Â¯Ã™â€¡ Ã˜Â¨Ã˜Â§Ã˜Â±ÃšÂ¯Ã˜Â°Ã˜Â§Ã˜Â±Ã›Å’ Ã™â€ Ã˜Â´Ã˜Â¯.", null);

                componentBuckets[component.ComponentId] = (bucketsResult.Data?.Items ?? new List<StockDetailBucketModel>())
                    .Where(x =>
                        x.QuantityOnHand > 0 &&
                        x.WarehouseRef == recipeWarehouseRef &&
                        x.LocationRef == configuredLocationRef)
                    .ToList();
            }

            var sellerQualityGroups = recipe.Select(component =>
                componentBuckets[component.ComponentId]
                    .GroupBy(ToSellerQualityKey)
                    .Where(group => group.Sum(x => x.QuantityOnHand) >= component.Quantity * quantity)
                    .Select(group => group.Key)
                    .ToHashSet())
                .ToList();

            if (sellerQualityGroups.Any(group => group.Count == 0))
                return (false, "Ã˜Â¨Ã˜Â±Ã˜Â§Ã›Å’ Ã›Å’ÃšÂ©Ã›Å’ Ã˜Â§Ã˜Â² Ã˜Â§Ã˜Â¬Ã˜Â²Ã˜Â§Ã›Å’ Ã˜Â³Ã˜Â§Ã˜Â²Ã™â€ Ã˜Â¯Ã™â€¡Ã˜Å’ Ã˜Â¯Ã˜Â± Ã˜Â§Ã™â€ Ã˜Â¨Ã˜Â§Ã˜Â± Ã™Ë† Ã™â€žÃ™Ë†ÃšÂ©Ã›Å’Ã˜Â´Ã™â€  Ã˜Â«Ã˜Â¨Ã˜ÂªÃ¢â‚¬Å’Ã˜Â´Ã˜Â¯Ã™â€¡ Ã™â€¦Ã™Ë†Ã˜Â¬Ã™Ë†Ã˜Â¯Ã›Å’ ÃšÂ©Ã˜Â§Ã™ÂÃ›Å’ Ã™Â¾Ã›Å’Ã˜Â¯Ã˜Â§ Ã™â€ Ã˜Â´Ã˜Â¯.", null);

            var sharedSellerQualities = sellerQualityGroups.Skip(1)
                .Aggregate(new HashSet<string>(sellerQualityGroups.First(), StringComparer.OrdinalIgnoreCase), (acc, set) =>
                {
                    acc.IntersectWith(set);
                    return acc;
                });

            if (sharedSellerQualities.Count == 0)
                return (false, "Ã˜Â§Ã˜Â¬Ã˜Â²Ã˜Â§Ã›Å’ Ã˜Â³Ã˜Â§Ã˜Â²Ã™â€ Ã˜Â¯Ã™â€¡ Ã˜Â¯Ã˜Â± Ã›Å’ÃšÂ© Ã™ÂÃ˜Â±Ã™Ë†Ã˜Â´Ã™â€ Ã˜Â¯Ã™â€¡/ÃšÂ©Ã›Å’Ã™ÂÃ›Å’Ã˜Âª Ã™â€¦Ã˜Â´Ã˜ÂªÃ˜Â±ÃšÂ© Ã˜Â¨Ã˜Â±Ã˜Â§Ã›Å’ Ã™â€¦Ã™Ë†Ã™â€ Ã˜ÂªÃ˜Â§ÃšËœ Ã™Â¾Ã›Å’Ã˜Â¯Ã˜Â§ Ã™â€ Ã˜Â´Ã˜Â¯Ã™â€ Ã˜Â¯.", null);

            if (sharedSellerQualities.Count > 1)
                return (false, "Ã˜Â¨Ã˜Â±Ã˜Â§Ã›Å’ Ã™â€¦Ã™Ë†Ã™â€ Ã˜ÂªÃ˜Â§ÃšËœ Ã˜Â¨Ã›Å’Ã˜Â´ Ã˜Â§Ã˜Â² Ã›Å’ÃšÂ© Ã™ÂÃ˜Â±Ã™Ë†Ã˜Â´Ã™â€ Ã˜Â¯Ã™â€¡/ÃšÂ©Ã›Å’Ã™ÂÃ›Å’Ã˜Âª Ã™â€¦Ã™â€¦ÃšÂ©Ã™â€  Ã™Â¾Ã›Å’Ã˜Â¯Ã˜Â§ Ã˜Â´Ã˜Â¯. Ã˜Â§Ã˜Â¨Ã˜ÂªÃ˜Â¯Ã˜Â§ Ã™â€¦Ã™Ë†Ã˜Â¬Ã™Ë†Ã˜Â¯Ã›Å’ Ã˜Â±Ã˜Â§ Ã›Å’ÃšÂ©Ã˜Â¯Ã˜Â³Ã˜Âª ÃšÂ©Ã™â€ Ã›Å’Ã˜Â¯.", null);

            var selectedSellerQuality = sharedSellerQualities.First();
            var selectedSellerQualityParts = ParseSellerQualityKey(selectedSellerQuality);
            var assembledLines = new List<CreateInventoryDocumentLineForm>();

            foreach (var component in recipe)
            {
                var remaining = component.Quantity * quantity;
                foreach (var bucket in componentBuckets[component.ComponentId]
                             .Where(x => string.Equals(ToSellerQualityKey(x), selectedSellerQuality, StringComparison.OrdinalIgnoreCase))
                             .OrderByDescending(x => x.QuantityOnHand))
                {
                    if (remaining <= 0)
                        break;

                    var take = Math.Min(bucket.QuantityOnHand, remaining);
                    if (take <= 0)
                        continue;

                    assembledLines.Add(new CreateInventoryDocumentLineForm
                    {
                        VariantId = component.ComponentVariantId,
                        Qty = take,
                        UomRef = string.Empty,
                        BaseUomRef = string.Empty,
                        SourceLocationRef = bucket.LocationRef.ToString("D"),
                        QualityStatusRef = bucket.QualityStatusRef.ToString("D"),
                        LotBatchNo = bucket.LotBatchNo
                    });

                    remaining -= take;
                }

                if (remaining > 0)
                    return (false, "Ã™â€¦Ã™Ë†Ã˜Â¬Ã™Ë†Ã˜Â¯Ã›Å’ Ã˜Â§Ã˜Â¬Ã˜Â²Ã˜Â§Ã›Å’ Ã˜Â³Ã˜Â§Ã˜Â²Ã™â€ Ã˜Â¯Ã™â€¡ Ã˜Â¨Ã˜Â±Ã˜Â§Ã›Å’ Ã™â€¦Ã™Ë†Ã™â€ Ã˜ÂªÃ˜Â§ÃšËœ ÃšÂ©Ã˜Â§Ã™ÂÃ›Å’ Ã™â€ Ã›Å’Ã˜Â³Ã˜Âª.", null);
            }

            assembledLines.Add(new CreateInventoryDocumentLineForm
            {
                VariantId = variant.Id,
                Qty = quantity,
                UomRef = variant.BaseUomRef,
                BaseUomRef = variant.BaseUomRef,
                DestinationLocationRef = recipeLocations[0],
                QualityStatusRef = selectedSellerQualityParts.QualityStatusRef.ToString("D")
            });

            return (true, null, CreateConversionDocumentForm(
                recipeWarehouseRef,
                selectedSellerQualityParts.SellerRef,
                $"Assembly:{variant.Sku}",
                reasonCode,
                assembledLines));
        }

        var requiredByComponent = recipe.ToDictionary(
            x => x.ComponentVariantId,
            x => x.Quantity * quantity,
            StringComparer.OrdinalIgnoreCase);

        var bucketMap = new Dictionary<string, List<StockDetailBucketModel>>(StringComparer.OrdinalIgnoreCase);
        foreach (var entry in requiredByComponent)
        {
            var bucketsResult = await _apiService.GetAvailableStockBucketsAsync(token, variantRef: Guid.Parse(entry.Key));
            if (!bucketsResult.IsSuccess)
            {
                return (false, bucketsResult.ErrorMessage ?? "Ã™â€¦Ã™Ë†Ã˜Â¬Ã™Ë†Ã˜Â¯Ã›Å’ Ã˜Â§Ã˜Â¬Ã˜Â²Ã˜Â§Ã›Å’ Ã˜Â³Ã˜Â§Ã˜Â²Ã™â€ Ã˜Â¯Ã™â€¡ Ã˜Â¨Ã˜Â§Ã˜Â±ÃšÂ¯Ã˜Â°Ã˜Â§Ã˜Â±Ã›Å’ Ã™â€ Ã˜Â´Ã˜Â¯.", null);
            }

            var buckets = bucketsResult.Data?.Items ?? new List<StockDetailBucketModel>();
            bucketMap[entry.Key] = buckets.Where(x => x.QuantityOnHand > 0).ToList();
        }

        var contextGroups = requiredByComponent.Select(entry =>
            bucketMap[entry.Key]
                .GroupBy(ToContextKey)
                .Where(group => group.Sum(x => x.QuantityOnHand) >= entry.Value)
                .Select(group => group.Key)
                .ToHashSet())
            .ToList();

        if (contextGroups.Any(group => group.Count == 0))
        {
            return (false, "Ã˜Â¨Ã˜Â±Ã˜Â§Ã›Å’ Ã›Å’ÃšÂ©Ã›Å’ Ã˜Â§Ã˜Â² Ã˜Â§Ã˜Â¬Ã˜Â²Ã˜Â§Ã›Å’ Ã˜Â³Ã˜Â§Ã˜Â²Ã™â€ Ã˜Â¯Ã™â€¡Ã˜Å’ Ã™â€¦Ã™Ë†Ã˜Â¬Ã™Ë†Ã˜Â¯Ã›Å’ ÃšÂ©Ã˜Â§Ã™ÂÃ›Å’ Ã˜Â¯Ã˜Â± Ã™â€¡Ã›Å’Ãšâ€  context Ã™â€¦Ã˜Â´Ã˜ÂªÃ˜Â±ÃšÂ©Ã›Å’ Ã™Â¾Ã›Å’Ã˜Â¯Ã˜Â§ Ã™â€ Ã˜Â´Ã˜Â¯.", null);
        }

        var sharedContexts = contextGroups.Skip(1)
            .Aggregate(new HashSet<string>(contextGroups.First(), StringComparer.OrdinalIgnoreCase), (acc, set) =>
            {
                acc.IntersectWith(set);
                return acc;
            });

        if (sharedContexts.Count == 0)
        {
            return (false, "Ã˜Â§Ã˜Â¬Ã˜Â²Ã˜Â§Ã›Å’ Ã˜Â³Ã˜Â§Ã˜Â²Ã™â€ Ã˜Â¯Ã™â€¡ Ã˜Â¯Ã˜Â± Ã›Å’ÃšÂ© Ã˜Â§Ã™â€ Ã˜Â¨Ã˜Â§Ã˜Â±/Ã™ÂÃ˜Â±Ã™Ë†Ã˜Â´Ã™â€ Ã˜Â¯Ã™â€¡/Ã™â€žÃ™Ë†ÃšÂ©Ã›Å’Ã˜Â´Ã™â€ /ÃšÂ©Ã›Å’Ã™ÂÃ›Å’Ã˜Âª Ã™â€¦Ã˜Â´Ã˜ÂªÃ˜Â±ÃšÂ© Ã˜Â¨Ã˜Â±Ã˜Â§Ã›Å’ Ã™â€¦Ã™Ë†Ã™â€ Ã˜ÂªÃ˜Â§ÃšËœ Ã™Â¾Ã›Å’Ã˜Â¯Ã˜Â§ Ã™â€ Ã˜Â´Ã˜Â¯Ã™â€ Ã˜Â¯.", null);
        }

        if (sharedContexts.Count > 1)
        {
            return (false, "Ã˜Â¨Ã˜Â±Ã˜Â§Ã›Å’ Ã™â€¦Ã™Ë†Ã™â€ Ã˜ÂªÃ˜Â§ÃšËœ Ã˜Â¨Ã›Å’Ã˜Â´ Ã˜Â§Ã˜Â² Ã›Å’ÃšÂ© context Ã™â€¦Ã™â€¦ÃšÂ©Ã™â€  Ã™Â¾Ã›Å’Ã˜Â¯Ã˜Â§ Ã˜Â´Ã˜Â¯. Ã˜Â§Ã˜Â¨Ã˜ÂªÃ˜Â¯Ã˜Â§ Ã™â€¦Ã™Ë†Ã˜Â¬Ã™Ë†Ã˜Â¯Ã›Å’ Ã˜Â±Ã˜Â§ Ã›Å’ÃšÂ©Ã˜Â¯Ã˜Â³Ã˜Âª ÃšÂ©Ã™â€ Ã›Å’Ã˜Â¯.", null);
        }

        var selectedContext = sharedContexts.First();
        var contextParts = ParseContextKey(selectedContext);
        var lines = new List<CreateInventoryDocumentLineForm>();

        foreach (var entry in requiredByComponent)
        {
            var remaining = entry.Value;
            foreach (var bucket in bucketMap[entry.Key]
                         .Where(x => string.Equals(ToContextKey(x), selectedContext, StringComparison.OrdinalIgnoreCase))
                         .OrderByDescending(x => x.QuantityOnHand))
            {
                if (remaining <= 0)
                {
                    break;
                }

                var take = Math.Min(bucket.QuantityOnHand, remaining);
                if (take <= 0)
                {
                    continue;
                }

                lines.Add(new CreateInventoryDocumentLineForm
                {
                    VariantId = entry.Key,
                    Qty = take,
                    UomRef = string.Empty,
                    BaseUomRef = string.Empty,
                    SourceLocationRef = bucket.LocationRef.ToString("D"),
                    QualityStatusRef = bucket.QualityStatusRef.ToString("D"),
                    LotBatchNo = bucket.LotBatchNo
                });

                remaining -= take;
            }

            if (remaining > 0)
            {
                return (false, "Ã™â€¦Ã™Ë†Ã˜Â¬Ã™Ë†Ã˜Â¯Ã›Å’ Ã˜Â§Ã˜Â¬Ã˜Â²Ã˜Â§Ã›Å’ Ã˜Â³Ã˜Â§Ã˜Â²Ã™â€ Ã˜Â¯Ã™â€¡ Ã˜Â¨Ã˜Â±Ã˜Â§Ã›Å’ Ã™â€¦Ã™Ë†Ã™â€ Ã˜ÂªÃ˜Â§ÃšËœ ÃšÂ©Ã˜Â§Ã™ÂÃ›Å’ Ã™â€ Ã›Å’Ã˜Â³Ã˜Âª.", null);
            }
        }

        lines.Add(new CreateInventoryDocumentLineForm
        {
            VariantId = variant.Id,
            Qty = quantity,
            UomRef = variant.BaseUomRef,
            BaseUomRef = variant.BaseUomRef,
            DestinationLocationRef = contextParts.LocationRef.ToString("D"),
            QualityStatusRef = contextParts.QualityStatusRef.ToString("D")
        });

        return (true, null, CreateConversionDocumentForm(
            contextParts.WarehouseRef,
            contextParts.SellerRef,
            $"Assembly:{variant.Sku}",
            reasonCode,
            lines));
    }

    private async Task<(bool Success, string? Error, CreateInventoryDocumentForm? Form)> BuildDisassembleDocumentAsync(
        string token,
        ProductVariantDetailsModel variant,
        IReadOnlyCollection<VariantComponentModel> recipe,
        decimal quantity,
        string? reasonCode)
    {
        if (recipe.Count > 0)
        {
            if (!Guid.TryParse(variant.Id, out var recipeVariantRef))
            {
                return (false, "Ã˜Â´Ã™â€ Ã˜Â§Ã˜Â³Ã™â€¡ Ã™Ë†Ã˜Â§Ã˜Â±Ã›Å’Ã˜Â§Ã™â€ Ã˜Âª Ã™â€¦Ã˜Â¹Ã˜ÂªÃ˜Â¨Ã˜Â± Ã™â€ Ã›Å’Ã˜Â³Ã˜Âª.", null);
            }

            if (!TryResolveRecipeWarehouse(recipe, out var recipeWarehouseRef, out var warehouseError))
            {
                return (false, warehouseError, null);
            }

            var recipeBucketsResult = await _apiService.GetAvailableStockBucketsAsync(token, variantRef: recipeVariantRef);
            if (!recipeBucketsResult.IsSuccess)
            {
                return (false, recipeBucketsResult.ErrorMessage ?? "Ã™â€¦Ã™Ë†Ã˜Â¬Ã™Ë†Ã˜Â¯Ã›Å’ Ã™Ë†Ã˜Â§Ã˜Â±Ã›Å’Ã˜Â§Ã™â€ Ã˜Âª Ã˜Â¨Ã˜Â±Ã˜Â§Ã›Å’ Ã˜Â¯Ã›Å’Ã¢â‚¬Å’Ã˜Â§Ã˜Â³Ã™â€¦Ã˜Â¨Ã™â€ž Ã˜Â¨Ã˜Â§Ã˜Â±ÃšÂ¯Ã˜Â°Ã˜Â§Ã˜Â±Ã›Å’ Ã™â€ Ã˜Â´Ã˜Â¯.", null);
            }

            var recipeCandidateBuckets = (recipeBucketsResult.Data?.Items ?? new List<StockDetailBucketModel>())
                .Where(x => x.QuantityOnHand > 0 && x.WarehouseRef == recipeWarehouseRef)
                .ToList();

            var candidateSellerQualities = recipeCandidateBuckets
                .GroupBy(ToSellerQualityKey)
                .Where(group => group.Sum(x => x.QuantityOnHand) >= quantity)
                .Select(group => group.Key)
                .ToList();

            if (candidateSellerQualities.Count == 0)
            {
                return (false, "Ã˜Â¨Ã˜Â±Ã˜Â§Ã›Å’ Ã˜Â¯Ã›Å’Ã¢â‚¬Å’Ã˜Â§Ã˜Â³Ã™â€¦Ã˜Â¨Ã™â€žÃ˜Å’ Ã™â€¦Ã™Ë†Ã˜Â¬Ã™Ë†Ã˜Â¯Ã›Å’ ÃšÂ©Ã˜Â§Ã™ÂÃ›Å’ Ã˜Â§Ã˜Â² Ã™Ë†Ã˜Â§Ã˜Â±Ã›Å’Ã˜Â§Ã™â€ Ã˜Âª Ã™â€¦Ã™â€ Ã˜ÂªÃ˜Â®Ã˜Â¨ Ã˜Â¯Ã˜Â± Ã˜Â§Ã™â€ Ã˜Â¨Ã˜Â§Ã˜Â± recipe Ã™Â¾Ã›Å’Ã˜Â¯Ã˜Â§ Ã™â€ Ã˜Â´Ã˜Â¯.", null);
            }

            if (candidateSellerQualities.Count > 1)
            {
                return (false, "Ã˜Â¨Ã˜Â±Ã˜Â§Ã›Å’ Ã˜Â¯Ã›Å’Ã¢â‚¬Å’Ã˜Â§Ã˜Â³Ã™â€¦Ã˜Â¨Ã™â€ž Ã˜Â¨Ã›Å’Ã˜Â´ Ã˜Â§Ã˜Â² Ã›Å’ÃšÂ© Ã™ÂÃ˜Â±Ã™Ë†Ã˜Â´Ã™â€ Ã˜Â¯Ã™â€¡/ÃšÂ©Ã›Å’Ã™ÂÃ›Å’Ã˜Âª Ã™â€¦Ã™â€¦ÃšÂ©Ã™â€  Ã™Â¾Ã›Å’Ã˜Â¯Ã˜Â§ Ã˜Â´Ã˜Â¯. Ã˜Â§Ã˜Â¨Ã˜ÂªÃ˜Â¯Ã˜Â§ Ã™â€¦Ã™Ë†Ã˜Â¬Ã™Ë†Ã˜Â¯Ã›Å’ Ã˜Â§Ã›Å’Ã™â€  Ã™Ë†Ã˜Â§Ã˜Â±Ã›Å’Ã˜Â§Ã™â€ Ã˜Âª Ã˜Â±Ã˜Â§ Ã›Å’ÃšÂ©Ã˜Â¯Ã˜Â³Ã˜Âª ÃšÂ©Ã™â€ Ã›Å’Ã˜Â¯.", null);
            }

            var selectedSellerQuality = candidateSellerQualities[0];
            var selectedSellerQualityParts = ParseSellerQualityKey(selectedSellerQuality);
            var disassemblyLines = new List<CreateInventoryDocumentLineForm>();
            var remainingRecipeQty = quantity;

            foreach (var bucket in recipeCandidateBuckets
                         .Where(x => string.Equals(ToSellerQualityKey(x), selectedSellerQuality, StringComparison.OrdinalIgnoreCase))
                         .OrderByDescending(x => x.QuantityOnHand))
            {
                if (remainingRecipeQty <= 0)
                {
                    break;
                }

                var take = Math.Min(bucket.QuantityOnHand, remainingRecipeQty);
                if (take <= 0)
                {
                    continue;
                }

                disassemblyLines.Add(new CreateInventoryDocumentLineForm
                {
                    VariantId = variant.Id,
                    Qty = take,
                    UomRef = variant.BaseUomRef,
                    BaseUomRef = variant.BaseUomRef,
                    SourceLocationRef = bucket.LocationRef.ToString("D"),
                    QualityStatusRef = bucket.QualityStatusRef.ToString("D"),
                    LotBatchNo = bucket.LotBatchNo
                });

                remainingRecipeQty -= take;
            }

            if (remainingRecipeQty > 0)
            {
                return (false, "Ã™â€¦Ã™Ë†Ã˜Â¬Ã™Ë†Ã˜Â¯Ã›Å’ Ã™Ë†Ã˜Â§Ã˜Â±Ã›Å’Ã˜Â§Ã™â€ Ã˜Âª Ã™â€¦Ã™â€ Ã˜ÂªÃ˜Â®Ã˜Â¨ Ã˜Â¨Ã˜Â±Ã˜Â§Ã›Å’ Ã˜Â¯Ã›Å’Ã¢â‚¬Å’Ã˜Â§Ã˜Â³Ã™â€¦Ã˜Â¨Ã™â€ž ÃšÂ©Ã˜Â§Ã™ÂÃ›Å’ Ã™â€ Ã›Å’Ã˜Â³Ã˜Âª.", null);
            }

            foreach (var component in recipe)
            {
                disassemblyLines.Add(new CreateInventoryDocumentLineForm
                {
                    VariantId = component.ComponentVariantId,
                    Qty = component.Quantity * quantity,
                    UomRef = string.Empty,
                    BaseUomRef = string.Empty,
                    DestinationLocationRef = component.LocationId,
                    QualityStatusRef = selectedSellerQualityParts.QualityStatusRef.ToString("D")
                });
            }

            return (true, null, CreateConversionDocumentForm(
                recipeWarehouseRef,
                selectedSellerQualityParts.SellerRef,
                $"Disassembly:{variant.Sku}",
                reasonCode,
                disassemblyLines));
        }

        if (!Guid.TryParse(variant.Id, out var variantRef))
        {
            return (false, "Ã˜Â´Ã™â€ Ã˜Â§Ã˜Â³Ã™â€¡ Ã™Ë†Ã˜Â§Ã˜Â±Ã›Å’Ã˜Â§Ã™â€ Ã˜Âª Ã™â€¦Ã˜Â¹Ã˜ÂªÃ˜Â¨Ã˜Â± Ã™â€ Ã›Å’Ã˜Â³Ã˜Âª.", null);
        }

        var bucketsResultFallback = await _apiService.GetAvailableStockBucketsAsync(token, variantRef: variantRef);
        if (!bucketsResultFallback.IsSuccess)
        {
            return (false, bucketsResultFallback.ErrorMessage ?? "Ã™â€¦Ã™Ë†Ã˜Â¬Ã™Ë†Ã˜Â¯Ã›Å’ Ã™Ë†Ã˜Â§Ã˜Â±Ã›Å’Ã˜Â§Ã™â€ Ã˜Âª Ã˜Â¨Ã˜Â±Ã˜Â§Ã›Å’ Ã˜Â¯Ã›Å’Ã¢â‚¬Å’Ã˜Â§Ã˜Â³Ã™â€¦Ã˜Â¨Ã™â€ž Ã˜Â¨Ã˜Â§Ã˜Â±ÃšÂ¯Ã˜Â°Ã˜Â§Ã˜Â±Ã›Å’ Ã™â€ Ã˜Â´Ã˜Â¯.", null);
        }

        var candidateBucketsFallback = (bucketsResultFallback.Data?.Items ?? new List<StockDetailBucketModel>())
            .Where(x => x.QuantityOnHand > 0)
            .ToList();

        var candidateContexts = candidateBucketsFallback
            .GroupBy(ToContextKey)
            .Where(group => group.Sum(x => x.QuantityOnHand) >= quantity)
            .Select(group => group.Key)
            .ToList();

        if (candidateContexts.Count == 0)
        {
            return (false, "Ã˜Â¨Ã˜Â±Ã˜Â§Ã›Å’ Ã˜Â¯Ã›Å’Ã¢â‚¬Å’Ã˜Â§Ã˜Â³Ã™â€¦Ã˜Â¨Ã™â€žÃ˜Å’ Ã™â€¦Ã™Ë†Ã˜Â¬Ã™Ë†Ã˜Â¯Ã›Å’ ÃšÂ©Ã˜Â§Ã™ÂÃ›Å’ Ã˜Â§Ã˜Â² Ã™Ë†Ã˜Â§Ã˜Â±Ã›Å’Ã˜Â§Ã™â€ Ã˜Âª Ã™â€¦Ã™â€ Ã˜ÂªÃ˜Â®Ã˜Â¨ Ã™Â¾Ã›Å’Ã˜Â¯Ã˜Â§ Ã™â€ Ã˜Â´Ã˜Â¯.", null);
        }

        if (candidateContexts.Count > 1)
        {
            return (false, "Ã˜Â¨Ã˜Â±Ã˜Â§Ã›Å’ Ã˜Â¯Ã›Å’Ã¢â‚¬Å’Ã˜Â§Ã˜Â³Ã™â€¦Ã˜Â¨Ã™â€ž Ã˜Â¨Ã›Å’Ã˜Â´ Ã˜Â§Ã˜Â² Ã›Å’ÃšÂ© context Ã™â€¦Ã™â€¦ÃšÂ©Ã™â€  Ã™Â¾Ã›Å’Ã˜Â¯Ã˜Â§ Ã˜Â´Ã˜Â¯. Ã˜Â§Ã˜Â¨Ã˜ÂªÃ˜Â¯Ã˜Â§ Ã™â€¦Ã™Ë†Ã˜Â¬Ã™Ë†Ã˜Â¯Ã›Å’ Ã˜Â§Ã›Å’Ã™â€  Ã™Ë†Ã˜Â§Ã˜Â±Ã›Å’Ã˜Â§Ã™â€ Ã˜Âª Ã˜Â±Ã˜Â§ Ã›Å’ÃšÂ©Ã˜Â¯Ã˜Â³Ã˜Âª ÃšÂ©Ã™â€ Ã›Å’Ã˜Â¯.", null);
        }

        var selectedContext = candidateContexts[0];
        var contextParts = ParseContextKey(selectedContext);
        var lines = new List<CreateInventoryDocumentLineForm>();
        var remaining = quantity;

        foreach (var bucket in candidateBucketsFallback
                     .Where(x => string.Equals(ToContextKey(x), selectedContext, StringComparison.OrdinalIgnoreCase))
                     .OrderByDescending(x => x.QuantityOnHand))
        {
            if (remaining <= 0)
            {
                break;
            }

            var take = Math.Min(bucket.QuantityOnHand, remaining);
            if (take <= 0)
            {
                continue;
            }

            lines.Add(new CreateInventoryDocumentLineForm
            {
                VariantId = variant.Id,
                Qty = take,
                UomRef = variant.BaseUomRef,
                BaseUomRef = variant.BaseUomRef,
                SourceLocationRef = bucket.LocationRef.ToString("D"),
                QualityStatusRef = bucket.QualityStatusRef.ToString("D"),
                LotBatchNo = bucket.LotBatchNo
            });

            remaining -= take;
        }

        if (remaining > 0)
        {
            return (false, "Ã™â€¦Ã™Ë†Ã˜Â¬Ã™Ë†Ã˜Â¯Ã›Å’ Ã™Ë†Ã˜Â§Ã˜Â±Ã›Å’Ã˜Â§Ã™â€ Ã˜Âª Ã™â€¦Ã™â€ Ã˜ÂªÃ˜Â®Ã˜Â¨ Ã˜Â¨Ã˜Â±Ã˜Â§Ã›Å’ Ã˜Â¯Ã›Å’Ã¢â‚¬Å’Ã˜Â§Ã˜Â³Ã™â€¦Ã˜Â¨Ã™â€ž ÃšÂ©Ã˜Â§Ã™ÂÃ›Å’ Ã™â€ Ã›Å’Ã˜Â³Ã˜Âª.", null);
        }

        foreach (var component in recipe)
        {
            var componentQty = component.Quantity * quantity;
            lines.Add(new CreateInventoryDocumentLineForm
            {
                VariantId = component.ComponentVariantId,
                Qty = componentQty,
                UomRef = string.Empty,
                BaseUomRef = string.Empty,
                DestinationLocationRef = contextParts.LocationRef.ToString("D"),
                QualityStatusRef = contextParts.QualityStatusRef.ToString("D")
            });
        }

        return (true, null, CreateConversionDocumentForm(
            contextParts.WarehouseRef,
            contextParts.SellerRef,
            $"Disassembly:{variant.Sku}",
            reasonCode,
            lines));
    }

    private static CreateInventoryDocumentForm CreateConversionDocumentForm(
        Guid warehouseRef,
        Guid sellerRef,
        string referenceType,
        string? reasonCode,
        List<CreateInventoryDocumentLineForm> lines)
    {
        return new CreateInventoryDocumentForm
        {
            DocumentType = "Conversion",
            WarehouseRef = warehouseRef.ToString("D"),
            SellerRef = sellerRef.ToString("D"),
            OccurredAt = DateTime.Now,
            ReferenceType = referenceType,
            ReasonCode = string.IsNullOrWhiteSpace(reasonCode) ? null : reasonCode.Trim(),
            Lines = lines
        };
    }

    private static string ToSellerQualityKey(StockDetailBucketModel bucket)
        => $"{bucket.SellerRef:D}|{bucket.QualityStatusRef:D}";

    private static (Guid SellerRef, Guid QualityStatusRef) ParseSellerQualityKey(string key)
    {
        var parts = key.Split('|', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        return (
            Guid.Parse(parts[0]),
            Guid.Parse(parts[1]));
    }

    private static bool TryResolveRecipeWarehouse(
        IReadOnlyCollection<VariantComponentModel> recipe,
        out Guid warehouseRef,
        out string? error)
    {
        warehouseRef = Guid.Empty;
        error = null;

        var warehouseIds = recipe
            .Select(x => x.WarehouseId)
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        if (warehouseIds.Count != 1)
        {
            error = "Ã˜Â¨Ã˜Â±Ã˜Â§Ã›Å’ Ã˜Â¹Ã™â€¦Ã™â€žÃ›Å’Ã˜Â§Ã˜Âª Ã˜Â®Ã™Ë†Ã˜Â¯ÃšÂ©Ã˜Â§Ã˜Â±Ã˜Å’ Ã™â€¡Ã™â€¦Ã™â€¡ Ã˜Â§Ã˜Â¬Ã˜Â²Ã˜Â§Ã›Å’ recipe Ã˜Â¨Ã˜Â§Ã›Å’Ã˜Â¯ Ã˜Â¯Ã˜Â± Ã›Å’ÃšÂ© Ã˜Â§Ã™â€ Ã˜Â¨Ã˜Â§Ã˜Â± Ã˜Â«Ã˜Â¨Ã˜Âª Ã˜Â´Ã˜Â¯Ã™â€¡ Ã˜Â¨Ã˜Â§Ã˜Â´Ã™â€ Ã˜Â¯.";
            return false;
        }

        if (!Guid.TryParse(warehouseIds[0], out warehouseRef))
        {
            error = "Ã˜Â´Ã™â€ Ã˜Â§Ã˜Â³Ã™â€¡ Ã˜Â§Ã™â€ Ã˜Â¨Ã˜Â§Ã˜Â± recipe Ã™â€¦Ã˜Â¹Ã˜ÂªÃ˜Â¨Ã˜Â± Ã™â€ Ã›Å’Ã˜Â³Ã˜Âª.";
            return false;
        }

        return true;
    }

    private static string ToContextKey(StockDetailBucketModel bucket)
        => $"{bucket.SellerRef:D}|{bucket.WarehouseRef:D}|{bucket.LocationRef:D}|{bucket.QualityStatusRef:D}";

    private static (Guid SellerRef, Guid WarehouseRef, Guid LocationRef, Guid QualityStatusRef) ParseContextKey(string key)
    {
        var parts = key.Split('|', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        return (
            Guid.Parse(parts[0]),
            Guid.Parse(parts[1]),
            Guid.Parse(parts[2]),
            Guid.Parse(parts[3]));
    }
}
