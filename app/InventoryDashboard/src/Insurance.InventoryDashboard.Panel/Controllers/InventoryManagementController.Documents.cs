using System.Globalization;
using Insurance.InventoryDashboard.Panel.Models;
using Microsoft.AspNetCore.Mvc;

namespace Insurance.InventoryDashboard.Panel.Controllers;

public sealed partial class InventoryManagementController
{
    [HttpGet]
    public async Task<IActionResult> Documents(
        string? documentId,
        string? documentNo,
        string? documentType,
        string? status,
        string? warehouseId,
        string? sellerId,
        string? occurredFrom,
        string? occurredTo,
        int page = 1,
        int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        if (!TryGetToken(out var token))
        {
            return RedirectToAction("Login", "Auth");
        }

        var roles = ResolveRolesFromSession(token);
        if (!IsAuthorizedFor(token, "Inventory.Document.View", "Inventory.Document.Search", "InventoryDocument.Read", "InventoryDocument.Search", "Document.Read"))
        {
            TempData["CatalogError"] = "شما دسترسی مشاهده اسناد موجودی را ندارید.";
            return RedirectToAction("Index", "Dashboard");
        }

        var modules = await _dashboardConfigService.GetMenuByRolesAsync(roles, cancellationToken);
        var menu = ResolveMenu(modules, "inventory_management", "documents");

        pageSize = NormalizePageSize(pageSize);
        var searchResult = await _apiService.SearchInventoryDocumentsAsync(
            token,
            documentNo,
            documentType,
            status,
            warehouseId,
            sellerId,
            ParseDate(occurredFrom),
            ParseDateEndOfDay(occurredTo),
            Math.Max(page, 1),
            pageSize);

        var warehouseLookupResult = await _apiService.GetWarehouseLookupAsync(token, includeInactive: true);
        var sellerLookupResult = await _apiService.GetSellerLookupAsync(token, includeInactive: true);
        var locationLookupResult = await _apiService.GetLocationLookupAsync(token, warehouseId: null, includeInactive: false);
        var qualityStatusLookupResult = await _apiService.GetQualityStatusLookupAsync(token, includeInactive: false);
        var productLookupResult = await _apiService.SearchProductsAsync(token, page: 1, pageSize: 2000);
        var variantLookupResult = await _apiService.SearchVariantsAsync(token, page: 1, pageSize: 2000);
        var unitOfMeasureLookupResult = await _apiService.GetUnitOfMeasureLookupAsync(token);

        var documents = searchResult.Data?.Items ?? new List<InventoryDocumentListItemModel>();
        var selectedDocumentId = ResolveSelectedDocumentId(documentId, documents);
        var selectedDocumentResult = !string.IsNullOrWhiteSpace(selectedDocumentId)
            ? await _apiService.GetInventoryDocumentByBusinessKeyAsync(selectedDocumentId, token)
            : new ApiResponse<InventoryDocumentDetailsModel> { IsSuccess = true };

        var model = new InventoryDocumentManagementPageViewModel
        {
            UserName = HttpContext.Session.GetString("UserName") ?? "کاربر",
            Roles = roles,
            Permissions = ResolvePermissionsFromSession(),
            Modules = modules,
            ActiveModule = menu.Module,
            ActiveItem = menu.Item,
            SelectedDocumentId = selectedDocumentId,
            SelectedDocumentDetails = selectedDocumentResult.Data,
            Documents = documents,
            WarehouseLookup = warehouseLookupResult.Data ?? new List<WarehouseLookupItemModel>(),
            SellerLookup = sellerLookupResult.Data ?? new List<SellerLookupItemModel>(),
            LocationLookup = locationLookupResult.Data ?? new List<LocationLookupItemModel>(),
            QualityStatusLookup = qualityStatusLookupResult.Data ?? new List<QualityStatusLookupItemModel>(),
            ProductLookup = productLookupResult.Data ?? new List<ProductSummaryModel>(),
            VariantLookup = variantLookupResult.Data ?? new List<ProductVariantSummaryModel>(),
            UnitOfMeasureLookup = unitOfMeasureLookupResult.Data ?? new List<UnitOfMeasureLookupModel>(),
            DocumentNoFilter = documentNo,
            DocumentTypeFilter = documentType,
            DocumentStatusFilter = status,
            WarehouseFilter = warehouseId,
            SellerFilter = sellerId,
            OccurredFromFilter = occurredFrom,
            OccurredToFilter = occurredTo,
            DocumentPage = searchResult.Data?.Page ?? Math.Max(page, 1),
            DocumentPageSize = searchResult.Data?.PageSize ?? pageSize,
            DocumentTotalCount = searchResult.Data?.TotalCount ?? documents.Count,
            DocumentTotalPages = CalculateTotalPages(searchResult.Data?.TotalCount ?? documents.Count, searchResult.Data?.PageSize ?? pageSize),
            PageSizeOptions = PageSizeOptions,
            ErrorMessage = JoinErrors(
                searchResult.ErrorMessage,
                warehouseLookupResult.ErrorMessage,
                sellerLookupResult.ErrorMessage,
                locationLookupResult.ErrorMessage,
                qualityStatusLookupResult.ErrorMessage,
                productLookupResult.ErrorMessage,
                variantLookupResult.ErrorMessage,
                unitOfMeasureLookupResult.ErrorMessage,
                selectedDocumentResult.ErrorMessage),
            CreateForm = BuildCreateForm(warehouseId, sellerId)
        };

        SetLayoutViewBag(model.Modules, model.ActiveModule?.ModuleId, model.ActiveItem?.ItemId, model.UserName);
        return View("~/Views/InventoryManagement/Documents.cshtml", model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SaveDocument(CreateInventoryDocumentForm form)
    {
        if (!TryGetToken(out var token))
        {
            return RedirectToAction("Login", "Auth");
        }

        if (!IsAuthorizedFor(token, "Inventory.Document.Create", "InventoryDocument.Create", "Document.Create"))
        {
            TempData["CatalogError"] = "شما دسترسی ایجاد سند را ندارید.";
            return RedirectToAction(nameof(Documents));
        }

        form.Lines ??= new List<CreateInventoryDocumentLineForm>();
        form.Lines = form.Lines
            .Where(line =>
                !string.IsNullOrWhiteSpace(line.VariantId) ||
                line.Qty > 0 ||
                !string.IsNullOrWhiteSpace(line.SourceLocationRef) ||
                !string.IsNullOrWhiteSpace(line.DestinationLocationRef))
            .ToList();

        if (!TryValidateModel(form))
        {
            TempData["CatalogError"] = ExtractModelError(ModelState);
            return RedirectToAction(nameof(Documents), new { warehouseId = form.WarehouseRef, sellerId = form.SellerRef });
        }

        if (form.Lines.Count == 0)
        {
            TempData["CatalogError"] = "حداقل یک ردیف برای سند لازم است.";
            return RedirectToAction(nameof(Documents), new { warehouseId = form.WarehouseRef, sellerId = form.SellerRef });
        }

        var ruleError = ValidateDocumentFormAgainstBackendRules(form);
        if (!string.IsNullOrWhiteSpace(ruleError))
        {
            TempData["CatalogError"] = ruleError;
            return RedirectToAction(nameof(Documents), new { warehouseId = form.WarehouseRef, sellerId = form.SellerRef });
        }

        var variantLookupResult = await _apiService.SearchVariantsAsync(token, page: 1, pageSize: 2000);
        var variants = variantLookupResult.Data ?? new List<ProductVariantSummaryModel>();

        foreach (var line in form.Lines)
        {
            var variant = variants.FirstOrDefault(x => string.Equals(x.Id, line.VariantId, StringComparison.OrdinalIgnoreCase));
            if (variant is null)
            {
                TempData["CatalogError"] = "واریانت انتخاب شده معتبر نیست.";
                return RedirectToAction(nameof(Documents), new { warehouseId = form.WarehouseRef, sellerId = form.SellerRef });
            }

            line.UomRef = variant.BaseUomRef;
            line.BaseUomRef = variant.BaseUomRef;
        }

        var result = await _apiService.CreateInventoryDocumentAsync(form, token);
        TempData[result.IsSuccess ? "CatalogSuccess" : "CatalogError"] =
            result.IsSuccess ? "سند موجودی با موفقیت ایجاد شد." : result.ErrorMessage ?? "ایجاد سند انجام نشد.";

        if (!result.IsSuccess || result.Data is null)
        {
            return RedirectToAction(nameof(Documents), new { warehouseId = form.WarehouseRef, sellerId = form.SellerRef });
        }

        return RedirectToAction(nameof(Documents), new { documentId = result.Data.DocumentId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ApproveDocument(string documentId)
    {
        if (!TryGetToken(out var token))
        {
            return RedirectToAction("Login", "Auth");
        }

        if (string.IsNullOrWhiteSpace(documentId))
        {
            TempData["CatalogError"] = "سند برای تایید مشخص نشده است.";
            return RedirectToAction(nameof(Documents));
        }

        if (!IsAuthorizedFor(token, "Inventory.Document.Approve", "InventoryDocument.Approve", "Document.Approve"))
        {
            TempData["CatalogError"] = "شما دسترسی تایید سند را ندارید.";
            return RedirectToAction(nameof(Documents), new { documentId });
        }

        var approvedBy = HttpContext.Session.GetString("UserName") ?? "dashboard";
        var result = await _apiService.ApproveInventoryDocumentAsync(documentId, approvedBy, token);
        TempData[result.IsSuccess ? "CatalogSuccess" : "CatalogError"] =
            result.IsSuccess ? "سند تایید شد." : result.ErrorMessage ?? "تایید سند انجام نشد.";
        return RedirectToAction(nameof(Documents), new { documentId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RejectDocument(InventoryDocumentDecisionForm form)
    {
        if (!TryGetToken(out var token))
        {
            return RedirectToAction("Login", "Auth");
        }

        if (!IsAuthorizedFor(token, "Inventory.Document.Reject", "InventoryDocument.Reject", "Document.Reject"))
        {
            TempData["CatalogError"] = "شما دسترسی رد سند را ندارید.";
            return RedirectToAction(nameof(Documents), new { documentId = form.DocumentId });
        }

        if (!TryValidateModel(form))
        {
            TempData["CatalogError"] = ExtractModelError(ModelState);
            return RedirectToAction(nameof(Documents), new { documentId = form.DocumentId });
        }

        var result = await _apiService.RejectInventoryDocumentAsync(form.DocumentId, form.ReasonCode, token);
        TempData[result.IsSuccess ? "CatalogSuccess" : "CatalogError"] =
            result.IsSuccess ? "سند رد شد." : result.ErrorMessage ?? "رد سند انجام نشد.";
        return RedirectToAction(nameof(Documents), new { documentId = form.DocumentId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CancelDocument(InventoryDocumentDecisionForm form)
    {
        if (!TryGetToken(out var token))
        {
            return RedirectToAction("Login", "Auth");
        }

        if (!IsAuthorizedFor(token, "Inventory.Document.Cancel", "InventoryDocument.Cancel", "Document.Cancel"))
        {
            TempData["CatalogError"] = "شما دسترسی لغو سند را ندارید.";
            return RedirectToAction(nameof(Documents), new { documentId = form.DocumentId });
        }

        if (!TryValidateModel(form))
        {
            TempData["CatalogError"] = ExtractModelError(ModelState);
            return RedirectToAction(nameof(Documents), new { documentId = form.DocumentId });
        }

        var result = await _apiService.CancelInventoryDocumentAsync(form.DocumentId, form.ReasonCode, token);
        TempData[result.IsSuccess ? "CatalogSuccess" : "CatalogError"] =
            result.IsSuccess ? "سند لغو شد." : result.ErrorMessage ?? "لغو سند انجام نشد.";
        return RedirectToAction(nameof(Documents), new { documentId = form.DocumentId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> PostDocument(string documentId)
    {
        if (!TryGetToken(out var token))
        {
            return RedirectToAction("Login", "Auth");
        }

        if (string.IsNullOrWhiteSpace(documentId))
        {
            TempData["CatalogError"] = "سند برای پست مشخص نشده است.";
            return RedirectToAction(nameof(Documents));
        }

        if (!IsAuthorizedFor(token, "Inventory.Document.Post", "InventoryDocument.Post", "Document.Post"))
        {
            TempData["CatalogError"] = "شما دسترسی ثبت نهایی سند را ندارید.";
            return RedirectToAction(nameof(Documents), new { documentId });
        }

        var postedBy = HttpContext.Session.GetString("UserName") ?? "dashboard";
        var result = await _apiService.PostInventoryDocumentAsync(documentId, postedBy, token);
        TempData[result.IsSuccess ? "CatalogSuccess" : "CatalogError"] =
            result.IsSuccess ? "سند پست شد." : result.ErrorMessage ?? "پست سند انجام نشد.";
        return RedirectToAction(nameof(Documents), new { documentId });
    }

    private static string? ValidateDocumentFormAgainstBackendRules(CreateInventoryDocumentForm form)
    {
        foreach (var line in form.Lines)
        {
            switch (form.DocumentType)
            {
                case "Receipt":
                case "Return":
                    if (string.IsNullOrWhiteSpace(line.DestinationLocationRef))
                    {
                        return "برای سند رسید یا برگشت، لوکیشن مقصد الزامی است.";
                    }
                    break;

                case "Issue":
                    if (string.IsNullOrWhiteSpace(line.SourceLocationRef))
                    {
                        return "برای سند حواله، لوکیشن مبدا الزامی است.";
                    }
                    break;

                case "Transfer":
                    if (string.IsNullOrWhiteSpace(line.SourceLocationRef) || string.IsNullOrWhiteSpace(line.DestinationLocationRef))
                    {
                        return "برای سند انتقال، لوکیشن مبدا و مقصد الزامی است.";
                    }

                    if (string.Equals(line.SourceLocationRef, line.DestinationLocationRef, StringComparison.OrdinalIgnoreCase))
                    {
                        return "در سند انتقال، لوکیشن مبدا و مقصد باید متفاوت باشند.";
                    }
                    break;

                case "Adjustment":
                    if (string.IsNullOrWhiteSpace(line.AdjustmentDirection))
                    {
                        return "برای سند تعدیل، جهت تعدیل الزامی است.";
                    }

                    if (string.IsNullOrWhiteSpace(line.ReasonCode))
                    {
                        return "برای سند تعدیل، علت ردیف الزامی است.";
                    }
                    break;

                case "QualityChange":
                    if (string.IsNullOrWhiteSpace(line.FromQualityStatusRef) || string.IsNullOrWhiteSpace(line.ToQualityStatusRef))
                    {
                        return "برای سند تغییر کیفیت، وضعیت کیفیت مبدا و مقصد الزامی است.";
                    }

                    if (string.Equals(line.FromQualityStatusRef, line.ToQualityStatusRef, StringComparison.OrdinalIgnoreCase))
                    {
                        return "در سند تغییر کیفیت، وضعیت کیفیت مبدا و مقصد باید متفاوت باشند.";
                    }
                    break;
            }
        }

        return null;
    }

    private static string? ResolveSelectedDocumentId(string? documentId, IReadOnlyList<InventoryDocumentListItemModel> documents)
    {
        if (!string.IsNullOrWhiteSpace(documentId))
        {
            return documentId;
        }

        return documents.FirstOrDefault()?.DocumentBusinessKey;
    }

    private static DateTime? ParseDate(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        return DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out var parsed)
            ? parsed
            : null;
    }

    private static DateTime? ParseDateEndOfDay(string? value)
    {
        var date = ParseDate(value);
        return date?.Date.AddDays(1).AddTicks(-1);
    }

    private static CreateInventoryDocumentForm BuildCreateForm(string? warehouseId, string? sellerId)
    {
        return new CreateInventoryDocumentForm
        {
            WarehouseRef = warehouseId ?? string.Empty,
            SellerRef = sellerId ?? string.Empty,
            OccurredAt = DateTime.Now,
            Lines = new List<CreateInventoryDocumentLineForm>
            {
                new()
            }
        };
    }
}
