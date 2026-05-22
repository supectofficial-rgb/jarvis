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
            null,
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

    [HttpGet("/InventoryManagement/Documents/List")]
    public async Task<IActionResult> DocumentList(
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

        if (!IsAuthorizedFor(token, "Inventory.Document.View", "Inventory.Document.Search", "InventoryDocument.Read", "InventoryDocument.Search", "Document.Read"))
        {
            return StatusCode(403, new { isSuccess = false, errorMessage = "شما دسترسی مشاهده اسناد را ندارید." });
        }

        pageSize = NormalizePageSize(pageSize);
        var resolvedDocumentType = NormalizeDocumentType(documentType);
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

        var data = searchResult.Data ?? new InventoryDocumentSearchResultModel
        {
            Page = Math.Max(page, 1),
            PageSize = pageSize,
            TotalCount = 0,
            Items = new List<InventoryDocumentListItemModel>()
        };

        return Json(new
        {
            data.Items,
            data.Page,
            data.PageSize,
            data.TotalCount,
            TotalPages = CalculateTotalPages(data.TotalCount, data.PageSize)
        });
    }

    [HttpGet("/InventoryManagement/Documents/Receipt")]
    public Task<IActionResult> ReceiptDocuments(
        string? documentId,
        string? editingLineId,
        string? tab,
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
            tab,
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

    [HttpGet("/InventoryManagement/Documents/Receipt/Details")]
    public async Task<IActionResult> ReceiptDocumentDetails(
        string documentId,
        CancellationToken cancellationToken = default)
    {
        if (!TryGetToken(out var token))
        {
            return RedirectToAction("Login", "Auth");
        }

        if (!IsAuthorizedFor(token, "Inventory.Document.View", "Inventory.Document.Search", "InventoryDocument.Read", "InventoryDocument.Search", "Document.Read"))
        {
            return Content("شما دسترسی مشاهده اسناد موجودی را ندارید.");
        }

        var model = await BuildReceiptDocumentDetailsModalModelAsync(documentId, token, cancellationToken);
        return PartialView("~/Views/InventoryManagement/_ReceiptDocumentDetailsModalBody.cshtml", model);
    }

    [HttpGet("/InventoryManagement/Documents/Receipt/VariantLookup")]
    public async Task<IActionResult> SearchReceiptVariantLookup(string? term, CancellationToken cancellationToken = default)
    {
        if (!TryGetToken(out var token))
        {
            return Json(new
            {
                isSuccess = false,
                errorMessage = "نشست کاربری منقضی شده است. لطفا دوباره وارد شوید."
            });
        }

        if (!IsAuthorizedFor(token, "Inventory.Document.View", "Inventory.Document.Search", "InventoryDocument.Read", "InventoryDocument.Search", "Document.Read"))
        {
            return Json(new
            {
                isSuccess = false,
                errorMessage = "شما دسترسی جستجوی واریانت را ندارید."
            });
        }

        var result = await _apiService.SearchProductVariantsAsync(
            token,
            searchTerm: term,
            isActive: true,
            page: 1,
            pageSize: 20);

        if (!result.IsSuccess)
        {
            return Json(new
            {
                isSuccess = false,
                errorMessage = result.ErrorMessage
            });
        }

        return Json(new
        {
            isSuccess = true,
            items = (result.Data?.Items ?? new List<ProductVariantSummaryModel>())
                .Select(x => new
                {
                    id = x.Id,
                    text = BuildVariantLookupLabel(x),
                    sku = x.Sku,
                    name = x.Name,
                    barcode = x.Barcode
                })
                .ToList()
        });
    }

    [HttpGet("/InventoryManagement/Documents/Receipt/VariantInventoryLookup")]
    public async Task<IActionResult> SearchReceiptVariantInventoryLookup(string variantId, CancellationToken cancellationToken = default)
    {
        if (!TryGetToken(out var token))
        {
            return Json(new
            {
                isSuccess = false,
                errorMessage = "نشست کاربری منقضی شده است. لطفا دوباره وارد شوید."
            });
        }

        if (!Guid.TryParse(variantId, out var parsedVariantId))
        {
            return Json(new
            {
                isSuccess = false,
                errorMessage = "واریانت انتخاب‌شده معتبر نیست."
            });
        }

        if (!IsAuthorizedFor(token, "Inventory.Document.View", "Inventory.Document.Search", "InventoryDocument.Read", "InventoryDocument.Search", "Document.Read"))
        {
            return Json(new
            {
                isSuccess = false,
                errorMessage = "شما دسترسی مشاهده انبارهای واریانت را ندارید."
            });
        }

        var warehouseLookupResult = await _apiService.GetWarehouseLookupAsync(token, includeInactive: true);
        var locationLookupResult = await _apiService.GetLocationLookupAsync(token, warehouseId: null, includeInactive: true);

        if (!warehouseLookupResult.IsSuccess || !locationLookupResult.IsSuccess)
        {
            return Json(new
            {
                isSuccess = false,
                errorMessage = JoinErrors(
                    warehouseLookupResult.ErrorMessage,
                    locationLookupResult.ErrorMessage)
            });
        }

        var warehouses = (warehouseLookupResult.Data ?? new List<WarehouseLookupItemModel>())
            .OrderBy(x => x.Code)
            .ThenBy(x => x.Name)
            .Select(x => new
            {
                id = x.WarehouseBusinessKey,
                warehouseId = x.WarehouseBusinessKey,
                text = $"{x.Code} - {x.Name}",
                code = x.Code,
                name = x.Name
            })
            .ToList();

        var warehouseMap = warehouses.ToDictionary(x => x.warehouseId, x => x, StringComparer.OrdinalIgnoreCase);

        var locations = (locationLookupResult.Data ?? new List<LocationLookupItemModel>())
            .OrderBy(x => x.WarehouseRef)
            .ThenBy(x => x.LocationCode)
            .ThenBy(x => x.LocationType)
            .ThenBy(x => x.WarehouseRef)
            .Select(x =>
            {
                var warehouse = warehouseMap.TryGetValue(x.WarehouseRef, out var warehouseItem) ? warehouseItem : null;
                var warehouseLabel = warehouse is null ? x.WarehouseRef : warehouse.text;
                var locationLabel = string.IsNullOrWhiteSpace(x.LocationType)
                    ? x.LocationCode
                    : $"{x.LocationCode} ({x.LocationType})";

                return new
                {
                    id = x.LocationBusinessKey,
                    locationId = x.LocationBusinessKey,
                    warehouseId = x.WarehouseRef,
                    warehouseText = warehouseLabel,
                    locationText = locationLabel,
                    text = $"{warehouseLabel} - {locationLabel}",
                    code = x.LocationCode,
                    type = x.LocationType
                };
            })
            .ToList();

        return Json(new
        {
            isSuccess = true,
            warehouses,
            locations
        });
    }

    [HttpGet("/InventoryManagement/Documents/Transfer/Details")]
    public async Task<IActionResult> TransferDocumentDetails(
        string documentId,
        CancellationToken cancellationToken = default)
    {
        if (!TryGetToken(out var token))
        {
            return RedirectToAction("Login", "Auth");
        }

        if (!IsAuthorizedFor(token, "Inventory.Document.View", "Inventory.Document.Search", "InventoryDocument.Read", "InventoryDocument.Search", "Document.Read"))
        {
            return Content("شما دسترسی مشاهده اسناد موجودی را ندارید.");
        }

        var model = await BuildTransferDocumentDetailsModalModelAsync(documentId, token, cancellationToken);
        return PartialView("~/Views/InventoryManagement/_TransferDocumentDetailsModalBody.cshtml", model);
    }

    [HttpGet("/InventoryManagement/Documents/Transfer/VariantInventoryLookup")]
    public async Task<IActionResult> SearchTransferVariantInventoryLookup(string variantId, CancellationToken cancellationToken = default)
    {
        if (!TryGetToken(out var token))
        {
            return Json(new
            {
                isSuccess = false,
                errorMessage = "نشست کاربری منقضی شده است. لطفا دوباره وارد شوید."
            });
        }

        if (!Guid.TryParse(variantId, out _))
        {
            return Json(new
            {
                isSuccess = false,
                errorMessage = "واریانت انتخاب‌شده معتبر نیست."
            });
        }

        var parsedVariantId = Guid.Parse(variantId);

        if (!IsAuthorizedFor(token, "Inventory.Document.View", "Inventory.Document.Search", "InventoryDocument.Read", "InventoryDocument.Search", "Document.Read"))
        {
            return Json(new
            {
                isSuccess = false,
                errorMessage = "شما دسترسی مشاهده انبارهای واریانت را ندارید."
            });
        }

        var stockBucketsResult = await _apiService.GetAvailableStockBucketsAsync(token, variantRef: parsedVariantId, minQuantity: 0);

        if (!stockBucketsResult.IsSuccess)
        {
            return Json(new
            {
                isSuccess = false,
                errorMessage = stockBucketsResult.ErrorMessage
            });
        }

        var allowedLocationIds = (stockBucketsResult.Data?.Items ?? new List<StockDetailBucketModel>())
            .Where(x => x.QuantityOnHand > 0)
            .Select(x => x.LocationRef.ToString("D"))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        return Json(new
        {
            isSuccess = true,
            allowedLocationIds
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SaveReceiptDocumentLine([Bind(Prefix = "LineForm")] InventoryDocumentLineForm form, CancellationToken cancellationToken = default)
    {
        if (!TryGetToken(out var token))
        {
            return RedirectToAction("Login", "Auth");
        }

        if (!IsAuthorizedFor(token, "Inventory.Document.Create", "InventoryDocument.Create", "Document.Create"))
        {
            return Content("شما دسترسی ویرایش آیتم سند را ندارید.");
        }

        if (!TryValidateModel(form))
        {
            var invalidModel = await BuildReceiptDocumentDetailsModalModelAsync(form.DocumentId, token);
            invalidModel.ErrorMessage = ExtractModelError(ModelState);
            invalidModel.LineForm = form;
            return PartialView("~/Views/InventoryManagement/_ReceiptDocumentDetailsModalBody.cshtml", invalidModel);
        }

        var documentResult = await _apiService.GetInventoryDocumentByBusinessKeyAsync(form.DocumentId, token);
        var document = documentResult.Data;
        if (document is null)
        {
            var notFoundModel = await BuildReceiptDocumentDetailsModalModelAsync(form.DocumentId, token);
            notFoundModel.ErrorMessage = documentResult.ErrorMessage ?? "سند یافت نشد.";
            notFoundModel.LineForm = form;
            return PartialView("~/Views/InventoryManagement/_ReceiptDocumentDetailsModalBody.cshtml", notFoundModel);
        }

        var variantsResult = await _apiService.SearchVariantsAsync(token, page: 1, pageSize: 2000);
        var variants = variantsResult.Data ?? new List<ProductVariantSummaryModel>();
        var variant = variants.FirstOrDefault(x => string.Equals(x.Id, form.VariantId, StringComparison.OrdinalIgnoreCase));
        if (variant is null)
        {
            var invalidVariantModel = await BuildReceiptDocumentDetailsModalModelAsync(form.DocumentId, token);
            invalidVariantModel.ErrorMessage = "واریانت انتخاب شده معتبر نیست.";
            invalidVariantModel.LineForm = form;
            return PartialView("~/Views/InventoryManagement/_ReceiptDocumentDetailsModalBody.cshtml", invalidVariantModel);
        }

        form.UomRef = variant.BaseUomRef;
        form.BaseUomRef = variant.BaseUomRef;

        if (string.IsNullOrWhiteSpace(form.DestinationLocationRef))
        {
            form.DestinationLocationRef = await ResolveReceiptDefaultLocationRefAsync(document.WarehouseRef, token, cancellationToken);
        }

        var result = string.IsNullOrWhiteSpace(form.LineId)
            ? await _apiService.AddInventoryDocumentLineAsync(form.DocumentId, form, token)
            : await _apiService.UpdateInventoryDocumentLineAsync(form.DocumentId, form.LineId!, form, token);

        var refreshedModel = await BuildReceiptDocumentDetailsModalModelAsync(form.DocumentId, token, cancellationToken);
        refreshedModel.ErrorMessage = result.IsSuccess ? null : result.ErrorMessage ?? "ذخیره آیتم سند انجام نشد.";
        return PartialView("~/Views/InventoryManagement/_ReceiptDocumentDetailsModalBody.cshtml", refreshedModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SaveTransferDocumentLine([Bind(Prefix = "LineForm")] InventoryDocumentLineForm form, CancellationToken cancellationToken = default)
    {
        if (!TryGetToken(out var token))
        {
            return RedirectToAction("Login", "Auth");
        }

        if (!IsAuthorizedFor(token, "Inventory.Document.Create", "InventoryDocument.Create", "Document.Create"))
        {
            return Content("شما دسترسی ویرایش آیتم سند را ندارید.");
        }

        if (!TryValidateModel(form))
        {
            var invalidModel = await BuildTransferDocumentDetailsModalModelAsync(form.DocumentId, token);
            invalidModel.ErrorMessage = ExtractModelError(ModelState);
            invalidModel.LineForm = form;
            return PartialView("~/Views/InventoryManagement/_TransferDocumentDetailsModalBody.cshtml", invalidModel);
        }

        var documentResult = await _apiService.GetInventoryDocumentByBusinessKeyAsync(form.DocumentId, token);
        var document = documentResult.Data;
        if (document is null)
        {
            var notFoundModel = await BuildTransferDocumentDetailsModalModelAsync(form.DocumentId, token);
            notFoundModel.ErrorMessage = documentResult.ErrorMessage ?? "سند یافت نشد.";
            notFoundModel.LineForm = form;
            return PartialView("~/Views/InventoryManagement/_TransferDocumentDetailsModalBody.cshtml", notFoundModel);
        }

        var variantsResult = await _apiService.SearchVariantsAsync(token, page: 1, pageSize: 2000);
        var variants = variantsResult.Data ?? new List<ProductVariantSummaryModel>();
        var variant = variants.FirstOrDefault(x => string.Equals(x.Id, form.VariantId, StringComparison.OrdinalIgnoreCase));
        if (variant is null)
        {
            var invalidVariantModel = await BuildTransferDocumentDetailsModalModelAsync(form.DocumentId, token);
            invalidVariantModel.ErrorMessage = "واریانت انتخاب شده معتبر نیست.";
            invalidVariantModel.LineForm = form;
            return PartialView("~/Views/InventoryManagement/_TransferDocumentDetailsModalBody.cshtml", invalidVariantModel);
        }

        form.UomRef = variant.BaseUomRef;
        form.BaseUomRef = variant.BaseUomRef;

        if (string.IsNullOrWhiteSpace(form.SourceLocationRef))
        {
            var invalidLocationModel = await BuildTransferDocumentDetailsModalModelAsync(form.DocumentId, token);
            invalidLocationModel.ErrorMessage = "برای سند انتقال، لوکیشن مبدا الزامی است.";
            invalidLocationModel.LineForm = form;
            return PartialView("~/Views/InventoryManagement/_TransferDocumentDetailsModalBody.cshtml", invalidLocationModel);
        }

        if (string.IsNullOrWhiteSpace(form.DestinationLocationRef))
        {
            var invalidLocationModel = await BuildTransferDocumentDetailsModalModelAsync(form.DocumentId, token);
            invalidLocationModel.ErrorMessage = "برای سند انتقال، لوکیشن مقصد الزامی است.";
            invalidLocationModel.LineForm = form;
            return PartialView("~/Views/InventoryManagement/_TransferDocumentDetailsModalBody.cshtml", invalidLocationModel);
        }

        if (string.Equals(form.SourceLocationRef, form.DestinationLocationRef, StringComparison.OrdinalIgnoreCase))
        {
            var invalidLocationModel = await BuildTransferDocumentDetailsModalModelAsync(form.DocumentId, token);
            invalidLocationModel.ErrorMessage = "لوکیشن مبدأ و مقصد باید متفاوت باشند.";
            invalidLocationModel.LineForm = form;
            return PartialView("~/Views/InventoryManagement/_TransferDocumentDetailsModalBody.cshtml", invalidLocationModel);
        }

        var result = string.IsNullOrWhiteSpace(form.LineId)
            ? await _apiService.AddInventoryDocumentLineAsync(form.DocumentId, form, token)
            : await _apiService.UpdateInventoryDocumentLineAsync(form.DocumentId, form.LineId!, form, token);

        var refreshedModel = await BuildTransferDocumentDetailsModalModelAsync(form.DocumentId, token, cancellationToken);
        refreshedModel.ErrorMessage = result.IsSuccess ? null : result.ErrorMessage ?? "ذخیره آیتم سند انجام نشد.";
        return PartialView("~/Views/InventoryManagement/_TransferDocumentDetailsModalBody.cshtml", refreshedModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteReceiptDocumentLine(string documentId, string lineId, CancellationToken cancellationToken = default)
    {
        if (!TryGetToken(out var token))
        {
            return RedirectToAction("Login", "Auth");
        }

        if (!IsAuthorizedFor(token, "Inventory.Document.Create", "InventoryDocument.Create", "Document.Create"))
        {
            return Content("شما دسترسی حذف آیتم سند را ندارید.");
        }

        if (string.IsNullOrWhiteSpace(documentId) || string.IsNullOrWhiteSpace(lineId))
        {
            var invalidModel = await BuildReceiptDocumentDetailsModalModelAsync(documentId, token);
            invalidModel.ErrorMessage = "آیتم سند برای حذف مشخص نشده است.";
            return PartialView("~/Views/InventoryManagement/_ReceiptDocumentDetailsModalBody.cshtml", invalidModel);
        }

        var result = await _apiService.DeleteInventoryDocumentLineAsync(documentId, lineId, token);
        var refreshedModel = await BuildReceiptDocumentDetailsModalModelAsync(documentId, token);
        refreshedModel.ErrorMessage = result.IsSuccess ? null : result.ErrorMessage ?? "حذف آیتم سند انجام نشد.";
        return PartialView("~/Views/InventoryManagement/_ReceiptDocumentDetailsModalBody.cshtml", refreshedModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteTransferDocumentLine(string documentId, string lineId, CancellationToken cancellationToken = default)
    {
        if (!TryGetToken(out var token))
        {
            return RedirectToAction("Login", "Auth");
        }

        if (!IsAuthorizedFor(token, "Inventory.Document.Create", "InventoryDocument.Create", "Document.Create"))
        {
            return Content("شما دسترسی حذف آیتم سند را ندارید.");
        }

        if (string.IsNullOrWhiteSpace(documentId) || string.IsNullOrWhiteSpace(lineId))
        {
            var invalidModel = await BuildTransferDocumentDetailsModalModelAsync(documentId, token);
            invalidModel.ErrorMessage = "آیتم سند برای حذف مشخص نشده است.";
            return PartialView("~/Views/InventoryManagement/_TransferDocumentDetailsModalBody.cshtml", invalidModel);
        }

        var result = await _apiService.DeleteInventoryDocumentLineAsync(documentId, lineId, token);
        var refreshedModel = await BuildTransferDocumentDetailsModalModelAsync(documentId, token);
        refreshedModel.ErrorMessage = result.IsSuccess ? null : result.ErrorMessage ?? "حذف آیتم سند انجام نشد.";
        return PartialView("~/Views/InventoryManagement/_TransferDocumentDetailsModalBody.cshtml", refreshedModel);
    }

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
            null,
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

    [HttpGet("/InventoryManagement/Documents/Issue/Details")]
    public async Task<IActionResult> IssueDocumentDetails(
        string documentId,
        CancellationToken cancellationToken = default)
    {
        if (!TryGetToken(out var token))
        {
            return RedirectToAction("Login", "Auth");
        }

        if (!IsAuthorizedFor(token, "Inventory.Document.View", "Inventory.Document.Search", "InventoryDocument.Read", "InventoryDocument.Search", "Document.Read"))
        {
            return Content("شما دسترسی مشاهده اسناد موجودی را ندارید.");
        }

        var model = await BuildIssueDocumentDetailsModalModelAsync(documentId, token, cancellationToken);
        return PartialView("~/Views/InventoryManagement/_IssueDocumentDetailsModalBody.cshtml", model);
    }

    [HttpGet("/InventoryManagement/Documents/Issue/VariantInventoryLookup")]
    public async Task<IActionResult> SearchIssueVariantInventoryLookup(string variantId, CancellationToken cancellationToken = default)
    {
        if (!TryGetToken(out var token))
        {
            return Json(new
            {
                isSuccess = false,
                errorMessage = "نشست کاربری منقضی شده است. لطفا دوباره وارد شوید."
            });
        }

        if (!Guid.TryParse(variantId, out var parsedVariantId))
        {
            return Json(new
            {
                isSuccess = false,
                errorMessage = "واریانت انتخاب‌شده معتبر نیست."
            });
        }

        if (!IsAuthorizedFor(token, "Inventory.Document.View", "Inventory.Document.Search", "InventoryDocument.Read", "InventoryDocument.Search", "Document.Read"))
        {
            return Json(new
            {
                isSuccess = false,
                errorMessage = "شما دسترسی مشاهده انبارهای واریانت را ندارید."
            });
        }

        var stockBucketsResult = await _apiService.GetAvailableStockBucketsAsync(token, variantRef: parsedVariantId, minQuantity: 0);
        if (!stockBucketsResult.IsSuccess)
        {
            return Json(new
            {
                isSuccess = false,
                errorMessage = stockBucketsResult.ErrorMessage
            });
        }

        var allowedLocationIds = (stockBucketsResult.Data?.Items ?? new List<StockDetailBucketModel>())
            .Where(x => x.QuantityOnHand > 0)
            .Select(x => x.LocationRef.ToString("D"))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        return Json(new
        {
            isSuccess = true,
            allowedLocationIds
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SaveIssueDocumentLine([Bind(Prefix = "LineForm")] InventoryDocumentLineForm form, CancellationToken cancellationToken = default)
    {
        if (!TryGetToken(out var token))
        {
            return RedirectToAction("Login", "Auth");
        }

        if (!IsAuthorizedFor(token, "Inventory.Document.Create", "InventoryDocument.Create", "Document.Create"))
        {
            return Content("شما دسترسی ویرایش آیتم سند را ندارید.");
        }

        if (!TryValidateModel(form))
        {
            var invalidModel = await BuildIssueDocumentDetailsModalModelAsync(form.DocumentId, token);
            invalidModel.ErrorMessage = ExtractModelError(ModelState);
            invalidModel.LineForm = form;
            return PartialView("~/Views/InventoryManagement/_IssueDocumentDetailsModalBody.cshtml", invalidModel);
        }

        var documentResult = await _apiService.GetInventoryDocumentByBusinessKeyAsync(form.DocumentId, token);
        var document = documentResult.Data;
        if (document is null)
        {
            var notFoundModel = await BuildIssueDocumentDetailsModalModelAsync(form.DocumentId, token);
            notFoundModel.ErrorMessage = documentResult.ErrorMessage ?? "سند یافت نشد.";
            notFoundModel.LineForm = form;
            return PartialView("~/Views/InventoryManagement/_IssueDocumentDetailsModalBody.cshtml", notFoundModel);
        }

        var variantsResult = await _apiService.SearchVariantsAsync(token, page: 1, pageSize: 2000);
        var variants = variantsResult.Data ?? new List<ProductVariantSummaryModel>();
        var variant = variants.FirstOrDefault(x => string.Equals(x.Id, form.VariantId, StringComparison.OrdinalIgnoreCase));
        if (variant is null)
        {
            var invalidVariantModel = await BuildIssueDocumentDetailsModalModelAsync(form.DocumentId, token);
            invalidVariantModel.ErrorMessage = "واریانت انتخاب شده معتبر نیست.";
            invalidVariantModel.LineForm = form;
            return PartialView("~/Views/InventoryManagement/_IssueDocumentDetailsModalBody.cshtml", invalidVariantModel);
        }

        form.UomRef = variant.BaseUomRef;
        form.BaseUomRef = variant.BaseUomRef;

        if (string.IsNullOrWhiteSpace(form.SourceLocationRef))
        {
            var invalidLocationModel = await BuildIssueDocumentDetailsModalModelAsync(form.DocumentId, token);
            invalidLocationModel.ErrorMessage = "برای سند حواله، لوکیشن مبدأ الزامی است.";
            invalidLocationModel.LineForm = form;
            return PartialView("~/Views/InventoryManagement/_IssueDocumentDetailsModalBody.cshtml", invalidLocationModel);
        }

        var result = string.IsNullOrWhiteSpace(form.LineId)
            ? await _apiService.AddInventoryDocumentLineAsync(form.DocumentId, form, token)
            : await _apiService.UpdateInventoryDocumentLineAsync(form.DocumentId, form.LineId!, form, token);

        var refreshedModel = await BuildIssueDocumentDetailsModalModelAsync(form.DocumentId, token, cancellationToken);
        refreshedModel.ErrorMessage = result.IsSuccess ? null : result.ErrorMessage ?? "ذخیره آیتم سند انجام نشد.";
        return PartialView("~/Views/InventoryManagement/_IssueDocumentDetailsModalBody.cshtml", refreshedModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteIssueDocumentLine(string documentId, string lineId, CancellationToken cancellationToken = default)
    {
        if (!TryGetToken(out var token))
        {
            return RedirectToAction("Login", "Auth");
        }

        if (!IsAuthorizedFor(token, "Inventory.Document.Create", "InventoryDocument.Create", "Document.Create"))
        {
            return Content("شما دسترسی حذف آیتم سند را ندارید.");
        }

        if (string.IsNullOrWhiteSpace(documentId) || string.IsNullOrWhiteSpace(lineId))
        {
            var invalidModel = await BuildIssueDocumentDetailsModalModelAsync(documentId, token);
            invalidModel.ErrorMessage = "آیتم سند برای حذف مشخص نشده است.";
            return PartialView("~/Views/InventoryManagement/_IssueDocumentDetailsModalBody.cshtml", invalidModel);
        }

        var result = await _apiService.DeleteInventoryDocumentLineAsync(documentId, lineId, token);
        var refreshedModel = await BuildIssueDocumentDetailsModalModelAsync(documentId, token);
        refreshedModel.ErrorMessage = result.IsSuccess ? null : result.ErrorMessage ?? "حذف آیتم سند انجام نشد.";
        return PartialView("~/Views/InventoryManagement/_IssueDocumentDetailsModalBody.cshtml", refreshedModel);
    }

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
            null,
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
            null,
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
            null,
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
            null,
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
            null,
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
        string? tab,
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
            SelectedTab = tab,
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
            CreateForm = BuildCreateForm(
                selectedDocumentResult.Data,
                warehouseId,
                sellerId,
                resolvedDocumentType,
                tab),
            LineForm = BuildLineForm(selectedDocumentResult.Data, selectedDocumentId, editingLineId, resolvedDocumentType)
        };

        ViewBag.DocumentAction = documentAction;
        SetLayoutViewBag(model.Modules, model.ActiveModule?.ModuleId, model.ActiveItem?.ItemId, model.UserName);
        return View(ResolveDocumentViewPath(documentAction), model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SaveDocument([Bind(Prefix = "CreateForm")] CreateInventoryDocumentForm form)
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

        var isUpdate = !string.IsNullOrWhiteSpace(form.DocumentId);
        var isReceiptDocument = string.Equals(form.DocumentType, "Receipt", StringComparison.OrdinalIgnoreCase);
        form.Lines ??= new List<CreateInventoryDocumentLineForm>();

        if (isUpdate)
        {
            form.Lines.Clear();
        }
        else
        {
            form.Lines = form.Lines
                .Where(line =>
                    !string.IsNullOrWhiteSpace(line.VariantId) ||
                    line.Qty > 0 ||
                    !string.IsNullOrWhiteSpace(line.SourceLocationRef) ||
                    !string.IsNullOrWhiteSpace(line.DestinationLocationRef))
                .ToList();
        }

        if (!TryValidateModel(form))
        {
            TempData["CatalogError"] = ExtractModelError(ModelState);
            return RedirectToAction(
                ResolveDocumentRouteActionName(form.DocumentType),
                new { documentType = form.DocumentType, warehouseId = form.WarehouseRef, documentId = form.DocumentId, tab = "create" });
        }

        if (!isUpdate)
        {
            if (!isReceiptDocument)
            {
                var ruleError = ValidateDocumentFormAgainstBackendRules(form);
                if (!string.IsNullOrWhiteSpace(ruleError))
                {
                    TempData["CatalogError"] = ruleError;
                    return RedirectToAction(
                        ResolveDocumentRouteActionName(form.DocumentType),
                        new { documentType = form.DocumentType, warehouseId = form.WarehouseRef, tab = "create" });
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
                            new { documentType = form.DocumentType, warehouseId = form.WarehouseRef, tab = "create" });
                    }

                    line.UomRef = variant.BaseUomRef;
                    line.BaseUomRef = variant.BaseUomRef;
                }
            }

            var result = await _apiService.CreateInventoryDocumentAsync(form, token);
            TempData[result.IsSuccess ? "CatalogSuccess" : "CatalogError"] =
                result.IsSuccess ? "سند موجودی با موفقیت ایجاد شد." : result.ErrorMessage ?? "ایجاد سند انجام نشد.";

            return RedirectToAction(
                ResolveDocumentRouteActionName(form.DocumentType),
                new { documentType = form.DocumentType, warehouseId = form.WarehouseRef, tab = "create" });
        }

        var updateResult = await _apiService.UpdateInventoryDocumentAsync(form, token);
        TempData[updateResult.IsSuccess ? "CatalogSuccess" : "CatalogError"] =
            updateResult.IsSuccess ? "اطلاعات عمومی سند با موفقیت به‌روزرسانی شد." : updateResult.ErrorMessage ?? "به‌روزرسانی سند انجام نشد.";

        return RedirectToAction(
            ResolveDocumentRouteActionName(form.DocumentType),
            new { documentId = form.DocumentId, documentType = form.DocumentType, tab = "create" });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SaveDocumentLine([Bind(Prefix = "LineForm")] InventoryDocumentLineForm form)
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
    public async Task<IActionResult> DeleteDocument(
        string documentId,
        string? documentType,
        string? documentNo,
        string? status,
        string? warehouseId,
        string? sellerId,
        string? occurredFrom,
        string? occurredTo,
        int page = 1,
        int pageSize = 10)
    {
        if (!TryGetToken(out var token))
        {
            return RedirectToAction("Login", "Auth");
        }

        if (!IsAuthorizedFor(token, "Inventory.Document.Delete", "InventoryDocument.Delete", "Document.Delete"))
        {
            TempData["CatalogError"] = "شما دسترسی حذف سند را ندارید.";
            return RedirectToAction(
                ResolveDocumentRouteActionName(NormalizeDocumentType(documentType)),
                new
                {
                    documentId,
                    documentType = NormalizeDocumentType(documentType),
                    documentNo,
                    status,
                    warehouseId,
                    sellerId,
                    occurredFrom,
                    occurredTo,
                    page,
                    pageSize,
                    tab = "list"
                });
        }

        if (string.IsNullOrWhiteSpace(documentId))
        {
            TempData["CatalogError"] = "سند برای حذف مشخص نشده است.";
            return RedirectToAction(nameof(ReceiptDocuments), new { tab = "list" });
        }

        var documentResult = await _apiService.GetInventoryDocumentByBusinessKeyAsync(documentId, token);
        var document = documentResult.Data;
        if (document is null)
        {
            TempData["CatalogError"] = documentResult.ErrorMessage ?? "سند یافت نشد.";
            return RedirectToAction(nameof(ReceiptDocuments), new { tab = "list" });
        }

        if (!string.Equals(document.Status, "Draft", StringComparison.OrdinalIgnoreCase))
        {
            TempData["CatalogError"] = "فقط اسناد پیش‌نویس قابل حذف هستند.";
            return await RedirectToDocumentPageAsync(documentId, token);
        }

        var deleteResult = await _apiService.DeleteInventoryDocumentAsync(documentId, token);
        TempData[deleteResult.IsSuccess ? "CatalogSuccess" : "CatalogError"] =
            deleteResult.IsSuccess ? "سند و تمام آیتم‌های آن حذف شد." : deleteResult.ErrorMessage ?? "حذف سند انجام نشد.";

        return RedirectToAction(
            ResolveDocumentRouteActionName(document.DocumentType),
            new
            {
                documentType = document.DocumentType,
                documentNo,
                status,
                warehouseId,
                sellerId,
                occurredFrom,
                occurredTo,
                page,
                pageSize,
                tab = "list"
            });
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
                        return "برای سند انتقال، لوکیشن مبدا و مقصد باید متفاوت باشند.";
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

    private static CreateInventoryDocumentForm BuildCreateForm(
        InventoryDocumentDetailsModel? existingDocument,
        string? warehouseId,
        string? sellerId,
        string documentType,
        string? selectedTab)
    {
        var isEditMode = string.Equals(selectedTab, "create", StringComparison.OrdinalIgnoreCase)
            && existingDocument is not null;
        var isReceiptDocument = string.Equals(documentType, "Receipt", StringComparison.OrdinalIgnoreCase);

        return new CreateInventoryDocumentForm
        {
            DocumentId = isEditMode ? existingDocument?.DocumentBusinessKey : null,
            DocumentType = documentType,
            DocumentNo = isEditMode ? existingDocument?.DocumentNo : null,
            ReferenceType = isEditMode ? existingDocument?.ReferenceType : null,
            ReferenceBusinessId = isEditMode ? existingDocument?.ReferenceBusinessId?.ToString() : null,
            WarehouseRef = isReceiptDocument
                ? string.Empty
                : isEditMode ? existingDocument?.WarehouseRef ?? string.Empty : warehouseId ?? string.Empty,
            SellerRef = isReceiptDocument
                ? string.Empty
                : isEditMode ? existingDocument?.SellerRef ?? string.Empty : sellerId ?? string.Empty,
            OccurredAt = isEditMode ? existingDocument?.OccurredAt ?? DateTime.Now : DateTime.Now,
            ReasonCode = isEditMode ? existingDocument?.ReasonCode : null,
            Lines = new List<CreateInventoryDocumentLineForm>()
        };
    }

    private async Task<ApiResponse<SellerSearchItemModel?>> ResolveOwnerSellerAsync(string token)
    {
        var result = await _apiService.SearchSellersAsync(token, isSystemOwner: true, isActive: true, pageSize: 10);
        if (!result.IsSuccess)
        {
            return new ApiResponse<SellerSearchItemModel?> { IsSuccess = false, ErrorMessage = result.ErrorMessage };
        }

        var ownerSellers = result.Data?.Items?
            .Where(x => x.IsSystemOwner && x.IsActive)
            .OrderBy(x => x.Code)
            .ToList() ?? new List<SellerSearchItemModel>();

        if (ownerSellers.Count == 1)
        {
            return new ApiResponse<SellerSearchItemModel?> { IsSuccess = true, Data = ownerSellers[0] };
        }

        return new ApiResponse<SellerSearchItemModel?>
        {
            IsSuccess = false,
            ErrorMessage = ownerSellers.Count == 0
                ? "Seller Owner فعال در سیستم پیدا نشد."
                : "بیش از یک Seller Owner فعال پیدا شد."
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
            WarehouseRef = null,
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

    private async Task<InventoryDocumentManagementPageViewModel> BuildReceiptDocumentDetailsModalModelAsync(
        string documentId,
        string token,
        CancellationToken cancellationToken = default)
    {
        var documentResult = await _apiService.GetInventoryDocumentByBusinessKeyAsync(documentId, token);
        var document = documentResult.Data;

        var variantsTask = _apiService.SearchVariantsAsync(token, page: 1, pageSize: 2000);
        var productsTask = _apiService.SearchProductsAsync(token, page: 1, pageSize: 2000);
        var warehousesTask = _apiService.GetWarehouseLookupAsync(token, includeInactive: true);
        var sellersTask = _apiService.GetSellerLookupAsync(token, includeInactive: true);
        var locationsTask = _apiService.GetLocationLookupAsync(token, warehouseId: null, includeInactive: true);
        var uomsTask = _apiService.GetUnitOfMeasureLookupAsync(token);

        await Task.WhenAll(variantsTask, productsTask, warehousesTask, sellersTask, locationsTask, uomsTask);

        var lineForm = BuildLineForm(document, documentId, null, "Receipt");
        if (string.IsNullOrWhiteSpace(lineForm.DestinationLocationRef))
        {
            lineForm.DestinationLocationRef = locationsTask.Result.Data?.FirstOrDefault()?.LocationBusinessKey ?? string.Empty;
        }

        if (string.IsNullOrWhiteSpace(lineForm.WarehouseRef) && !string.IsNullOrWhiteSpace(lineForm.DestinationLocationRef))
        {
            lineForm.WarehouseRef = locationsTask.Result.Data?
                .FirstOrDefault(x => string.Equals(x.LocationBusinessKey, lineForm.DestinationLocationRef, StringComparison.OrdinalIgnoreCase))
                ?.WarehouseRef;
        }

        return new InventoryDocumentManagementPageViewModel
        {
            SelectedDocumentId = documentId,
            SelectedDocumentDetails = document,
            ErrorMessage = documentResult.IsSuccess ? null : documentResult.ErrorMessage,
            ProductLookup = productsTask.Result.Data ?? new List<ProductSummaryModel>(),
            VariantLookup = variantsTask.Result.Data ?? new List<ProductVariantSummaryModel>(),
            WarehouseLookup = warehousesTask.Result.Data ?? new List<WarehouseLookupItemModel>(),
            SellerLookup = sellersTask.Result.Data ?? new List<SellerLookupItemModel>(),
            LocationLookup = locationsTask.Result.Data ?? new List<LocationLookupItemModel>(),
            UnitOfMeasureLookup = uomsTask.Result.Data ?? new List<UnitOfMeasureLookupModel>(),
            LineForm = lineForm
        };
    }

    private Task<InventoryDocumentManagementPageViewModel> BuildTransferDocumentDetailsModalModelAsync(
        string documentId,
        string token,
        CancellationToken cancellationToken = default)
    {
        return BuildTransferDocumentDetailsModalModelCoreAsync(documentId, token, cancellationToken);
    }

    private async Task<InventoryDocumentManagementPageViewModel> BuildTransferDocumentDetailsModalModelCoreAsync(
        string documentId,
        string token,
        CancellationToken cancellationToken = default)
    {
        var documentResult = await _apiService.GetInventoryDocumentByBusinessKeyAsync(documentId, token);
        var document = documentResult.Data;

        var variantsTask = _apiService.SearchVariantsAsync(token, page: 1, pageSize: 2000);
        var productsTask = _apiService.SearchProductsAsync(token, page: 1, pageSize: 2000);
        var warehousesTask = _apiService.GetWarehouseLookupAsync(token, includeInactive: true);
        var sellersTask = _apiService.GetSellerLookupAsync(token, includeInactive: true);
        var locationsTask = _apiService.GetLocationLookupAsync(token, warehouseId: null, includeInactive: true);
        var uomsTask = _apiService.GetUnitOfMeasureLookupAsync(token);

        await Task.WhenAll(variantsTask, productsTask, warehousesTask, sellersTask, locationsTask, uomsTask);

        var lineForm = BuildLineForm(document, documentId, null, "Transfer");

        return new InventoryDocumentManagementPageViewModel
        {
            SelectedDocumentId = documentId,
            SelectedDocumentDetails = document,
            ErrorMessage = documentResult.IsSuccess ? null : documentResult.ErrorMessage,
            ProductLookup = productsTask.Result.Data ?? new List<ProductSummaryModel>(),
            VariantLookup = variantsTask.Result.Data ?? new List<ProductVariantSummaryModel>(),
            WarehouseLookup = warehousesTask.Result.Data ?? new List<WarehouseLookupItemModel>(),
            SellerLookup = sellersTask.Result.Data ?? new List<SellerLookupItemModel>(),
            LocationLookup = locationsTask.Result.Data ?? new List<LocationLookupItemModel>(),
            UnitOfMeasureLookup = uomsTask.Result.Data ?? new List<UnitOfMeasureLookupModel>(),
            LineForm = lineForm
        };
    }

    private async Task<InventoryDocumentManagementPageViewModel> BuildIssueDocumentDetailsModalModelAsync(
        string documentId,
        string token,
        CancellationToken cancellationToken = default)
    {
        var documentResult = await _apiService.GetInventoryDocumentByBusinessKeyAsync(documentId, token);
        var document = documentResult.Data;

        var variantsTask = _apiService.SearchVariantsAsync(token, page: 1, pageSize: 2000);
        var productsTask = _apiService.SearchProductsAsync(token, page: 1, pageSize: 2000);
        var warehousesTask = _apiService.GetWarehouseLookupAsync(token, includeInactive: true);
        var sellersTask = _apiService.GetSellerLookupAsync(token, includeInactive: true);
        var locationsTask = _apiService.GetLocationLookupAsync(token, warehouseId: null, includeInactive: true);
        var uomsTask = _apiService.GetUnitOfMeasureLookupAsync(token);

        await Task.WhenAll(variantsTask, productsTask, warehousesTask, sellersTask, locationsTask, uomsTask);

        var lineForm = BuildLineForm(document, documentId, null, "Issue");

        return new InventoryDocumentManagementPageViewModel
        {
            SelectedDocumentId = documentId,
            SelectedDocumentDetails = document,
            ErrorMessage = documentResult.IsSuccess ? null : documentResult.ErrorMessage,
            ProductLookup = productsTask.Result.Data ?? new List<ProductSummaryModel>(),
            VariantLookup = variantsTask.Result.Data ?? new List<ProductVariantSummaryModel>(),
            WarehouseLookup = warehousesTask.Result.Data ?? new List<WarehouseLookupItemModel>(),
            SellerLookup = sellersTask.Result.Data ?? new List<SellerLookupItemModel>(),
            LocationLookup = locationsTask.Result.Data ?? new List<LocationLookupItemModel>(),
            UnitOfMeasureLookup = uomsTask.Result.Data ?? new List<UnitOfMeasureLookupModel>(),
            LineForm = lineForm
        };
    }

    private static string BuildVariantLookupLabel(ProductVariantSummaryModel variant)
    {
        var parts = new List<string>();
        if (!string.IsNullOrWhiteSpace(variant.Sku))
        {
            parts.Add(variant.Sku);
        }

        if (!string.IsNullOrWhiteSpace(variant.Name))
        {
            parts.Add(variant.Name);
        }

        if (!string.IsNullOrWhiteSpace(variant.Barcode))
        {
            parts.Add($"[{variant.Barcode}]");
        }

        return parts.Count == 0 ? variant.Id : string.Join(" - ", parts);
    }

    private async Task<string?> ResolveReceiptDefaultLocationRefAsync(string warehouseRef, string token, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(warehouseRef))
        {
            return null;
        }

        var locationResult = await _apiService.SearchLocationsAsync(token, warehouseId: warehouseRef, isActive: true, pageSize: 2000);
        return locationResult.Data?.Items.FirstOrDefault()?.LocationBusinessKey;
    }
}
