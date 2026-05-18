using System.Globalization;
using Insurance.InventoryDashboard.Panel.Models;
using Microsoft.AspNetCore.Mvc;

namespace Insurance.InventoryDashboard.Panel.Controllers;

public sealed partial class InventoryManagementController
{
    [HttpGet]
    public Task<IActionResult> Documents(
        string? documentId,
        string? editingLineId,
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
        => BuildDocumentPageAsync(
            "Documents",
            documentId,
            editingLineId,
            documentNo,
            documentType,
            status,
            warehouseId,
            sellerId,
            occurredFrom,
            occurredTo,
            page,
            pageSize,
            cancellationToken);

    [HttpGet("/InventoryManagement/Documents/Receipt")]
    public Task<IActionResult> ReceiptDocuments(
        string? documentId,
        string? editingLineId,
        string? documentNo,
        string? status,
        string? warehouseId,
        string? sellerId,
        string? occurredFrom,
        string? occurredTo,
        int page = 1,
        int pageSize = 10,
        CancellationToken cancellationToken = default)
        => BuildDocumentPageAsync(
            "ReceiptDocuments",
            documentId,
            editingLineId,
            documentNo,
            "Receipt",
            status,
            warehouseId,
            sellerId,
            occurredFrom,
            occurredTo,
            page,
            pageSize,
            cancellationToken);

    [HttpGet("/InventoryManagement/Documents/Issue")]
    public Task<IActionResult> IssueDocuments(
        string? documentId,
        string? editingLineId,
        string? documentNo,
        string? status,
        string? warehouseId,
        string? sellerId,
        string? occurredFrom,
        string? occurredTo,
        int page = 1,
        int pageSize = 10,
        CancellationToken cancellationToken = default)
        => BuildDocumentPageAsync(
            "IssueDocuments",
            documentId,
            editingLineId,
            documentNo,
            "Issue",
            status,
            warehouseId,
            sellerId,
            occurredFrom,
            occurredTo,
            page,
            pageSize,
            cancellationToken);

    [HttpGet("/InventoryManagement/Documents/Transfer")]
    public Task<IActionResult> TransferDocuments(
        string? documentId,
        string? editingLineId,
        string? documentNo,
        string? status,
        string? warehouseId,
        string? sellerId,
        string? occurredFrom,
        string? occurredTo,
        int page = 1,
        int pageSize = 10,
        CancellationToken cancellationToken = default)
        => BuildDocumentPageAsync(
            "TransferDocuments",
            documentId,
            editingLineId,
            documentNo,
            "Transfer",
            status,
            warehouseId,
            sellerId,
            occurredFrom,
            occurredTo,
            page,
            pageSize,
            cancellationToken);

    [HttpGet("/InventoryManagement/Documents/Adjustment")]
    public Task<IActionResult> AdjustmentDocuments(
        string? documentId,
        string? editingLineId,
        string? documentNo,
        string? status,
        string? warehouseId,
        string? sellerId,
        string? occurredFrom,
        string? occurredTo,
        int page = 1,
        int pageSize = 10,
        CancellationToken cancellationToken = default)
        => BuildDocumentPageAsync(
            "AdjustmentDocuments",
            documentId,
            editingLineId,
            documentNo,
            "Adjustment",
            status,
            warehouseId,
            sellerId,
            occurredFrom,
            occurredTo,
            page,
            pageSize,
            cancellationToken);

    [HttpGet("/InventoryManagement/Documents/Return")]
    public Task<IActionResult> ReturnDocuments(
        string? documentId,
        string? editingLineId,
        string? documentNo,
        string? status,
        string? warehouseId,
        string? sellerId,
        string? occurredFrom,
        string? occurredTo,
        int page = 1,
        int pageSize = 10,
        CancellationToken cancellationToken = default)
        => BuildDocumentPageAsync(
            "ReturnDocuments",
            documentId,
            editingLineId,
            documentNo,
            "Return",
            status,
            warehouseId,
            sellerId,
            occurredFrom,
            occurredTo,
            page,
            pageSize,
            cancellationToken);

    [HttpGet("/InventoryManagement/Documents/QualityChange")]
    public Task<IActionResult> QualityChangeDocuments(
        string? documentId,
        string? editingLineId,
        string? documentNo,
        string? status,
        string? warehouseId,
        string? sellerId,
        string? occurredFrom,
        string? occurredTo,
        int page = 1,
        int pageSize = 10,
        CancellationToken cancellationToken = default)
        => BuildDocumentPageAsync(
            "QualityChangeDocuments",
            documentId,
            editingLineId,
            documentNo,
            "QualityChange",
            status,
            warehouseId,
            sellerId,
            occurredFrom,
            occurredTo,
            page,
            pageSize,
            cancellationToken);

    [HttpGet("/InventoryManagement/Documents/Conversion")]
    public Task<IActionResult> ConversionDocuments(
        string? documentId,
        string? editingLineId,
        string? documentNo,
        string? status,
        string? warehouseId,
        string? sellerId,
        string? occurredFrom,
        string? occurredTo,
        int page = 1,
        int pageSize = 10,
        CancellationToken cancellationToken = default)
        => BuildDocumentPageAsync(
            "ConversionDocuments",
            documentId,
            editingLineId,
            documentNo,
            "Conversion",
            status,
            warehouseId,
            sellerId,
            occurredFrom,
            occurredTo,
            page,
            pageSize,
            cancellationToken);

    private async Task<IActionResult> BuildDocumentPageAsync(
        string documentAction,
        string? documentId,
        string? editingLineId,
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
        var selectedDocumentResult = new ApiResponse<InventoryDocumentDetailsModel> { IsSuccess = true };
        var resolvedDocumentType = NormalizeDocumentType(documentType);

        if (string.IsNullOrWhiteSpace(documentType) && !string.IsNullOrWhiteSpace(documentId))
        {
            selectedDocumentResult = await _apiService.GetInventoryDocumentByBusinessKeyAsync(documentId, token);
            resolvedDocumentType = NormalizeDocumentType(selectedDocumentResult.Data?.DocumentType);
        }

        var menu = ResolveMenu(modules, "document_management", ResolveDocumentMenuItemId(resolvedDocumentType));

        pageSize = NormalizePageSize(pageSize);
        var searchResult = await _apiService.SearchInventoryDocumentsAsync(
            token,
            documentNo,
            resolvedDocumentType,
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
        if (selectedDocumentResult.Data is null && !string.IsNullOrWhiteSpace(selectedDocumentId))
        {
            selectedDocumentResult = await _apiService.GetInventoryDocumentByBusinessKeyAsync(selectedDocumentId, token);
        }

        var model = new InventoryDocumentManagementPageViewModel
        {
            UserName = HttpContext.Session.GetString("UserName") ?? "کاربر",
            Roles = roles,
            Permissions = ResolvePermissionsFromSession(),
            Modules = modules,
            ActiveModule = menu.Module,
            ActiveItem = menu.Item,
            SelectedDocumentId = selectedDocumentId,
            EditingLineId = editingLineId,
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
            DocumentTypeFilter = resolvedDocumentType,
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
            CreateForm = BuildCreateForm(warehouseId, sellerId, resolvedDocumentType),
            LineForm = BuildLineForm(selectedDocumentResult.Data, selectedDocumentId, editingLineId, resolvedDocumentType)
        };

        ViewBag.DocumentAction = documentAction;
        SetLayoutViewBag(model.Modules, model.ActiveModule?.ModuleId, model.ActiveItem?.ItemId, model.UserName);
        return View(ResolveDocumentViewPath(documentAction), model);
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
            return RedirectToAction(
                ResolveDocumentRouteActionName(form.DocumentType),
                new { documentType = form.DocumentType, warehouseId = form.WarehouseRef, sellerId = form.SellerRef });
        }

        if (form.Lines.Count == 0)
        {
            TempData["CatalogError"] = "حداقل یک ردیف برای سند لازم است.";
            return RedirectToAction(
                ResolveDocumentRouteActionName(form.DocumentType),
                new { documentType = form.DocumentType, warehouseId = form.WarehouseRef, sellerId = form.SellerRef });
        }

        var ruleError = ValidateDocumentFormAgainstBackendRules(form);
        if (!string.IsNullOrWhiteSpace(ruleError))
        {
            TempData["CatalogError"] = ruleError;
            return RedirectToAction(
                ResolveDocumentRouteActionName(form.DocumentType),
                new { documentType = form.DocumentType, warehouseId = form.WarehouseRef, sellerId = form.SellerRef });
        }

        var variantLookupResult = await _apiService.SearchVariantsAsync(token, page: 1, pageSize: 2000);
        var variants = variantLookupResult.Data ?? new List<ProductVariantSummaryModel>();

        foreach (var line in form.Lines)
        {
            var variant = variants.FirstOrDefault(x => string.Equals(x.Id, line.VariantId, StringComparison.OrdinalIgnoreCase));
            if (variant is null)
            {
                TempData["CatalogError"] = "واریانت انتخاب شده معتبر نیست.";
                return RedirectToAction(
                    ResolveDocumentRouteActionName(form.DocumentType),
                    new { documentType = form.DocumentType, warehouseId = form.WarehouseRef, sellerId = form.SellerRef });
            }

            line.UomRef = variant.BaseUomRef;
            line.BaseUomRef = variant.BaseUomRef;
        }

        var result = await _apiService.CreateInventoryDocumentAsync(form, token);
        TempData[result.IsSuccess ? "CatalogSuccess" : "CatalogError"] =
            result.IsSuccess ? "سند موجودی با موفقیت ایجاد شد." : result.ErrorMessage ?? "ایجاد سند انجام نشد.";

        if (!result.IsSuccess || result.Data is null)
        {
            return RedirectToAction(
                ResolveDocumentRouteActionName(form.DocumentType),
                new { documentType = form.DocumentType, warehouseId = form.WarehouseRef, sellerId = form.SellerRef });
        }

        return RedirectToAction(
            ResolveDocumentRouteActionName(form.DocumentType),
            new { documentId = result.Data.DocumentId, documentType = form.DocumentType });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SaveDocumentLine(InventoryDocumentLineForm form)
    {
        if (!TryGetToken(out var token))
        {
            return RedirectToAction("Login", "Auth");
        }

        if (!IsAuthorizedFor(token, "Inventory.Document.Create", "InventoryDocument.Create", "Document.Create"))
        {
            TempData["CatalogError"] = "شما دسترسی ویرایش آیتم سند را ندارید.";
            return await RedirectToDocumentPageAsync(form.DocumentId, token);
        }

        if (!TryValidateModel(form))
        {
            TempData["CatalogError"] = ExtractModelError(ModelState);
            return await RedirectToDocumentPageAsync(form.DocumentId, token);
        }

        var lineResult = string.IsNullOrWhiteSpace(form.LineId)
            ? await _apiService.AddInventoryDocumentLineAsync(form.DocumentId, form, token)
            : await _apiService.UpdateInventoryDocumentLineAsync(form.DocumentId, form.LineId!, form, token);

        TempData[lineResult.IsSuccess ? "CatalogSuccess" : "CatalogError"] =
            lineResult.IsSuccess ? "آیتم سند با موفقیت ذخیره شد." : lineResult.ErrorMessage ?? "ذخیره آیتم سند انجام نشد.";

        return await RedirectToDocumentPageAsync(form.DocumentId, token);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteDocumentLine(string documentId, string lineId)
    {
        if (!TryGetToken(out var token))
        {
            return RedirectToAction("Login", "Auth");
        }

        if (!IsAuthorizedFor(token, "Inventory.Document.Create", "InventoryDocument.Create", "Document.Create"))
        {
            TempData["CatalogError"] = "شما دسترسی حذف آیتم سند را ندارید.";
            return await RedirectToDocumentPageAsync(documentId, token);
        }

        if (string.IsNullOrWhiteSpace(documentId) || string.IsNullOrWhiteSpace(lineId))
        {
            TempData["CatalogError"] = "آیتم سند برای حذف مشخص نشده است.";
            return await RedirectToDocumentPageAsync(documentId, token);
        }

        var result = await _apiService.DeleteInventoryDocumentLineAsync(documentId, lineId, token);
        TempData[result.IsSuccess ? "CatalogSuccess" : "CatalogError"] =
            result.IsSuccess ? "آیتم سند حذف شد." : result.ErrorMessage ?? "حذف آیتم سند انجام نشد.";
        return await RedirectToDocumentPageAsync(documentId, token);
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
        return await RedirectToDocumentPageAsync(documentId, token);
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
            return await RedirectToDocumentPageAsync(form.DocumentId, token);
        }

        if (!TryValidateModel(form))
        {
            TempData["CatalogError"] = ExtractModelError(ModelState);
            return await RedirectToDocumentPageAsync(form.DocumentId, token);
        }

        var result = await _apiService.RejectInventoryDocumentAsync(form.DocumentId, form.ReasonCode, token);
        TempData[result.IsSuccess ? "CatalogSuccess" : "CatalogError"] =
            result.IsSuccess ? "سند رد شد." : result.ErrorMessage ?? "رد سند انجام نشد.";
        return await RedirectToDocumentPageAsync(form.DocumentId, token);
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
            return await RedirectToDocumentPageAsync(form.DocumentId, token);
        }

        if (!TryValidateModel(form))
        {
            TempData["CatalogError"] = ExtractModelError(ModelState);
            return await RedirectToDocumentPageAsync(form.DocumentId, token);
        }

        var result = await _apiService.CancelInventoryDocumentAsync(form.DocumentId, form.ReasonCode, token);
        TempData[result.IsSuccess ? "CatalogSuccess" : "CatalogError"] =
            result.IsSuccess ? "سند لغو شد." : result.ErrorMessage ?? "لغو سند انجام نشد.";
        return await RedirectToDocumentPageAsync(form.DocumentId, token);
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
            return await RedirectToDocumentPageAsync(documentId, token);
        }

        var postedBy = HttpContext.Session.GetString("UserName") ?? "dashboard";
        var result = await _apiService.PostInventoryDocumentAsync(documentId, postedBy, token);
        TempData[result.IsSuccess ? "CatalogSuccess" : "CatalogError"] =
            result.IsSuccess ? "سند پست شد." : result.ErrorMessage ?? "پست سند انجام نشد.";
        return await RedirectToDocumentPageAsync(documentId, token);
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

                case "Conversion":
                    var hasSource = !string.IsNullOrWhiteSpace(line.SourceLocationRef);
                    var hasDestination = !string.IsNullOrWhiteSpace(line.DestinationLocationRef);

                    if (hasSource == hasDestination)
                    {
                        return "در سند تبدیل، هر ردیف باید یا خروجی باشد یا ورودی.";
                    }

                    if (string.IsNullOrWhiteSpace(line.QualityStatusRef))
                    {
                        return "در سند تبدیل، وضعیت کیفیت ردیف الزامی است.";
                    }
                    break;
            }
        }

        return null;
    }

    private async Task<IActionResult> RedirectToDocumentPageAsync(string documentId, string token)
    {
        var documentResult = await _apiService.GetInventoryDocumentByBusinessKeyAsync(documentId, token);
        var action = ResolveDocumentRouteActionName(documentResult.Data?.DocumentType);
        return RedirectToAction(action, new { documentId, documentType = documentResult.Data?.DocumentType });
    }

    private static string ResolveDocumentRouteActionName(string? documentType)
    {
        return documentType switch
        {
            "Receipt" => "ReceiptDocuments",
            "Issue" => "IssueDocuments",
            "Transfer" => "TransferDocuments",
            "Adjustment" => "AdjustmentDocuments",
            "Return" => "ReturnDocuments",
            "QualityChange" => "QualityChangeDocuments",
            "Conversion" => "ConversionDocuments",
            _ => nameof(Documents)
        };
    }

    private static string ResolveDocumentViewPath(string documentAction)
    {
        return documentAction switch
        {
            "ReceiptDocuments" => "~/Views/InventoryManagement/ReceiptDocuments.cshtml",
            "IssueDocuments" => "~/Views/InventoryManagement/IssueDocuments.cshtml",
            "TransferDocuments" => "~/Views/InventoryManagement/TransferDocuments.cshtml",
            "AdjustmentDocuments" => "~/Views/InventoryManagement/AdjustmentDocuments.cshtml",
            "ReturnDocuments" => "~/Views/InventoryManagement/ReturnDocuments.cshtml",
            "QualityChangeDocuments" => "~/Views/InventoryManagement/QualityChangeDocuments.cshtml",
            "ConversionDocuments" => "~/Views/InventoryManagement/ConversionDocuments.cshtml",
            _ => "~/Views/InventoryManagement/Documents.cshtml"
        };
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

    private static string NormalizeDocumentType(string? documentType)
    {
        return documentType switch
        {
            "Receipt" or "Issue" or "Transfer" or "Adjustment" or "Return" or "QualityChange" or "Conversion" => documentType,
            _ => "Receipt"
        };
    }

    private static string ResolveDocumentMenuItemId(string documentType)
    {
        return documentType switch
        {
            "Issue" => "issue_documents",
            "Transfer" => "transfer_documents",
            "Adjustment" => "adjustment_documents",
            "Return" => "return_documents",
            "QualityChange" => "quality_change_documents",
            "Conversion" => "conversion_documents",
            _ => "receipt_documents"
        };
    }

    private static CreateInventoryDocumentForm BuildCreateForm(string? warehouseId, string? sellerId, string documentType)
    {
        return new CreateInventoryDocumentForm
        {
            DocumentType = documentType,
            WarehouseRef = warehouseId ?? string.Empty,
            SellerRef = sellerId ?? string.Empty,
            OccurredAt = DateTime.Now,
            Lines = new List<CreateInventoryDocumentLineForm>
            {
                new()
            }
        };
    }

    private static InventoryDocumentLineForm BuildLineForm(
        InventoryDocumentDetailsModel? document,
        string? selectedDocumentId,
        string? editingLineId,
        string documentType)
    {
        var line = document?.Lines.FirstOrDefault(x => string.Equals(x.LineBusinessKey, editingLineId, StringComparison.OrdinalIgnoreCase));
        if (line is null)
        {
            return new InventoryDocumentLineForm
            {
                DocumentId = selectedDocumentId ?? string.Empty,
                DocumentType = documentType,
                Qty = 1m
            };
        }

        return new InventoryDocumentLineForm
        {
            DocumentId = selectedDocumentId ?? string.Empty,
            DocumentType = documentType,
            LineId = line.LineBusinessKey,
            VariantId = line.VariantRef,
            Qty = line.Qty,
            UomRef = line.UomRef,
            BaseUomRef = line.BaseUomRef,
            SourceLocationRef = line.SourceLocationRef,
            DestinationLocationRef = line.DestinationLocationRef,
            QualityStatusRef = line.QualityStatusRef,
            FromQualityStatusRef = line.FromQualityStatusRef,
            ToQualityStatusRef = line.ToQualityStatusRef,
            LotBatchNo = line.LotBatchNo,
            ReasonCode = line.ReasonCode,
            AdjustmentDirection = line.AdjustmentDirection
        };
    }
}
