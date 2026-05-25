using System.Globalization;
using System.Text.Json;
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
        string? variantId,
        string? warehouseId,
        string? locationId,
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
            variantId,
            warehouseId,
            locationId,
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
        string? variantId,
        string? warehouseId,
        string? locationId,
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
            variantId,
            warehouseId,
            locationId,
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
        string? variantId,
        string? warehouseId,
        string? locationId,
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
            variantId,
            warehouseId,
            locationId,
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

    [HttpGet("/InventoryManagement/Documents/Receipt/PostPreview")]
    public async Task<IActionResult> ReceiptDocumentPostPreview(
        string documentId,
        CancellationToken cancellationToken = default)
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
                errorMessage = "شما دسترسی مشاهده جزئیات سند را ندارید."
            });
        }

        if (string.IsNullOrWhiteSpace(documentId))
        {
            return Json(new
            {
                isSuccess = false,
                errorMessage = "شناسه سند معتبر نیست."
            });
        }

        var documentResult = await _apiService.GetInventoryDocumentByBusinessKeyAsync(documentId, token);
        if (!documentResult.IsSuccess || documentResult.Data is null)
        {
            return Json(new
            {
                isSuccess = false,
                errorMessage = documentResult.ErrorMessage ?? "سند یافت نشد."
            });
        }

        var variantIds = documentResult.Data.Lines
            .Select(x => x.VariantRef)
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        var variantsResult = await _apiService.SearchVariantsAsync(token, page: 1, pageSize: 2000);
        var variantLookup = (variantsResult.Data ?? new List<ProductVariantSummaryModel>())
            .Where(x => variantIds.Contains(x.Id))
            .ToDictionary(x => x.Id, x => BuildVariantLookupLabel(x), StringComparer.OrdinalIgnoreCase);

        var serialTasks = documentResult.Data.Lines
            .Select(line => _apiService.GetAvailableSerialItemsAsync(token, line.VariantRef))
            .ToList();

        await Task.WhenAll(serialTasks);

        var lines = documentResult.Data.Lines
            .Select((line, index) =>
            {
                var availableSerials = serialTasks[index].Result.Data ?? new List<SerialItemLookupModel>();
                var selectedSerialRefs = line.Serials
                    .Select(serial => serial.SerialItemBusinessKey)
                    .Where(serialRef => !string.IsNullOrWhiteSpace(serialRef))
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .ToList();

                return new
                {
                    documentLineBusinessKey = line.LineBusinessKey,
                    lineNo = index + 1,
                    variantRef = line.VariantRef,
                    variantLabel = variantLookup.TryGetValue(line.VariantRef, out var label) ? label : line.VariantRef,
                    qty = line.Qty,
                    selectedSerials = selectedSerialRefs,
                    availableSerials = availableSerials.Select(serial => new
                    {
                        serialItemBusinessKey = serial.SerialItemBusinessKey,
                        serialNo = serial.SerialNo,
                        warehouseRef = serial.WarehouseRef,
                        locationRef = serial.LocationRef,
                        qualityStatusRef = serial.QualityStatusRef,
                        lotBatchNo = serial.LotBatchNo,
                        status = serial.Status
                    }).ToList()
                };
            })
            .ToList();

        return Json(new
        {
            isSuccess = true,
            documentId,
            documentNo = documentResult.Data.DocumentNo,
            lines
        });
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

    [HttpGet("/InventoryManagement/Documents/Receipt/WarehousesLookup")]
    public async Task<IActionResult> SearchReceiptWarehousesLookup(string? variantId, CancellationToken cancellationToken = default)
    {
        if (!TryGetToken(out var token))
        {
            return Json(new
            {
                isSuccess = false,
                errorMessage = "نشست کاربری منقضی شده است. لطفا دوباره وارد شوید."
            });
        }

        if (!string.IsNullOrWhiteSpace(variantId) && !Guid.TryParse(variantId, out _))
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
                errorMessage = "شما دسترسی مشاهده انبارها را ندارید."
            });
        }

        var warehouseLookupResult = await _apiService.GetWarehouseLookupAsync(token, includeInactive: true);
        if (!warehouseLookupResult.IsSuccess)
        {
            return Json(new
            {
                isSuccess = false,
                errorMessage = warehouseLookupResult.ErrorMessage
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

        return Json(new
        {
            isSuccess = true,
            warehouses
        });
    }

    [HttpGet("/InventoryManagement/Documents/Receipt/LocationsLookup")]
    public async Task<IActionResult> SearchReceiptWarehouseLocationsLookup(string warehouseId, CancellationToken cancellationToken = default)
    {
        if (!TryGetToken(out var token))
        {
            return Json(new
            {
                isSuccess = false,
                errorMessage = "نشست کاربری منقضی شده است. لطفا دوباره وارد شوید."
            });
        }

        if (string.IsNullOrWhiteSpace(warehouseId))
        {
            return Json(new
            {
                isSuccess = true,
                locations = new List<object>()
            });
        }

        if (!IsAuthorizedFor(token, "Inventory.Document.View", "Inventory.Document.Search", "InventoryDocument.Read", "InventoryDocument.Search", "Document.Read"))
        {
            return Json(new
            {
                isSuccess = false,
                errorMessage = "شما دسترسی مشاهده لوکیشن‌ها را ندارید."
            });
        }

        var locationLookupResult = await _apiService.GetLocationLookupAsync(token, warehouseId: warehouseId, includeInactive: true);
        if (!locationLookupResult.IsSuccess)
        {
            return Json(new
            {
                isSuccess = false,
                errorMessage = locationLookupResult.ErrorMessage
            });
        }

        var locations = (locationLookupResult.Data ?? new List<LocationLookupItemModel>())
            .OrderBy(x => x.LocationCode)
            .ThenBy(x => x.LocationType)
            .Select(x => new
            {
                id = x.LocationBusinessKey,
                locationId = x.LocationBusinessKey,
                warehouseId = x.WarehouseRef,
                text = string.IsNullOrWhiteSpace(x.LocationType)
                    ? x.LocationCode
                    : $"{x.LocationCode} ({x.LocationType})",
                code = x.LocationCode,
                type = x.LocationType
            })
            .ToList();

        return Json(new
        {
            isSuccess = true,
            locations
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

    [HttpGet("/InventoryManagement/Documents/Transfer/LocationsLookup")]
    public async Task<IActionResult> SearchTransferWarehouseLocationsLookup(string warehouseId, CancellationToken cancellationToken = default)
    {
        if (!TryGetToken(out var token))
        {
            return Json(new
            {
                isSuccess = false,
                errorMessage = "نوبت کاربری منقضی شده است. لطفا دوباره وارد شوید."
            });
        }

        if (string.IsNullOrWhiteSpace(warehouseId))
        {
            return Json(new
            {
                isSuccess = true,
                locations = new List<object>()
            });
        }

        if (!IsAuthorizedFor(token, "Inventory.Document.View", "Inventory.Document.Search", "InventoryDocument.Read", "InventoryDocument.Search", "Document.Read"))
        {
            return Json(new
            {
                isSuccess = false,
                errorMessage = "شما دسترسی مشاهده لوکیشن‌ها را ندارید."
            });
        }

        var locationLookupResult = await _apiService.GetLocationLookupAsync(token, warehouseId: warehouseId, includeInactive: true);
        if (!locationLookupResult.IsSuccess)
        {
            return Json(new
            {
                isSuccess = false,
                errorMessage = locationLookupResult.ErrorMessage
            });
        }

        var locations = (locationLookupResult.Data ?? new List<LocationLookupItemModel>())
            .OrderBy(x => x.LocationCode)
            .ThenBy(x => x.LocationType)
            .Select(x => new
            {
                id = x.LocationBusinessKey,
                locationId = x.LocationBusinessKey,
                warehouseId = x.WarehouseRef,
                text = string.IsNullOrWhiteSpace(x.LocationType)
                    ? x.LocationCode
                    : $"{x.LocationCode} ({x.LocationType})",
                code = x.LocationCode,
                type = x.LocationType
            })
            .ToList();

        return Json(new
        {
            isSuccess = true,
            locations
        });
    }

    [HttpGet("/InventoryManagement/Documents/WarehouseLocationsLookup")]
    public async Task<IActionResult> SearchDocumentWarehouseLocationsLookup(string warehouseId, CancellationToken cancellationToken = default)
        => await SearchTransferWarehouseLocationsLookup(warehouseId, cancellationToken);

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

        var warehouseLookup = (warehouseLookupResult.Data ?? new List<WarehouseLookupItemModel>())
            .ToDictionary(x => x.WarehouseBusinessKey, x => x, StringComparer.OrdinalIgnoreCase);
        var locationLookup = (locationLookupResult.Data ?? new List<LocationLookupItemModel>())
            .ToDictionary(x => x.LocationBusinessKey, x => x, StringComparer.OrdinalIgnoreCase);

        var allowedLocationIds = (stockBucketsResult.Data?.Items ?? new List<StockDetailBucketModel>())
            .Select(x => x.LocationRef.ToString("D"))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        var locations = allowedLocationIds
            .Select(locationId =>
            {
                if (!locationLookup.TryGetValue(locationId, out var location))
                {
                    return null;
                }

                var warehouse = warehouseLookup.TryGetValue(location.WarehouseRef, out var warehouseItem) ? warehouseItem : null;
                var warehouseLabel = warehouse is null ? location.WarehouseRef : $"{warehouse.Code} - {warehouse.Name}";
                var locationLabel = string.IsNullOrWhiteSpace(location.LocationType)
                    ? location.LocationCode
                    : $"{location.LocationCode} ({location.LocationType})";

                return new
                {
                    id = location.LocationBusinessKey,
                    locationId = location.LocationBusinessKey,
                    warehouseId = location.WarehouseRef,
                    warehouseText = warehouseLabel,
                    locationText = locationLabel,
                    text = string.IsNullOrWhiteSpace(warehouseLabel) ? locationLabel : $"{warehouseLabel} - {locationLabel}"
                };
            })
            .Where(x => x is not null)
            .ToList();

        return Json(new
        {
            isSuccess = true,
            locations
        });
    }

    [HttpGet("/InventoryManagement/Documents/Transfer/VariantSerialLookup")]
    public async Task<IActionResult> SearchTransferVariantSerialLookup(
        string variantId,
        string? sourceLocationId,
        CancellationToken cancellationToken = default)
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
                errorMessage = "شما دسترسی مشاهده سریال‌های واریانت را ندارید."
            });
        }

        var serialsResult = await _apiService.GetAvailableSerialItemsAsync(token, parsedVariantId.ToString("D"));
        if (!serialsResult.IsSuccess)
        {
            return Json(new
            {
                isSuccess = false,
                errorMessage = serialsResult.ErrorMessage
            });
        }

        var filteredSerials = serialsResult.Data ?? new List<SerialItemLookupModel>();
        if (Guid.TryParse(sourceLocationId, out var parsedSourceLocationId))
        {
            var sourceLocationKey = parsedSourceLocationId.ToString("D");
            filteredSerials = filteredSerials
                .Where(x => string.Equals(x.LocationRef, sourceLocationKey, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        return Json(new
        {
            isSuccess = true,
            serials = filteredSerials.Select(serial => new
            {
                serialItemBusinessKey = serial.SerialItemBusinessKey,
                serialNo = serial.SerialNo,
                warehouseRef = serial.WarehouseRef,
                locationRef = serial.LocationRef,
                qualityStatusRef = serial.QualityStatusRef,
                lotBatchNo = serial.LotBatchNo,
                status = serial.Status
            }).ToList()
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

        if (string.IsNullOrWhiteSpace(form.QualityStatusRef))
        {
            var invalidQualityModel = await BuildReceiptDocumentDetailsModalModelAsync(form.DocumentId, token);
            invalidQualityModel.ErrorMessage = "برای سند رسید، وضعیت کیفیت الزامی است.";
            invalidQualityModel.LineForm = form;
            return PartialView("~/Views/InventoryManagement/_ReceiptDocumentDetailsModalBody.cshtml", invalidQualityModel);
        }

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

        if (string.IsNullOrWhiteSpace(form.QualityStatusRef))
        {
            var invalidQualityModel = await BuildTransferDocumentDetailsModalModelAsync(form.DocumentId, token);
            invalidQualityModel.ErrorMessage = "برای سند انتقال، وضعیت کیفیت الزامی است.";
            invalidQualityModel.LineForm = form;
            return PartialView("~/Views/InventoryManagement/_TransferDocumentDetailsModalBody.cshtml", invalidQualityModel);
        }

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

        var availableSerialsResult = await _apiService.GetAvailableSerialItemsAsync(token, form.VariantId);
        if (!availableSerialsResult.IsSuccess)
        {
            var serialLookupErrorModel = await BuildTransferDocumentDetailsModalModelAsync(form.DocumentId, token);
            serialLookupErrorModel.ErrorMessage = availableSerialsResult.ErrorMessage ?? "واکشی سریال‌های واریانت انجام نشد.";
            serialLookupErrorModel.LineForm = form;
            return PartialView("~/Views/InventoryManagement/_TransferDocumentDetailsModalBody.cshtml", serialLookupErrorModel);
        }

        if (Guid.TryParse(form.SourceLocationRef, out var parsedSourceLocationId))
        {
            var sourceLocationKey = parsedSourceLocationId.ToString("D");
            var availableSerialsForSource = (availableSerialsResult.Data ?? new List<SerialItemLookupModel>())
                .Where(x => string.Equals(x.LocationRef, sourceLocationKey, StringComparison.OrdinalIgnoreCase))
                .ToList();

            if (availableSerialsForSource.Count > 0 && (form.Serials is null || form.Serials.Count == 0))
            {
                var serialRequiredModel = await BuildTransferDocumentDetailsModalModelAsync(form.DocumentId, token);
                serialRequiredModel.ErrorMessage = "برای این واریانت، انتخاب حداقل یک سریال در انتقال الزامی است.";
                serialRequiredModel.LineForm = form;
                return PartialView("~/Views/InventoryManagement/_TransferDocumentDetailsModalBody.cshtml", serialRequiredModel);
            }
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

    [HttpGet("/InventoryManagement/Documents/Adjustment/Details")]
    public async Task<IActionResult> AdjustmentDocumentDetails(
        string documentId,
        string? editingLineId = null,
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

        var model = await BuildAdjustmentDocumentDetailsModalModelAsync(documentId, token, editingLineId, cancellationToken);
        return PartialView("~/Views/InventoryManagement/_AdjustmentDocumentDetailsModalBody.cshtml", model);
    }

    [HttpGet("/InventoryManagement/Documents/Adjustment/VariantInventoryLookup")]
    public async Task<IActionResult> SearchAdjustmentVariantInventoryLookup(string variantId, CancellationToken cancellationToken = default)
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
            return Json(new { isSuccess = false, errorMessage = stockBucketsResult.ErrorMessage });
        }

        var warehouseLookupResult = await _apiService.GetWarehouseLookupAsync(token, includeInactive: true);
        var locationLookupResult = await _apiService.GetLocationLookupAsync(token, warehouseId: null, includeInactive: true);
        if (!warehouseLookupResult.IsSuccess || !locationLookupResult.IsSuccess)
        {
            return Json(new
            {
                isSuccess = false,
                errorMessage = JoinErrors(warehouseLookupResult.ErrorMessage, locationLookupResult.ErrorMessage)
            });
        }

        var warehouseLookup = (warehouseLookupResult.Data ?? new List<WarehouseLookupItemModel>())
            .ToDictionary(x => x.WarehouseBusinessKey, x => x, StringComparer.OrdinalIgnoreCase);
        var locationLookup = (locationLookupResult.Data ?? new List<LocationLookupItemModel>())
            .ToDictionary(x => x.LocationBusinessKey, x => x, StringComparer.OrdinalIgnoreCase);

        var allowedLocationIds = (stockBucketsResult.Data?.Items ?? new List<StockDetailBucketModel>())
            .Select(x => x.LocationRef.ToString("D"))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        var locations = allowedLocationIds
            .Select(locationId =>
            {
                if (!locationLookup.TryGetValue(locationId, out var location))
                {
                    return null;
                }

                var warehouse = warehouseLookup.TryGetValue(location.WarehouseRef, out var warehouseItem) ? warehouseItem : null;
                var warehouseLabel = warehouse is null ? location.WarehouseRef : $"{warehouse.Code} - {warehouse.Name}";
                var locationLabel = string.IsNullOrWhiteSpace(location.LocationType)
                    ? location.LocationCode
                    : $"{location.LocationCode} ({location.LocationType})";

                return new
                {
                    id = location.LocationBusinessKey,
                    locationId = location.LocationBusinessKey,
                    warehouseId = location.WarehouseRef,
                    warehouseText = warehouseLabel,
                    locationText = locationLabel,
                    text = string.IsNullOrWhiteSpace(warehouseLabel) ? locationLabel : $"{warehouseLabel} - {locationLabel}"
                };
            })
            .Where(x => x is not null)
            .ToList();

        return Json(new { isSuccess = true, locations });
    }

    [HttpGet("/InventoryManagement/Documents/Adjustment/SerialLookup")]
    public async Task<IActionResult> SearchAdjustmentSerialLookup(string variantId, string serialNo, CancellationToken cancellationToken = default)
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

        if (string.IsNullOrWhiteSpace(serialNo))
        {
            return Json(new
            {
                isSuccess = false,
                errorMessage = "شماره شناسه الزامی است."
            });
        }

        if (!IsAuthorizedFor(token, "Inventory.Document.View", "Inventory.Document.Search", "InventoryDocument.Read", "InventoryDocument.Search", "Document.Read"))
        {
            return Json(new
            {
                isSuccess = false,
                errorMessage = "شما دسترسی مشاهده شناسه‌ها را ندارید."
            });
        }

        var serialResult = await _apiService.GetSerialItemBySerialNoAsync(token, serialNo, variantId);
        if (!serialResult.IsSuccess)
        {
            return Json(new { isSuccess = false, errorMessage = serialResult.ErrorMessage });
        }

        var serial = serialResult.Data;
        return Json(new
        {
            isSuccess = serial is not null && !string.Equals(serial.Status, "Available", StringComparison.OrdinalIgnoreCase),
            serial = serial is null ? null : new
            {
                serialItemBusinessKey = serial.SerialItemBusinessKey,
                serialNo = serial.SerialNo,
                warehouseRef = serial.WarehouseRef,
                locationRef = serial.LocationRef,
                qualityStatusRef = serial.QualityStatusRef,
                lotBatchNo = serial.LotBatchNo,
                status = serial.Status
            }
        });
    }

    [HttpGet("/InventoryManagement/Documents/Adjustment/AvailableSerialLookup")]
    public async Task<IActionResult> SearchAdjustmentAvailableSerialLookup(
        string variantId,
        string? locationId,
        CancellationToken cancellationToken = default)
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
                errorMessage = "شما دسترسی مشاهده شناسه‌ها را ندارید."
            });
        }

        var serialsResult = await _apiService.GetAvailableSerialItemsAsync(token, parsedVariantId.ToString("D"));
        if (!serialsResult.IsSuccess)
        {
            return Json(new { isSuccess = false, errorMessage = serialsResult.ErrorMessage });
        }

        var serials = serialsResult.Data ?? new List<SerialItemLookupModel>();
        if (Guid.TryParse(locationId, out var parsedLocationId))
        {
            var locationKey = parsedLocationId.ToString("D");
            serials = serials.Where(x => string.Equals(x.LocationRef, locationKey, StringComparison.OrdinalIgnoreCase)).ToList();
        }

        return Json(new
        {
            isSuccess = true,
            serials = serials.Select(serial => new
            {
                serialItemBusinessKey = serial.SerialItemBusinessKey,
                serialNo = serial.SerialNo,
                warehouseRef = serial.WarehouseRef,
                locationRef = serial.LocationRef,
                qualityStatusRef = serial.QualityStatusRef,
                lotBatchNo = serial.LotBatchNo,
                status = serial.Status
            }).ToList()
        });
    }

    [HttpGet("/InventoryManagement/Documents/Issue")]
    public Task<IActionResult> IssueDocuments(
        string? documentId,
        string? editingLineId,
        string? documentNo,
        string? status,
        string? variantId,
        string? warehouseId,
        string? locationId,
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
            variantId,
            warehouseId,
            locationId,
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
            .Select(x => x.LocationRef.ToString("D"))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        return Json(new
        {
            isSuccess = true,
            allowedLocationIds
        });
    }

    [HttpGet("/InventoryManagement/Documents/Issue/VariantSerialLookup")]
    public async Task<IActionResult> SearchIssueVariantSerialLookup(
        string variantId,
        string? sourceLocationId,
        CancellationToken cancellationToken = default)
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
                errorMessage = "شما دسترسی مشاهده سریال‌های واریانت را ندارید."
            });
        }

        var serialsResult = await _apiService.GetAvailableSerialItemsAsync(token, parsedVariantId.ToString("D"));
        if (!serialsResult.IsSuccess)
        {
            return Json(new
            {
                isSuccess = false,
                errorMessage = serialsResult.ErrorMessage
            });
        }

        var filteredSerials = serialsResult.Data ?? new List<SerialItemLookupModel>();
        if (Guid.TryParse(sourceLocationId, out var parsedSourceLocationId))
        {
            var sourceLocationKey = parsedSourceLocationId.ToString("D");
            filteredSerials = filteredSerials
                .Where(x => string.Equals(x.LocationRef, sourceLocationKey, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        return Json(new
        {
            isSuccess = true,
            serials = filteredSerials.Select(serial => new
            {
                serialItemBusinessKey = serial.SerialItemBusinessKey,
                serialNo = serial.SerialNo,
                warehouseRef = serial.WarehouseRef,
                locationRef = serial.LocationRef,
                qualityStatusRef = serial.QualityStatusRef,
                lotBatchNo = serial.LotBatchNo,
                status = serial.Status
            }).ToList()
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

        if (string.IsNullOrWhiteSpace(form.QualityStatusRef))
        {
            var invalidQualityModel = await BuildIssueDocumentDetailsModalModelAsync(form.DocumentId, token);
            invalidQualityModel.ErrorMessage = "برای سند حواله، وضعیت کیفیت الزامی است.";
            invalidQualityModel.LineForm = form;
            return PartialView("~/Views/InventoryManagement/_IssueDocumentDetailsModalBody.cshtml", invalidQualityModel);
        }

        if (string.IsNullOrWhiteSpace(form.SourceLocationRef))
        {
            var invalidLocationModel = await BuildIssueDocumentDetailsModalModelAsync(form.DocumentId, token);
            invalidLocationModel.ErrorMessage = "برای سند حواله، لوکیشن مبدأ الزامی است.";
            invalidLocationModel.LineForm = form;
            return PartialView("~/Views/InventoryManagement/_IssueDocumentDetailsModalBody.cshtml", invalidLocationModel);
        }

        var availableSerialsResult = await _apiService.GetAvailableSerialItemsAsync(token, form.VariantId);
        if (!availableSerialsResult.IsSuccess)
        {
            var serialLookupErrorModel = await BuildIssueDocumentDetailsModalModelAsync(form.DocumentId, token);
            serialLookupErrorModel.ErrorMessage = availableSerialsResult.ErrorMessage ?? "واکشی سریال‌های واریانت انجام نشد.";
            serialLookupErrorModel.LineForm = form;
            return PartialView("~/Views/InventoryManagement/_IssueDocumentDetailsModalBody.cshtml", serialLookupErrorModel);
        }

        if (Guid.TryParse(form.SourceLocationRef, out var parsedSourceLocationId))
        {
            var sourceLocationKey = parsedSourceLocationId.ToString("D");
            var availableSerialsForSource = (availableSerialsResult.Data ?? new List<SerialItemLookupModel>())
                .Where(x => string.Equals(x.LocationRef, sourceLocationKey, StringComparison.OrdinalIgnoreCase))
                .ToList();

            if (availableSerialsForSource.Count > 0 && (form.Serials is null || form.Serials.Count == 0))
            {
                var serialRequiredModel = await BuildIssueDocumentDetailsModalModelAsync(form.DocumentId, token);
                serialRequiredModel.ErrorMessage = "برای این واریانت، انتخاب حداقل یک سریال در حواله الزامی است.";
                serialRequiredModel.LineForm = form;
                return PartialView("~/Views/InventoryManagement/_IssueDocumentDetailsModalBody.cshtml", serialRequiredModel);
            }
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
        string? variantId,
        string? warehouseId,
        string? locationId,
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
            variantId,
            warehouseId,
            locationId,
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
        string? variantId,
        string? warehouseId,
        string? locationId,
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
            variantId,
            warehouseId,
            locationId,
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
        string? variantId,
        string? warehouseId,
        string? locationId,
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
            variantId,
            warehouseId,
            locationId,
            sellerId,
            occurredFrom,
            occurredTo,
            page,
            pageSize,
            cancellationToken);

    [HttpGet("/InventoryManagement/Documents/Return/Details")]
    public async Task<IActionResult> ReturnDocumentDetails(
        string documentId,
        string? editingLineId = null,
        CancellationToken cancellationToken = default)
    {
        if (!TryGetToken(out var token))
        {
            return Content("Ø´Ù…Ø§ Ø¯Ø³ØªØ±Ø³ÛŒ Ù…Ø´Ø§Ù‡Ø¯Ù‡ Ø§Ø³Ù†Ø§Ø¯ Ø±Ø§ Ù†Ø¯Ø§Ø±ÛŒØ¯.");
        }

        if (!IsAuthorizedFor(token, "Inventory.Document.View", "Inventory.Document.Search", "InventoryDocument.Read", "InventoryDocument.Search", "Document.Read"))
        {
            return Content("Ø´Ù…Ø§ Ø¯Ø³ØªØ±Ø³ÛŒ Ù…Ø´Ø§Ù‡Ø¯Ù‡ Ø§Ø³Ù†Ø§Ø¯ Ø±Ø§ Ù†Ø¯Ø§Ø±ÛŒØ¯.");
        }

        if (!Guid.TryParse(documentId, out _))
        {
            return Content("Ø³Ù†Ø¯ Ù…Ø¹ØªØ¨Ø± Ù†ÛŒØ³Øª.");
        }

        var model = await BuildReturnDocumentDetailsModalModelAsync(documentId, token, editingLineId, cancellationToken);
        return PartialView("~/Views/InventoryManagement/_ReturnDocumentDetailsModalBody.cshtml", model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SaveReturnDocumentLine([Bind(Prefix = "LineForm")] InventoryDocumentLineForm form, CancellationToken cancellationToken = default)
    {
        if (!TryGetToken(out var token))
        {
            return RedirectToAction("Login", "Auth");
        }

        if (!IsAuthorizedFor(token, "Inventory.Document.Create", "InventoryDocument.Create", "Document.Create"))
        {
            return Content("Ø´Ù…Ø§ Ø¯Ø³ØªØ±Ø³ÛŒ ÙˆÛŒØ±Ø§ÛŒØ´ Ø¢ÛŒØªÙ… Ø³Ù†Ø¯ Ø±Ø§ Ù†Ø¯Ø§Ø±ÛŒØ¯.");
        }

        if (!TryValidateModel(form))
        {
            var invalidModel = await BuildReturnDocumentDetailsModalModelAsync(form.DocumentId, token, form.LineId, cancellationToken);
            invalidModel.ErrorMessage = ExtractModelError(ModelState);
            invalidModel.LineForm = form;
            return PartialView("~/Views/InventoryManagement/_ReturnDocumentDetailsModalBody.cshtml", invalidModel);
        }

        if (string.IsNullOrWhiteSpace(form.DestinationLocationRef))
        {
            var invalidModel = await BuildReturnDocumentDetailsModalModelAsync(form.DocumentId, token, form.LineId, cancellationToken);
            invalidModel.ErrorMessage = "Ø¨Ø±Ø§ÛŒ Ø³Ù†Ø¯ Ø¨Ø§Ø±Ú¯Ø´ØªØŒ Ù„ÙˆÚÚÛŒØ´Ù† Ù…Ù‚ØµØ¯ Ø§Ù„Ø²Ø§Ù…ÛŒ Ø§Ø³Øª.";
            invalidModel.LineForm = form;
            return PartialView("~/Views/InventoryManagement/_ReturnDocumentDetailsModalBody.cshtml", invalidModel);
        }

        var lineResult = string.IsNullOrWhiteSpace(form.LineId)
            ? await _apiService.AddInventoryDocumentLineAsync(form.DocumentId, form, token)
            : await _apiService.UpdateInventoryDocumentLineAsync(form.DocumentId, form.LineId!, form, token);

        var refreshedModel = await BuildReturnDocumentDetailsModalModelAsync(form.DocumentId, token, null, cancellationToken);
        refreshedModel.ErrorMessage = lineResult.IsSuccess ? null : lineResult.ErrorMessage ?? "Ø°Ø®ÛŒØ±Ù‡ Ø¢ÛŒØªÙ… Ø³Ù†Ø¯ Ø§Ù†Ø¬Ø§Ù… Ù†Ø´Ø¯.";
        return PartialView("~/Views/InventoryManagement/_ReturnDocumentDetailsModalBody.cshtml", refreshedModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteReturnDocumentLine(string documentId, string lineId, CancellationToken cancellationToken = default)
    {
        if (!TryGetToken(out var token))
        {
            return RedirectToAction("Login", "Auth");
        }

        if (!IsAuthorizedFor(token, "Inventory.Document.Create", "InventoryDocument.Create", "Document.Create"))
        {
            return Content("Ø´Ù…Ø§ Ø¯Ø³ØªØ±Ø³ÛŒ Ø­Ø°Ù Ø¢ÛŒØªÙ… Ø³Ù†Ø¯ Ø±Ø§ Ù†Ø¯Ø§Ø±ÛŒØ¯.");
        }

        if (string.IsNullOrWhiteSpace(documentId) || string.IsNullOrWhiteSpace(lineId))
        {
            var invalidModel = await BuildReturnDocumentDetailsModalModelAsync(documentId, token, null, cancellationToken);
            invalidModel.ErrorMessage = "Ø¢ÛŒØªÙ… Ø³Ù†Ø¯ Ø¨Ø±Ø§ÛŒ Ø­Ø°Ù Ù…Ø´Ø®Øµ Ù†Ø´Ø¯Ù‡ Ø§Ø³Øª.";
            return PartialView("~/Views/InventoryManagement/_ReturnDocumentDetailsModalBody.cshtml", invalidModel);
        }

        var deleteResult = await _apiService.DeleteInventoryDocumentLineAsync(documentId, lineId, token);
        var refreshedModel = await BuildReturnDocumentDetailsModalModelAsync(documentId, token, null, cancellationToken);
        refreshedModel.ErrorMessage = deleteResult.IsSuccess ? null : deleteResult.ErrorMessage ?? "Ø­Ø°Ù Ø¢ÛŒØªÙ… Ø³Ù†Ø¯ Ø§Ù†Ø¬Ø§Ù… Ù†Ø´Ø¯.";
        return PartialView("~/Views/InventoryManagement/_ReturnDocumentDetailsModalBody.cshtml", refreshedModel);
    }

    [HttpGet("/InventoryManagement/Documents/QualityChange")]
    public Task<IActionResult> QualityChangeDocuments(
        string? documentId,
        string? editingLineId,
        string? documentNo,
        string? status,
        string? variantId,
        string? warehouseId,
        string? locationId,
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
            variantId,
            warehouseId,
            locationId,
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
        string? variantId,
        string? warehouseId,
        string? locationId,
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
            variantId,
            warehouseId,
            locationId,
            sellerId,
            occurredFrom,
            occurredTo,
            page,
            pageSize,
            cancellationToken);

    [HttpGet("/InventoryManagement/Documents/Return/ReferenceSearch")]
    public async Task<IActionResult> SearchReturnReferenceDocuments(
        string? term,
        CancellationToken cancellationToken = default)
    {
        if (!TryGetToken(out var token))
        {
            return Json(new
            {
                isSuccess = false,
                errorMessage = "نشست کاربری منقضی شده است. لطفاً دوباره وارد شوید."
            });
        }

        if (!IsAuthorizedFor(token, "Inventory.Document.View", "Inventory.Document.Search", "InventoryDocument.Read", "InventoryDocument.Search", "Document.Read"))
        {
            return Json(new
            {
                isSuccess = false,
                errorMessage = "شما دسترسی مشاهده اسناد حواله مبنا را ندارید."
            });
        }

        var searchResult = await _apiService.SearchInventoryDocumentsAsync(
            token,
            documentNo: string.IsNullOrWhiteSpace(term) ? null : term.Trim(),
            documentType: "Transfer",
            status: null,
            variantId: null,
            warehouseId: null,
            locationId: null,
            sellerId: null,
            occurredFrom: null,
            occurredTo: null,
            page: 1,
            pageSize: 15);

        if (!searchResult.IsSuccess)
        {
            return Json(new
            {
                isSuccess = false,
                errorMessage = searchResult.ErrorMessage
            });
        }

        return Json(new
        {
            isSuccess = true,
            documents = (searchResult.Data?.Items ?? new List<InventoryDocumentListItemModel>())
                .Select(document => new
                {
                    documentBusinessKey = document.DocumentBusinessKey,
                    documentNo = document.DocumentNo,
                    status = document.Status,
                    warehouseRef = document.WarehouseRef,
                    sellerRef = document.SellerRef,
                    occurredAt = document.OccurredAt,
                    referenceType = document.ReferenceType,
                    referenceBusinessId = document.ReferenceBusinessId
                })
                .ToList()
        });
    }

    [HttpGet("/InventoryManagement/Documents/Return/ReferenceDocument")]
    public async Task<IActionResult> GetReturnReferenceDocument(
        string documentId,
        CancellationToken cancellationToken = default)
    {
        if (!TryGetToken(out var token))
        {
            return Json(new
            {
                isSuccess = false,
                errorMessage = "نشست کاربری منقضی شده است. لطفاً دوباره وارد شوید."
            });
        }

        if (!IsAuthorizedFor(token, "Inventory.Document.View", "Inventory.Document.Search", "InventoryDocument.Read", "InventoryDocument.Search", "Document.Read"))
        {
            return Json(new
            {
                isSuccess = false,
                errorMessage = "شما دسترسی مشاهده سند مبنا را ندارید."
            });
        }

        if (!Guid.TryParse(documentId, out _))
        {
            return Json(new
            {
                isSuccess = false,
                errorMessage = "سند مبنا معتبر نیست."
            });
        }

        var documentResult = await _apiService.GetInventoryDocumentByBusinessKeyAsync(documentId, token);
        if (!documentResult.IsSuccess || documentResult.Data is null)
        {
            return Json(new
            {
                isSuccess = false,
                errorMessage = documentResult.ErrorMessage ?? "سند مبنا یافت نشد."
            });
        }

        return Json(new
        {
            isSuccess = true,
            document = documentResult.Data
        });
    }

    private async Task<IActionResult> BuildDocumentPageAsync(
        string documentAction,
        string? documentId,
        string? editingLineId,
        string? tab,
        string? documentNo,
        string? documentType,
        string? status,
        string? variantId,
        string? warehouseId,
        string? locationId,
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

        var createSellerId = sellerId;
        if (string.IsNullOrWhiteSpace(createSellerId) && !string.Equals(resolvedDocumentType, "Receipt", StringComparison.OrdinalIgnoreCase))
        {
            var ownerSellerResult = await ResolveOwnerSellerAsync(token);
            if (ownerSellerResult.IsSuccess && ownerSellerResult.Data is not null)
            {
                createSellerId = ownerSellerResult.Data.SellerBusinessKey.ToString("D");
            }
        }

        pageSize = NormalizePageSize(pageSize);
        var searchResult = await _apiService.SearchInventoryDocumentsAsync(
            token,
            documentNo,
            resolvedDocumentType,
            status,
            variantId,
            warehouseId,
            locationId,
            sellerId,
            ParseDate(occurredFrom),
            ParseDateEndOfDay(occurredTo),
            Math.Max(page, 1),
            pageSize);

        var warehouseLookupResult = await _apiService.GetWarehouseLookupAsync(token, includeInactive: true);
        var sellerLookupResult = await _apiService.GetSellerLookupAsync(token, includeInactive: true);
        var locationLookupResult = await _apiService.GetLocationLookupAsync(token, warehouseId: null, includeInactive: false);
        var qualityStatusLookupResult = await _apiService.GetQualityStatusLookupAsync(token, includeInactive: false);
        var userLookupResult = await _apiService.GetUsersAsync(token);
        var roleLookupResult = await _apiService.GetRolesAsync(token);
        var productLookupResult = await _apiService.SearchProductsAsync(token, page: 1, pageSize: 2000);
        var variantLookupResult = await _apiService.SearchVariantsAsync(token, page: 1, pageSize: 2000);
        var unitOfMeasureLookupResult = await _apiService.GetUnitOfMeasureLookupAsync(token);
        var createWarehouseId = warehouseId;
        if (string.IsNullOrWhiteSpace(createWarehouseId))
        {
            createWarehouseId = warehouseLookupResult.Data?.FirstOrDefault()?.WarehouseBusinessKey;
        }

        if (string.IsNullOrWhiteSpace(createSellerId))
        {
            createSellerId = sellerLookupResult.Data?.FirstOrDefault()?.SellerBusinessKey;
        }

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
            UserLookup = userLookupResult.Data ?? new List<UserSummaryModel>(),
            RoleLookup = roleLookupResult.Data ?? new List<RoleSummaryModel>(),
            ProductLookup = productLookupResult.Data ?? new List<ProductSummaryModel>(),
            VariantLookup = variantLookupResult.Data ?? new List<ProductVariantSummaryModel>(),
            UnitOfMeasureLookup = unitOfMeasureLookupResult.Data ?? new List<UnitOfMeasureLookupModel>(),
            DocumentNoFilter = documentNo,
            DocumentTypeFilter = resolvedDocumentType,
            DocumentStatusFilter = status,
            VariantFilter = variantId,
            WarehouseFilter = warehouseId,
            LocationFilter = locationId,
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
                userLookupResult.ErrorMessage,
                roleLookupResult.ErrorMessage,
                productLookupResult.ErrorMessage,
                variantLookupResult.ErrorMessage,
                unitOfMeasureLookupResult.ErrorMessage,
                selectedDocumentResult.ErrorMessage),
            CreateForm = BuildCreateForm(
                selectedDocumentResult.Data,
                createWarehouseId,
                createSellerId,
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
        var isConversionDocument = string.Equals(form.DocumentType, "Conversion", StringComparison.OrdinalIgnoreCase);
        form.Lines ??= new List<CreateInventoryDocumentLineForm>();

        if (isUpdate)
        {
            form.Lines.Clear();
        }
        else if (isConversionDocument)
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

        if (string.Equals(form.DocumentType, "Receipt", StringComparison.OrdinalIgnoreCase)
            || string.Equals(form.DocumentType, "Issue", StringComparison.OrdinalIgnoreCase))
        {
            if (string.IsNullOrWhiteSpace(form.ReceivedBy) || string.IsNullOrWhiteSpace(form.DeliveredBy))
            {
                TempData["CatalogError"] = "برای رسید و حواله، انتخاب تحویل‌دهنده و تحویل‌گیرنده الزامی است.";
                return RedirectToAction(
                    ResolveDocumentRouteActionName(form.DocumentType),
                    new { documentType = form.DocumentType, warehouseId = form.WarehouseRef, documentId = form.DocumentId, tab = "create" });
            }
        }

        if (string.Equals(form.DocumentType, "Return", StringComparison.OrdinalIgnoreCase)
            && string.IsNullOrWhiteSpace(form.ReferenceBusinessId))
        {
            TempData["CatalogError"] = "برای سند مرجوعی، انتخاب سند حواله مبنا الزامی است.";
            return RedirectToAction(
                ResolveDocumentRouteActionName(form.DocumentType),
                new { documentType = form.DocumentType, warehouseId = form.WarehouseRef, documentId = form.DocumentId, tab = "create" });
        }

        if (string.Equals(form.DocumentType, "Adjustment", StringComparison.OrdinalIgnoreCase)
            && string.IsNullOrWhiteSpace(form.WarehouseRef))
        {
            TempData["CatalogError"] = "انتخاب انبار برای سند تعدیل الزامی است.";
            return RedirectToAction(
                ResolveDocumentRouteActionName(form.DocumentType),
                new { documentType = form.DocumentType, documentId = form.DocumentId, tab = "create" });
        }

        if (!isUpdate)
        {
            if (isConversionDocument)
            {
                var conversionError = ValidateConversionDocumentForm(form);
                if (!string.IsNullOrWhiteSpace(conversionError))
                {
                    TempData["CatalogError"] = conversionError;
                    return RedirectToAction(
                        ResolveDocumentRouteActionName(form.DocumentType),
                        new { documentType = form.DocumentType, warehouseId = form.WarehouseRef, tab = "create" });
                }

                var conversionBuildResult = await BuildConversionDocumentAsync(token, form);
                if (!conversionBuildResult.Success || conversionBuildResult.Form is null)
                {
                    TempData["CatalogError"] = conversionBuildResult.Error ?? "امکان آماده‌سازی سند تبدیل وجود نداشت.";
                    return RedirectToAction(
                        ResolveDocumentRouteActionName(form.DocumentType),
                        new { documentType = form.DocumentType, warehouseId = form.WarehouseRef, tab = "create" });
                }

                form = conversionBuildResult.Form;
            }
            else if (!isReceiptDocument)
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
    public async Task<IActionResult> CreateDocumentUser(CreateDocumentUserForm form, CancellationToken cancellationToken = default)
    {
        if (!TryGetToken(out var token))
        {
            return Json(new
            {
                isSuccess = false,
                errorMessage = "نشست کاربری منقضی شده است. لطفاً دوباره وارد شوید."
            });
        }

        if (!IsAuthorizedFor(token, "User.Create", "User.Manage"))
        {
            return Json(new
            {
                isSuccess = false,
                errorMessage = "شما دسترسی ایجاد کاربر را ندارید."
            });
        }

        if (!TryValidateModel(form))
        {
            return Json(new
            {
                isSuccess = false,
                errorMessage = ExtractModelError(ModelState)
            });
        }

        var tempPassword = BuildTemporaryPassword();
        var result = await _apiService.CreateUserAsync(token, form, tempPassword);
        return Json(new
        {
            isSuccess = result.IsSuccess,
            errorMessage = result.ErrorMessage,
            temporaryPassword = result.IsSuccess ? tempPassword : null
        });
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
    public async Task<IActionResult> SaveAdjustmentDocumentLine([Bind(Prefix = "LineForm")] InventoryDocumentLineForm form, CancellationToken cancellationToken = default)
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
            var invalidModel = await BuildAdjustmentDocumentDetailsModalModelAsync(form.DocumentId, token, null, cancellationToken);
            invalidModel.ErrorMessage = ExtractModelError(ModelState);
            invalidModel.LineForm = form;
            return PartialView("~/Views/InventoryManagement/_AdjustmentDocumentDetailsModalBody.cshtml", invalidModel);
        }

        if (string.IsNullOrWhiteSpace(form.AdjustmentDirection))
        {
            var invalidDirectionModel = await BuildAdjustmentDocumentDetailsModalModelAsync(form.DocumentId, token, null, cancellationToken);
            invalidDirectionModel.ErrorMessage = "جهت تعدیل الزامی است.";
            invalidDirectionModel.LineForm = form;
            return PartialView("~/Views/InventoryManagement/_AdjustmentDocumentDetailsModalBody.cshtml", invalidDirectionModel);
        }

        if (string.IsNullOrWhiteSpace(form.ReasonCode))
        {
            var invalidReasonModel = await BuildAdjustmentDocumentDetailsModalModelAsync(form.DocumentId, token, null, cancellationToken);
            invalidReasonModel.ErrorMessage = "علت ردیف الزامی است.";
            invalidReasonModel.LineForm = form;
            return PartialView("~/Views/InventoryManagement/_AdjustmentDocumentDetailsModalBody.cshtml", invalidReasonModel);
        }

        if (string.Equals(form.AdjustmentDirection, "Increase", StringComparison.OrdinalIgnoreCase))
        {
            if (string.IsNullOrWhiteSpace(form.DestinationLocationRef))
            {
                var invalidLocationModel = await BuildAdjustmentDocumentDetailsModalModelAsync(form.DocumentId, token, null, cancellationToken);
                invalidLocationModel.ErrorMessage = "برای افزایش، لوکیشن مقصد الزامی است.";
                invalidLocationModel.LineForm = form;
                return PartialView("~/Views/InventoryManagement/_AdjustmentDocumentDetailsModalBody.cshtml", invalidLocationModel);
            }
        }
        else if (string.Equals(form.AdjustmentDirection, "Decrease", StringComparison.OrdinalIgnoreCase))
        {
            if (string.IsNullOrWhiteSpace(form.SourceLocationRef))
            {
                var invalidLocationModel = await BuildAdjustmentDocumentDetailsModalModelAsync(form.DocumentId, token, null, cancellationToken);
                invalidLocationModel.ErrorMessage = "برای کاهش، لوکیشن مبدأ الزامی است.";
                invalidLocationModel.LineForm = form;
                return PartialView("~/Views/InventoryManagement/_AdjustmentDocumentDetailsModalBody.cshtml", invalidLocationModel);
            }
        }

        var result = string.IsNullOrWhiteSpace(form.LineId)
            ? await _apiService.AddInventoryDocumentLineAsync(form.DocumentId, form, token)
            : await _apiService.UpdateInventoryDocumentLineAsync(form.DocumentId, form.LineId!, form, token);

        var refreshedModel = await BuildAdjustmentDocumentDetailsModalModelAsync(form.DocumentId, token, null, cancellationToken);
        refreshedModel.ErrorMessage = result.IsSuccess ? null : result.ErrorMessage ?? "ذخیره آیتم سند انجام نشد.";
        return PartialView("~/Views/InventoryManagement/_AdjustmentDocumentDetailsModalBody.cshtml", refreshedModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteAdjustmentDocumentLine(string documentId, string lineId, CancellationToken cancellationToken = default)
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
            var invalidModel = await BuildAdjustmentDocumentDetailsModalModelAsync(documentId, token, null, cancellationToken);
            invalidModel.ErrorMessage = "آیتم سند برای حذف مشخص نشده است.";
            return PartialView("~/Views/InventoryManagement/_AdjustmentDocumentDetailsModalBody.cshtml", invalidModel);
        }

        var result = await _apiService.DeleteInventoryDocumentLineAsync(documentId, lineId, token);
        var refreshedModel = await BuildAdjustmentDocumentDetailsModalModelAsync(documentId, token, null, cancellationToken);
        refreshedModel.ErrorMessage = result.IsSuccess ? null : result.ErrorMessage ?? "حذف آیتم سند انجام نشد.";
        return PartialView("~/Views/InventoryManagement/_AdjustmentDocumentDetailsModalBody.cshtml", refreshedModel);
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
    public async Task<IActionResult> PostDocument(string documentId, string? serialSelectionsJson)
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
        var serialSelections = ParsePostDocumentSerialSelections(serialSelectionsJson);
        var result = await _apiService.PostInventoryDocumentAsync(documentId, postedBy, serialSelections, token);
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

    private static string? ValidateConversionDocumentForm(CreateInventoryDocumentForm form)
    {
        if (string.IsNullOrWhiteSpace(form.ConversionVariantId))
        {
            return "انتخاب واریانت برای سند تبدیل الزامی است.";
        }

        if (string.IsNullOrWhiteSpace(form.ConversionOperationType))
        {
            return "نوع عملیات تبدیل الزامی است.";
        }

        if (!string.Equals(form.ConversionOperationType, "Assemble", StringComparison.OrdinalIgnoreCase)
            && !string.Equals(form.ConversionOperationType, "Disassemble", StringComparison.OrdinalIgnoreCase))
        {
            return "نوع عملیات تبدیل باید Assemble یا Disassemble باشد.";
        }

        if (form.ConversionQuantity <= 0)
        {
            return "تعداد باید بزرگتر از صفر باشد.";
        }

        return null;
    }

    private async Task<(bool Success, string? Error, CreateInventoryDocumentForm? Form)> BuildConversionDocumentAsync(
        string token,
        CreateInventoryDocumentForm form)
    {
        var detailsResult = await _apiService.GetProductVariantFullDetailsAsync(form.ConversionVariantId ?? string.Empty, token);
        if (!detailsResult.IsSuccess || detailsResult.Data is null)
        {
            return (false, detailsResult.ErrorMessage ?? "جزئیات واریانت برای ساخت سند تبدیل دریافت نشد.", null);
        }

        var details = detailsResult.Data;
        var recipe = details.Components
            .Where(x =>
                !string.IsNullOrWhiteSpace(x.ComponentVariantId) &&
                !string.IsNullOrWhiteSpace(x.WarehouseId) &&
                !string.IsNullOrWhiteSpace(x.LocationId) &&
                x.Quantity > 0)
            .ToList();

        if (recipe.Count == 0)
        {
            return (false, "برای این واریانت recipe یا جزء سازنده معتبر ثبت نشده است.", null);
        }

        var assemble = string.Equals(form.ConversionOperationType, "Assemble", StringComparison.OrdinalIgnoreCase);
        return assemble
            ? await BuildConversionAssembleDocumentAsync(token, details, recipe, form.ConversionQuantity, form.ReasonCode)
            : await BuildConversionDisassembleDocumentAsync(token, details, recipe, form.ConversionQuantity, form.ReasonCode);
    }

    private async Task<(bool Success, string? Error, CreateInventoryDocumentForm? Form)> BuildConversionAssembleDocumentAsync(
        string token,
        ProductVariantDetailsModel variant,
        IReadOnlyCollection<VariantComponentModel> recipe,
        decimal quantity,
        string? reasonCode)
    {
        if (!TryResolveRecipeWarehouse(recipe, out var recipeWarehouseRef, out var warehouseError))
        {
            return (false, warehouseError, null);
        }

        var recipeLocations = recipe
            .Select(x => x.LocationId)
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        if (recipeLocations.Count != 1)
        {
            return (false, "برای عملیات مونتاژ، همه اجزای recipe باید در یک لوکیشن مشترک ثبت شده باشند.", null);
        }

        var componentBuckets = new Dictionary<string, List<StockDetailBucketModel>>(StringComparer.OrdinalIgnoreCase);
        foreach (var component in recipe)
        {
            if (!Guid.TryParse(component.ComponentVariantId, out var componentVariantRef))
            {
                return (false, "شناسه واریانت جزء معتبر نیست.", null);
            }

            if (!Guid.TryParse(component.LocationId, out var configuredLocationRef))
            {
                return (false, "شناسه لوکیشن جزء معتبر نیست.", null);
            }

            var bucketsResult = await _apiService.GetAvailableStockBucketsAsync(token, variantRef: componentVariantRef);
            if (!bucketsResult.IsSuccess)
            {
                return (false, bucketsResult.ErrorMessage ?? "موجودی اجزای recipe دریافت نشد.", null);
            }

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
        {
            return (false, "برای یکی از اجزای سازنده، موجودی کافی در انبار و لوکیشن recipe پیدا نشد.", null);
        }

        var sharedSellerQualities = sellerQualityGroups
            .Skip(1)
            .Aggregate(new HashSet<string>(sellerQualityGroups.First(), StringComparer.OrdinalIgnoreCase), (acc, set) =>
            {
                acc.IntersectWith(set);
                return acc;
            });

        if (sharedSellerQualities.Count == 0)
        {
            return (false, "اجزای سازنده در یک seller/quality مشترک برای مونتاژ پیدا نشدند.", null);
        }

        if (sharedSellerQualities.Count > 1)
        {
            return (false, "برای مونتاژ بیش از یک seller/quality ممکن پیدا شد. ابتدا یکی را یکتا کنید.", null);
        }

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
                {
                    break;
                }

                var take = Math.Min(bucket.QuantityOnHand, remaining);
                if (take <= 0)
                {
                    continue;
                }

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
            {
                return (false, "موجودی کافی برای یکی از اجزای سازنده جهت مونتاژ پیدا نشد.", null);
            }
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

    private async Task<(bool Success, string? Error, CreateInventoryDocumentForm? Form)> BuildConversionDisassembleDocumentAsync(
        string token,
        ProductVariantDetailsModel variant,
        IReadOnlyCollection<VariantComponentModel> recipe,
        decimal quantity,
        string? reasonCode)
    {
        if (!Guid.TryParse(variant.Id, out var recipeVariantRef))
        {
            return (false, "شناسه واریانت معتبر نیست.", null);
        }

        if (!TryResolveRecipeWarehouse(recipe, out var recipeWarehouseRef, out var warehouseError))
        {
            return (false, warehouseError, null);
        }

        var recipeBucketsResult = await _apiService.GetAvailableStockBucketsAsync(token, variantRef: recipeVariantRef);
        if (!recipeBucketsResult.IsSuccess)
        {
            return (false, recipeBucketsResult.ErrorMessage ?? "موجودی واریانت برای عملیات دیس‌اسمبل دریافت نشد.", null);
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
            return (false, "برای دیس‌اسمبل، موجودی کافی از واریانت انتخاب‌شده پیدا نشد.", null);
        }

        if (candidateSellerQualities.Count > 1)
        {
            return (false, "برای دیس‌اسمبل بیش از یک seller/quality ممکن پیدا شد. ابتدا یکی را یکتا کنید.", null);
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
            return (false, "موجودی کافی برای واریانت اصلی جهت دیس‌اسمبل پیدا نشد.", null);
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
            error = "برای عملیات تبدیل، همه اجزای recipe باید در یک انبار ثبت شده باشند.";
            return false;
        }

        if (!Guid.TryParse(warehouseIds[0], out warehouseRef))
        {
            error = "شناسه انبار recipe معتبر نیست.";
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
            ReceivedBy = isEditMode ? existingDocument?.ReceivedBy : null,
            DeliveredBy = isEditMode ? existingDocument?.DeliveredBy : null,
            OccurredAt = isEditMode ? existingDocument?.OccurredAt ?? DateTime.Now : DateTime.Now,
            ReasonCode = isEditMode ? existingDocument?.ReasonCode : null,
            ConversionOperationType = string.Equals(documentType, "Conversion", StringComparison.OrdinalIgnoreCase) ? "Assemble" : "Assemble",
            ConversionQuantity = 1m,
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
            AdjustmentDirection = line.AdjustmentDirection,
            Serials = line.Serials.Select(serial => new InventoryDocumentLineSerialModel
            {
                SerialItemBusinessKey = serial.SerialItemBusinessKey,
                SerialRef = serial.SerialRef,
                SerialNo = serial.SerialNo
            }).ToList()
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
        var qualityStatusLookupTask = _apiService.GetQualityStatusLookupAsync(token, includeInactive: false);
        var uomsTask = _apiService.GetUnitOfMeasureLookupAsync(token);

        await Task.WhenAll(variantsTask, productsTask, warehousesTask, sellersTask, locationsTask, qualityStatusLookupTask, uomsTask);

        var lineForm = BuildLineForm(document, documentId, null, "Receipt");
        if (string.IsNullOrWhiteSpace(lineForm.DestinationLocationRef))
        {
            lineForm.DestinationLocationRef = locationsTask.Result.Data?.FirstOrDefault()?.LocationBusinessKey ?? string.Empty;
        }

        if (string.IsNullOrWhiteSpace(lineForm.QualityStatusRef))
        {
            lineForm.QualityStatusRef = qualityStatusLookupTask.Result.Data?.FirstOrDefault()?.QualityStatusBusinessKey;
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
            QualityStatusLookup = qualityStatusLookupTask.Result.Data ?? new List<QualityStatusLookupItemModel>(),
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
        var qualityStatusLookupTask = _apiService.GetQualityStatusLookupAsync(token, includeInactive: false);
        var uomsTask = _apiService.GetUnitOfMeasureLookupAsync(token);

        await Task.WhenAll(variantsTask, productsTask, warehousesTask, sellersTask, locationsTask, qualityStatusLookupTask, uomsTask);

        var lineForm = BuildLineForm(document, documentId, null, "Transfer");
        if (string.IsNullOrWhiteSpace(lineForm.QualityStatusRef))
        {
            lineForm.QualityStatusRef = qualityStatusLookupTask.Result.Data?.FirstOrDefault()?.QualityStatusBusinessKey;
        }
        if (string.IsNullOrWhiteSpace(lineForm.FromQualityStatusRef))
        {
            lineForm.FromQualityStatusRef = lineForm.QualityStatusRef;
        }
        if (string.IsNullOrWhiteSpace(lineForm.ToQualityStatusRef))
        {
            lineForm.ToQualityStatusRef = lineForm.QualityStatusRef;
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
            QualityStatusLookup = qualityStatusLookupTask.Result.Data ?? new List<QualityStatusLookupItemModel>(),
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
        var qualityStatusLookupTask = _apiService.GetQualityStatusLookupAsync(token, includeInactive: false);
        var uomsTask = _apiService.GetUnitOfMeasureLookupAsync(token);

        await Task.WhenAll(variantsTask, productsTask, warehousesTask, sellersTask, locationsTask, qualityStatusLookupTask, uomsTask);

        var lineForm = BuildLineForm(document, documentId, null, "Issue");
        if (string.IsNullOrWhiteSpace(lineForm.QualityStatusRef))
        {
            lineForm.QualityStatusRef = qualityStatusLookupTask.Result.Data?.FirstOrDefault()?.QualityStatusBusinessKey;
        }
        if (string.IsNullOrWhiteSpace(lineForm.FromQualityStatusRef))
        {
            lineForm.FromQualityStatusRef = lineForm.QualityStatusRef;
        }
        if (string.IsNullOrWhiteSpace(lineForm.ToQualityStatusRef))
        {
            lineForm.ToQualityStatusRef = lineForm.QualityStatusRef;
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
            QualityStatusLookup = qualityStatusLookupTask.Result.Data ?? new List<QualityStatusLookupItemModel>(),
            UnitOfMeasureLookup = uomsTask.Result.Data ?? new List<UnitOfMeasureLookupModel>(),
            LineForm = lineForm
        };
    }

    private async Task<InventoryDocumentManagementPageViewModel> BuildAdjustmentDocumentDetailsModalModelAsync(
        string documentId,
        string token,
        string? editingLineId = null,
        CancellationToken cancellationToken = default)
    {
        var documentResult = await _apiService.GetInventoryDocumentByBusinessKeyAsync(documentId, token);
        var document = documentResult.Data;

        var variantsTask = _apiService.SearchVariantsAsync(token, page: 1, pageSize: 2000);
        var productsTask = _apiService.SearchProductsAsync(token, page: 1, pageSize: 2000);
        var warehousesTask = _apiService.GetWarehouseLookupAsync(token, includeInactive: true);
        var sellersTask = _apiService.GetSellerLookupAsync(token, includeInactive: true);
        var locationsTask = _apiService.GetLocationLookupAsync(token, warehouseId: null, includeInactive: true);
        var qualityStatusLookupTask = _apiService.GetQualityStatusLookupAsync(token, includeInactive: false);
        var uomsTask = _apiService.GetUnitOfMeasureLookupAsync(token);

        await Task.WhenAll(variantsTask, productsTask, warehousesTask, sellersTask, locationsTask, qualityStatusLookupTask, uomsTask);

        var lineForm = BuildLineForm(document, documentId, editingLineId, "Adjustment");
        if (string.IsNullOrWhiteSpace(lineForm.QualityStatusRef))
        {
            lineForm.QualityStatusRef = qualityStatusLookupTask.Result.Data?.FirstOrDefault()?.QualityStatusBusinessKey;
        }
        if (string.IsNullOrWhiteSpace(lineForm.WarehouseRef) && !string.IsNullOrWhiteSpace(lineForm.SourceLocationRef))
        {
            lineForm.WarehouseRef = locationsTask.Result.Data?
                .FirstOrDefault(x => string.Equals(x.LocationBusinessKey, lineForm.SourceLocationRef, StringComparison.OrdinalIgnoreCase))
                ?.WarehouseRef;
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
            QualityStatusLookup = qualityStatusLookupTask.Result.Data ?? new List<QualityStatusLookupItemModel>(),
            UnitOfMeasureLookup = uomsTask.Result.Data ?? new List<UnitOfMeasureLookupModel>(),
            LineForm = lineForm
        };
    }

    private async Task<InventoryDocumentManagementPageViewModel> BuildReturnDocumentDetailsModalModelAsync(
        string documentId,
        string token,
        string? editingLineId = null,
        CancellationToken cancellationToken = default)
    {
        var documentResult = await _apiService.GetInventoryDocumentByBusinessKeyAsync(documentId, token);
        var document = documentResult.Data;

        var variantsTask = _apiService.SearchVariantsAsync(token, page: 1, pageSize: 2000);
        var productsTask = _apiService.SearchProductsAsync(token, page: 1, pageSize: 2000);
        var warehousesTask = _apiService.GetWarehouseLookupAsync(token, includeInactive: true);
        var sellersTask = _apiService.GetSellerLookupAsync(token, includeInactive: true);
        var locationsTask = _apiService.GetLocationLookupAsync(token, warehouseId: null, includeInactive: true);
        var qualityStatusLookupTask = _apiService.GetQualityStatusLookupAsync(token, includeInactive: false);
        var uomsTask = _apiService.GetUnitOfMeasureLookupAsync(token);

        await Task.WhenAll(variantsTask, productsTask, warehousesTask, sellersTask, locationsTask, qualityStatusLookupTask, uomsTask);

        var lineForm = BuildLineForm(document, documentId, editingLineId, "Return");
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
            QualityStatusLookup = qualityStatusLookupTask.Result.Data ?? new List<QualityStatusLookupItemModel>(),
            UnitOfMeasureLookup = uomsTask.Result.Data ?? new List<UnitOfMeasureLookupModel>(),
            LineForm = lineForm
        };
    }

    private static readonly JsonSerializerOptions PostDocumentSerialSelectionsJsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private static string BuildTemporaryPassword()
    {
        var suffix = Random.Shared.Next(1000, 9999);
        return $"Tmp@{suffix}A1";
    }

    private static List<PostDocumentLineSerialSelectionModel>? ParsePostDocumentSerialSelections(string? serialSelectionsJson)
    {
        if (string.IsNullOrWhiteSpace(serialSelectionsJson))
        {
            return null;
        }

        try
        {
            var selections = JsonSerializer.Deserialize<List<PostDocumentLineSerialSelectionModel>>(serialSelectionsJson, PostDocumentSerialSelectionsJsonOptions);
            if (selections is null || selections.Count == 0)
            {
                return null;
            }

            foreach (var selection in selections)
            {
                selection.DocumentLineBusinessKey = selection.DocumentLineBusinessKey?.Trim() ?? string.Empty;
                selection.Serials ??= new List<PostDocumentSerialItemModel>();
                selection.Serials = selection.Serials
                    .Where(serial => !string.IsNullOrWhiteSpace(serial.SerialItemBusinessKey) || !string.IsNullOrWhiteSpace(serial.SerialNo))
                    .Select(serial => new PostDocumentSerialItemModel
                    {
                        SerialItemBusinessKey = serial.SerialItemBusinessKey?.Trim() ?? string.Empty,
                        SerialNo = serial.SerialNo?.Trim() ?? string.Empty
                    })
                    .ToList();
            }

            return selections
                .Where(selection => !string.IsNullOrWhiteSpace(selection.DocumentLineBusinessKey))
                .ToList();
        }
        catch
        {
            return null;
        }
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
