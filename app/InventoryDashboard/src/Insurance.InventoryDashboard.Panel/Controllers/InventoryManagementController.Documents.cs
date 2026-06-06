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
        CancellationToken cancellationToken = default,
        string? returnReferenceDocumentType = null)
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

    [HttpGet("/InventoryManagement/Documents/VariantLookup")]
    public async Task<IActionResult> SearchVariantLookup(string? term, string? documentId, CancellationToken cancellationToken = default)
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

        Guid? referenceDocumentBusinessKey = null;
        if (!string.IsNullOrWhiteSpace(documentId))
        {
            if (!Guid.TryParse(documentId, out var parsedDocumentId))
            {
                return Json(new
                {
                    isSuccess = false,
                    errorMessage = "سند مبنای انتخاب‌شده معتبر نیست."
                });
            }

            referenceDocumentBusinessKey = parsedDocumentId;
        }

        var lookupPageSize = referenceDocumentBusinessKey.HasValue ? 200 : 20;
        var result = await _apiService.SearchProductVariantsAsync(
            token,
            searchTerm: term,
            isActive: true,
            page: 1,
            pageSize: lookupPageSize);

        if (!result.IsSuccess)
        {
            return Json(new
            {
                isSuccess = false,
                errorMessage = result.ErrorMessage
            });
        }

        var items = result.Data?.Items ?? new List<ProductVariantSummaryModel>();
        if (referenceDocumentBusinessKey.HasValue)
        {
            var documentResult = await _apiService.GetInventoryDocumentByBusinessKeyAsync(referenceDocumentBusinessKey.Value.ToString("D"), token);
            if (!documentResult.IsSuccess || documentResult.Data is null)
            {
                return Json(new
                {
                    isSuccess = false,
                    errorMessage = documentResult.ErrorMessage ?? "سند مبنا یافت نشد."
                });
            }

            var allowedVariantIds = documentResult.Data.Lines
                .Select(x => x.VariantRef)
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(x => x.Trim())
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            items = items
                .Where(x => allowedVariantIds.Contains(x.Id))
                .ToList();
        }

        return Json(new
        {
            isSuccess = true,
            items = items
                .Select(x => new
                {
                    id = x.Id,
                    text = BuildVariantLookupLabel(x),
                    sku = x.Sku,
                    name = x.Name,
                    barcode = x.Barcode,
                    productId = x.ProductId,
                    baseUomRef = x.BaseUomRef
                })
                .ToList()
        });
    }

    [HttpGet("/InventoryManagement/Documents/Receipt/VariantLookup")]
    public Task<IActionResult> SearchReceiptVariantLookup(string? term, string? documentId, CancellationToken cancellationToken = default)
        => SearchVariantLookup(term, documentId, cancellationToken);

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
    public async Task<IActionResult> SearchTransferVariantInventoryLookup(string variantId, string? qualityStatusId, CancellationToken cancellationToken = default)
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

        var stockBuckets = (stockBucketsResult.Data?.Items ?? new List<StockDetailBucketModel>()).ToList();
        if (Guid.TryParse(qualityStatusId, out var parsedQualityStatusId))
        {
            var qualityKey = parsedQualityStatusId.ToString("D");
            stockBuckets = stockBuckets
                .Where(x => string.Equals(x.QualityStatusRef.ToString("D"), qualityKey, StringComparison.OrdinalIgnoreCase))
                .ToList();
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

        var allowedLocationIds = stockBuckets
            .Select(x => x.LocationRef.ToString("D"))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        var buckets = stockBuckets.Select(bucket => new
        {
            warehouseRef = bucket.WarehouseRef.ToString("D"),
            locationRef = bucket.LocationRef.ToString("D"),
            qualityStatusRef = bucket.QualityStatusRef.ToString("D"),
            lotBatchNo = bucket.LotBatchNo,
            quantityOnHand = bucket.QuantityOnHand
        }).ToList();

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
            allowedLocationIds,
            locations,
            buckets
        });
    }

    [HttpGet("/InventoryManagement/Documents/Transfer/VariantSerialLookup")]
    public async Task<IActionResult> SearchTransferVariantSerialLookup(
        string variantId,
        string? sourceLocationId,
        string? qualityStatusId,
        string? lotBatchNo,
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

        if (Guid.TryParse(qualityStatusId, out var parsedQualityStatusId))
        {
            var qualityStatusKey = parsedQualityStatusId.ToString("D");
            filteredSerials = filteredSerials
                .Where(x => string.Equals(x.QualityStatusRef, qualityStatusKey, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        var normalizedLotBatchNo = NormalizeLotBatchNo(lotBatchNo);
        if (!string.IsNullOrWhiteSpace(normalizedLotBatchNo))
        {
            filteredSerials = filteredSerials
                .Where(x => string.Equals(NormalizeLotBatchNo(x.LotBatchNo), normalizedLotBatchNo, StringComparison.OrdinalIgnoreCase))
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
        form.LotBatchNo = NormalizeLotBatchNo(form.LotBatchNo) ?? ResolveReceiptDocumentLotBatchNo(document);
        var receiptSerialError = PrepareReceiptLineSerials(document, form);
        if (receiptSerialError is not null)
        {
            var invalidSerialModel = await BuildReceiptDocumentDetailsModalModelAsync(form.DocumentId, token, cancellationToken);
            invalidSerialModel.ErrorMessage = receiptSerialError;
            invalidSerialModel.LineForm = form;
            return PartialView("~/Views/InventoryManagement/_ReceiptDocumentDetailsModalBody.cshtml", invalidSerialModel);
        }

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

        if (string.IsNullOrWhiteSpace(form.DestinationLocationRef))
        {
            var invalidLocationModel = await BuildTransferDocumentDetailsModalModelAsync(form.DocumentId, token);
            invalidLocationModel.ErrorMessage = "برای سند انتقال، لوکیشن مقصد الزامی است.";
            invalidLocationModel.LineForm = form;
            return PartialView("~/Views/InventoryManagement/_TransferDocumentDetailsModalBody.cshtml", invalidLocationModel);
        }

        if (string.IsNullOrWhiteSpace(form.ToQualityStatusRef))
        {
            var invalidQualityModel = await BuildTransferDocumentDetailsModalModelAsync(form.DocumentId, token);
            invalidQualityModel.ErrorMessage = "برای سند انتقال، وضعیت کیفیت مقصد الزامی است.";
            invalidQualityModel.LineForm = form;
            return PartialView("~/Views/InventoryManagement/_TransferDocumentDetailsModalBody.cshtml", invalidQualityModel);
        }

        var serialsResult = await _apiService.GetAvailableSerialItemsAsync(token, variant.Id);
        if (!serialsResult.IsSuccess)
        {
            var serialLookupModel = await BuildTransferDocumentDetailsModalModelAsync(form.DocumentId, token);
            serialLookupModel.ErrorMessage = serialsResult.ErrorMessage ?? "Ø¨Ø§Ø±Ú¯Ø°Ø§Ø±ÛŒ Ø³Ø±ÛŒØ§Ù„â€ŒÙ‡Ø§ÛŒ ÙˆØ§Ø±ÛŒØ§Ù†Øª Ø§Ù†Ø¬Ø§Ù… Ù†Ø´Ø¯.";
            serialLookupModel.LineForm = form;
            return PartialView("~/Views/InventoryManagement/_TransferDocumentDetailsModalBody.cshtml", serialLookupModel);
        }

        var availableSerialsForSource = FilterAvailableSerialsForLine(
            serialsResult.Data ?? new List<SerialItemLookupModel>(),
            form.SourceLocationRef,
            form.QualityStatusRef,
            form.LotBatchNo);

        if (!TryResolveSelectedSerials(form.Serials, availableSerialsForSource, out var resolvedSerialSelections, out var selectedSerialError))
        {
            var serialSelectionModel = await BuildTransferDocumentDetailsModalModelAsync(form.DocumentId, token);
            serialSelectionModel.ErrorMessage = selectedSerialError ?? "انتخاب سریال‌ها معتبر نیست.";
            serialSelectionModel.LineForm = form;
            return PartialView("~/Views/InventoryManagement/_TransferDocumentDetailsModalBody.cshtml", serialSelectionModel);
        }

        if (resolvedSerialSelections.Count == 0)
        {
            var autoAllocationResult = await TryAutoAllocateSourceAllocationsAsync(token, document, variant, form);
            if (!autoAllocationResult.Success)
            {
                var autoAllocateModel = await BuildTransferDocumentDetailsModalModelAsync(form.DocumentId, token);
                autoAllocateModel.ErrorMessage = autoAllocationResult.ErrorMessage ?? "امکان تخصیص خودکار منبع وجود نداشت.";
                autoAllocateModel.LineForm = form;
                return PartialView("~/Views/InventoryManagement/_TransferDocumentDetailsModalBody.cshtml", autoAllocateModel);
            }

            if (autoAllocationResult.Allocations.Count > 0)
            {
                if (!string.IsNullOrWhiteSpace(form.LineId) && autoAllocationResult.Allocations.Count > 1)
                {
                    var splitLineErrorModel = await BuildTransferDocumentDetailsModalModelAsync(form.DocumentId, token);
                    splitLineErrorModel.ErrorMessage = "برای ویرایش آیتم، تخصیص خودکار از چند منبع پشتیبانی نمی‌شود. لطفاً مورد را حذف و دوباره اضافه کنید.";
                    splitLineErrorModel.LineForm = form;
                    return PartialView("~/Views/InventoryManagement/_TransferDocumentDetailsModalBody.cshtml", splitLineErrorModel);
                }

                if (autoAllocationResult.Allocations.Count > 1 && string.IsNullOrWhiteSpace(form.LineId))
                {
                    foreach (var allocation in autoAllocationResult.Allocations)
                    {
                        var splitForm = CloneInventoryDocumentLineForm(form);
                        splitForm.LineId = string.Empty;
                        splitForm.Qty = allocation.BaseQty;
                        splitForm.SourceLocationRef = allocation.Bucket.LocationRef.ToString("D");
                        splitForm.QualityStatusRef = allocation.Bucket.QualityStatusRef.ToString("D");
                        splitForm.FromQualityStatusRef = splitForm.QualityStatusRef;
                        splitForm.LotBatchNo = allocation.Bucket.LotBatchNo;
                        splitForm.Serials = allocation.Serials
                            .Select(item => new InventoryDocumentLineSerialModel
                            {
                                SerialItemBusinessKey = item.SerialItemBusinessKey,
                                SerialRef = item.SerialItemBusinessKey,
                                SerialNo = item.SerialNo
                            })
                            .ToList();

                        var splitResult = await _apiService.AddInventoryDocumentLineAsync(splitForm.DocumentId, splitForm, token);
                        if (!splitResult.IsSuccess)
                        {
                            var splitFailureModel = await BuildTransferDocumentDetailsModalModelAsync(form.DocumentId, token, cancellationToken);
                            splitFailureModel.ErrorMessage = splitResult.ErrorMessage ?? "ذخیره آیتم سند انجام نشد.";
                            splitFailureModel.LineForm = form;
                            return PartialView("~/Views/InventoryManagement/_TransferDocumentDetailsModalBody.cshtml", splitFailureModel);
                        }
                    }

                    var refreshedSplitModel = await BuildTransferDocumentDetailsModalModelAsync(form.DocumentId, token, cancellationToken);
                    return PartialView("~/Views/InventoryManagement/_TransferDocumentDetailsModalBody.cshtml", refreshedSplitModel);
                }

                var resolvedAllocation = autoAllocationResult.Allocations[0];
                form.SourceLocationRef = resolvedAllocation.Bucket.LocationRef.ToString("D");
                form.QualityStatusRef = resolvedAllocation.Bucket.QualityStatusRef.ToString("D");
                form.FromQualityStatusRef = form.QualityStatusRef;
                form.LotBatchNo = resolvedAllocation.Bucket.LotBatchNo;
                form.Qty = resolvedAllocation.BaseQty;
                form.Serials = resolvedAllocation.Serials
                    .Select(item => new InventoryDocumentLineSerialModel
                    {
                        SerialItemBusinessKey = item.SerialItemBusinessKey,
                        SerialRef = item.SerialItemBusinessKey,
                        SerialNo = item.SerialNo
                    })
                    .ToList();
            }
        }

        if (resolvedSerialSelections.Count > 0)
        {
            var selectedGroups = resolvedSerialSelections
                .GroupBy(x => new
                {
                    LocationRef = NormalizeLookupKey(x.AvailableSerial.LocationRef),
                    LotBatchNo = NormalizeLookupKey(NormalizeLotBatchNo(x.AvailableSerial.LotBatchNo)),
                    QualityStatusRef = NormalizeLookupKey(x.AvailableSerial.QualityStatusRef)
                })
                .ToList();

            if (!string.IsNullOrWhiteSpace(form.LineId) && selectedGroups.Count > 1)
            {
                var splitLineErrorModel = await BuildTransferDocumentDetailsModalModelAsync(form.DocumentId, token);
                splitLineErrorModel.ErrorMessage = "برای ویرایش آیتم، انتخاب سریال‌ها از چند لات پشتیبانی نمی‌شود. لطفاً مورد را حذف و دوباره اضافه کنید.";
                splitLineErrorModel.LineForm = form;
                return PartialView("~/Views/InventoryManagement/_TransferDocumentDetailsModalBody.cshtml", splitLineErrorModel);
            }

            if (selectedGroups.Count > 1 && !string.IsNullOrWhiteSpace(NormalizeLotBatchNo(form.LotBatchNo)))
            {
                var lotMismatchModel = await BuildTransferDocumentDetailsModalModelAsync(form.DocumentId, token);
                lotMismatchModel.ErrorMessage = "در انتخاب همزمان سریال‌های چند لات، فیلد لات باید خالی بماند تا سیستم ردیف‌ها را تفکیک کند.";
                lotMismatchModel.LineForm = form;
                return PartialView("~/Views/InventoryManagement/_TransferDocumentDetailsModalBody.cshtml", lotMismatchModel);
            }

            if (selectedGroups.Count > 1 && string.IsNullOrWhiteSpace(form.LineId))
            {
                foreach (var group in selectedGroups)
                {
                    var splitForm = CloneInventoryDocumentLineForm(form);
                    splitForm.LineId = string.Empty;
                    splitForm.Qty = group.Count();
                    splitForm.SourceLocationRef = string.IsNullOrWhiteSpace(group.Key.LocationRef) ? null : group.Key.LocationRef;
                    splitForm.LotBatchNo = string.IsNullOrWhiteSpace(group.Key.LotBatchNo) ? null : NormalizeLotBatchNo(group.Key.LotBatchNo);
                    splitForm.QualityStatusRef = string.IsNullOrWhiteSpace(group.Key.QualityStatusRef) ? null : group.Key.QualityStatusRef;
                    splitForm.FromQualityStatusRef = splitForm.QualityStatusRef;
                    splitForm.Serials = group
                        .Select(item => new InventoryDocumentLineSerialModel
                        {
                            SerialItemBusinessKey = item.AvailableSerial.SerialItemBusinessKey,
                            SerialRef = item.AvailableSerial.SerialItemBusinessKey,
                            SerialNo = item.AvailableSerial.SerialNo
                        })
                        .ToList();

                    var splitResult = await _apiService.AddInventoryDocumentLineAsync(splitForm.DocumentId, splitForm, token);
                    if (!splitResult.IsSuccess)
                    {
                        var splitFailureModel = await BuildTransferDocumentDetailsModalModelAsync(form.DocumentId, token, cancellationToken);
                        splitFailureModel.ErrorMessage = splitResult.ErrorMessage ?? "ذخیره آیتم سند انجام نشد.";
                        splitFailureModel.LineForm = form;
                        return PartialView("~/Views/InventoryManagement/_TransferDocumentDetailsModalBody.cshtml", splitFailureModel);
                    }
                }

                var refreshedSplitModel = await BuildTransferDocumentDetailsModalModelAsync(form.DocumentId, token, cancellationToken);
                return PartialView("~/Views/InventoryManagement/_TransferDocumentDetailsModalBody.cshtml", refreshedSplitModel);
            }

            var resolvedSerialGroup = selectedGroups[0];
            if (!string.IsNullOrWhiteSpace(form.SourceLocationRef) && !string.Equals(NormalizeLookupKey(form.SourceLocationRef), resolvedSerialGroup.Key.LocationRef, StringComparison.OrdinalIgnoreCase))
            {
                var locationMismatchModel = await BuildTransferDocumentDetailsModalModelAsync(form.DocumentId, token);
                locationMismatchModel.ErrorMessage = "انتخاب لوکیشن با سریال‌های انتخاب شده هم‌خوانی ندارد.";
                locationMismatchModel.LineForm = form;
                return PartialView("~/Views/InventoryManagement/_TransferDocumentDetailsModalBody.cshtml", locationMismatchModel);
            }

            var resolvedLotBatchNo = NormalizeLotBatchNo(form.LotBatchNo) ?? NormalizeLotBatchNo(resolvedSerialGroup.Key.LotBatchNo);
            if (!string.IsNullOrWhiteSpace(form.LotBatchNo) && !string.Equals(NormalizeLookupKey(form.LotBatchNo), resolvedSerialGroup.Key.LotBatchNo, StringComparison.OrdinalIgnoreCase))
            {
                var lotMismatchModel = await BuildTransferDocumentDetailsModalModelAsync(form.DocumentId, token);
                lotMismatchModel.ErrorMessage = "انتخاب لات با سریال‌های انتخاب شده هم‌خوانی ندارد.";
                lotMismatchModel.LineForm = form;
                return PartialView("~/Views/InventoryManagement/_TransferDocumentDetailsModalBody.cshtml", lotMismatchModel);
            }

            if (!string.IsNullOrWhiteSpace(form.QualityStatusRef) && !string.Equals(NormalizeLookupKey(form.QualityStatusRef), resolvedSerialGroup.Key.QualityStatusRef, StringComparison.OrdinalIgnoreCase))
            {
                var qualityMismatchModel = await BuildTransferDocumentDetailsModalModelAsync(form.DocumentId, token);
                qualityMismatchModel.ErrorMessage = "انتخاب کیفیت با سریال‌های انتخاب شده هم‌خوانی ندارد.";
                qualityMismatchModel.LineForm = form;
                return PartialView("~/Views/InventoryManagement/_TransferDocumentDetailsModalBody.cshtml", qualityMismatchModel);
            }

            form.SourceLocationRef = string.IsNullOrWhiteSpace(resolvedSerialGroup.Key.LocationRef) ? null : resolvedSerialGroup.Key.LocationRef;
            form.LotBatchNo = resolvedLotBatchNo;
            var resolvedQualityStatusRef = string.IsNullOrWhiteSpace(form.QualityStatusRef)
                ? resolvedSerialGroup.Key.QualityStatusRef
                : form.QualityStatusRef;
            form.QualityStatusRef = string.IsNullOrWhiteSpace(resolvedQualityStatusRef) ? null : resolvedQualityStatusRef;
            form.FromQualityStatusRef = form.QualityStatusRef;
            form.Qty = resolvedSerialSelections.Count;
            form.Serials = resolvedSerialGroup
                .Select(item => new InventoryDocumentLineSerialModel
                {
                    SerialItemBusinessKey = item.AvailableSerial.SerialItemBusinessKey,
                    SerialRef = item.AvailableSerial.SerialItemBusinessKey,
                    SerialNo = item.AvailableSerial.SerialNo
                })
                .ToList();
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

        var buckets = (stockBucketsResult.Data?.Items ?? new List<StockDetailBucketModel>())
            .Select(bucket => new
            {
                warehouseRef = bucket.WarehouseRef.ToString("D"),
                locationRef = bucket.LocationRef.ToString("D"),
                qualityStatusRef = bucket.QualityStatusRef.ToString("D"),
                lotBatchNo = bucket.LotBatchNo,
                quantityOnHand = bucket.QuantityOnHand
            })
            .ToList();

        return Json(new
        {
            isSuccess = true,
            locations,
            buckets
        });
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

    [HttpGet("/InventoryManagement/Documents/Adjustment/IncreaseSerialLookup")]
    public async Task<IActionResult> SearchAdjustmentIncreaseSerialLookup(
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

        if (string.IsNullOrWhiteSpace(locationId))
        {
            return Json(new
            {
                isSuccess = false,
                errorMessage = "برای افزایش، لوکیشن مقصد الزامی است."
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

        var locationResult = await _apiService.GetLocationByBusinessKeyAsync(locationId, token);
        if (!locationResult.IsSuccess || locationResult.Data is null)
        {
            return Json(new
            {
                isSuccess = false,
                errorMessage = locationResult.ErrorMessage ?? "اطلاعات لوکیشن یافت نشد."
            });
        }

        var warehouseId = locationResult.Data.WarehouseId;
        var serialsResult = await _apiService.SearchSerialItemsAsync(
            token,
            parsedVariantId.ToString("D"),
            warehouseId: warehouseId,
            status: "Issued");
        if (!serialsResult.IsSuccess)
        {
            return Json(new { isSuccess = false, errorMessage = serialsResult.ErrorMessage });
        }

        var serials = (serialsResult.Data ?? new List<SerialItemLookupModel>())
            .Where(x => string.Equals(x.LocationRef, locationId, StringComparison.OrdinalIgnoreCase))
            .ToList();

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
    public async Task<IActionResult> SearchIssueVariantInventoryLookup(string variantId, string? qualityStatusId, CancellationToken cancellationToken = default)
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

        var stockBuckets = (stockBucketsResult.Data?.Items ?? new List<StockDetailBucketModel>()).ToList();
        if (Guid.TryParse(qualityStatusId, out var parsedQualityStatusId))
        {
            var qualityKey = parsedQualityStatusId.ToString("D");
            stockBuckets = stockBuckets
                .Where(x => string.Equals(x.QualityStatusRef.ToString("D"), qualityKey, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        var allowedLocationIds = stockBuckets
            .Select(x => x.LocationRef.ToString("D"))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        var buckets = stockBuckets
            .Select(bucket => new
            {
                warehouseRef = bucket.WarehouseRef.ToString("D"),
                locationRef = bucket.LocationRef.ToString("D"),
                qualityStatusRef = bucket.QualityStatusRef.ToString("D"),
                lotBatchNo = bucket.LotBatchNo,
                quantityOnHand = bucket.QuantityOnHand
            })
            .ToList();

        return Json(new
        {
            isSuccess = true,
            allowedLocationIds,
            buckets
        });
    }

    [HttpGet("/InventoryManagement/Documents/Issue/VariantSerialLookup")]
    public async Task<IActionResult> SearchIssueVariantSerialLookup(
        string variantId,
        string? sourceLocationId,
        string? qualityStatusId,
        string? lotBatchNo,
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

        if (Guid.TryParse(qualityStatusId, out var parsedQualityStatusId))
        {
            var qualityStatusKey = parsedQualityStatusId.ToString("D");
            filteredSerials = filteredSerials
                .Where(x => string.Equals(x.QualityStatusRef, qualityStatusKey, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        var normalizedLotBatchNo = NormalizeLotBatchNo(lotBatchNo);
        if (!string.IsNullOrWhiteSpace(normalizedLotBatchNo))
        {
            filteredSerials = filteredSerials
                .Where(x => string.Equals(NormalizeLotBatchNo(x.LotBatchNo), normalizedLotBatchNo, StringComparison.OrdinalIgnoreCase))
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

        var serialsResult = await _apiService.GetAvailableSerialItemsAsync(token, variant.Id);
        if (!serialsResult.IsSuccess)
        {
            var serialLookupModel = await BuildIssueDocumentDetailsModalModelAsync(form.DocumentId, token);
            serialLookupModel.ErrorMessage = serialsResult.ErrorMessage ?? "Ø¨Ø§Ø±Ú¯Ø°Ø§Ø±ÛŒ Ø³Ø±ÛŒØ§Ù„â€ŒÙ‡Ø§ÛŒ ÙˆØ§Ø±ÛŒØ§Ù†Øª Ø§Ù†Ø¬Ø§ÙÙ… Ù†Ø´Ø¯.";
            serialLookupModel.LineForm = form;
            return PartialView("~/Views/InventoryManagement/_IssueDocumentDetailsModalBody.cshtml", serialLookupModel);
        }

        var availableSerialsForSource = FilterAvailableSerialsForLine(
            serialsResult.Data ?? new List<SerialItemLookupModel>(),
            form.SourceLocationRef,
            form.QualityStatusRef,
            form.LotBatchNo);

        if (!TryResolveSelectedSerials(form.Serials, availableSerialsForSource, out var resolvedSerialSelections, out var selectedSerialError))
        {
            var serialSelectionModel = await BuildIssueDocumentDetailsModalModelAsync(form.DocumentId, token);
            serialSelectionModel.ErrorMessage = selectedSerialError ?? "انتخاب سریال‌ها معتبر نیست.";
            serialSelectionModel.LineForm = form;
            return PartialView("~/Views/InventoryManagement/_IssueDocumentDetailsModalBody.cshtml", serialSelectionModel);
        }

        if (resolvedSerialSelections.Count == 0)
        {
            var autoAllocationResult = await TryAutoAllocateSourceAllocationsAsync(token, document, variant, form);
            if (!autoAllocationResult.Success)
            {
                var autoAllocateModel = await BuildIssueDocumentDetailsModalModelAsync(form.DocumentId, token);
                autoAllocateModel.ErrorMessage = autoAllocationResult.ErrorMessage ?? "امکان تخصیص خودکار منبع وجود نداشت.";
                autoAllocateModel.LineForm = form;
                return PartialView("~/Views/InventoryManagement/_IssueDocumentDetailsModalBody.cshtml", autoAllocateModel);
            }

            if (autoAllocationResult.Allocations.Count > 0)
            {
                if (!string.IsNullOrWhiteSpace(form.LineId) && autoAllocationResult.Allocations.Count > 1)
                {
                    var splitLineErrorModel = await BuildIssueDocumentDetailsModalModelAsync(form.DocumentId, token);
                    splitLineErrorModel.ErrorMessage = "برای ویرایش آیتم، تخصیص خودکار از چند منبع پشتیبانی نمی‌شود. لطفاً مورد را حذف و دوباره اضافه کنید.";
                    splitLineErrorModel.LineForm = form;
                    return PartialView("~/Views/InventoryManagement/_IssueDocumentDetailsModalBody.cshtml", splitLineErrorModel);
                }

                if (autoAllocationResult.Allocations.Count > 1 && string.IsNullOrWhiteSpace(form.LineId))
                {
                    foreach (var allocation in autoAllocationResult.Allocations)
                    {
                        var splitForm = CloneInventoryDocumentLineForm(form);
                        splitForm.LineId = string.Empty;
                        splitForm.Qty = allocation.BaseQty;
                        splitForm.SourceLocationRef = allocation.Bucket.LocationRef.ToString("D");
                        splitForm.QualityStatusRef = allocation.Bucket.QualityStatusRef.ToString("D");
                        splitForm.LotBatchNo = allocation.Bucket.LotBatchNo;
                        splitForm.Serials = allocation.Serials
                            .Select(item => new InventoryDocumentLineSerialModel
                            {
                                SerialItemBusinessKey = item.SerialItemBusinessKey,
                                SerialNo = item.SerialNo
                            })
                            .ToList();

                        var splitResult = await _apiService.AddInventoryDocumentLineAsync(splitForm.DocumentId, splitForm, token);
                        if (!splitResult.IsSuccess)
                        {
                            var splitFailureModel = await BuildIssueDocumentDetailsModalModelAsync(form.DocumentId, token, cancellationToken);
                            splitFailureModel.ErrorMessage = splitResult.ErrorMessage ?? "ذخیره آیتم سند انجام نشد.";
                            splitFailureModel.LineForm = form;
                            return PartialView("~/Views/InventoryManagement/_IssueDocumentDetailsModalBody.cshtml", splitFailureModel);
                        }
                    }

                    var refreshedSplitModel = await BuildIssueDocumentDetailsModalModelAsync(form.DocumentId, token, cancellationToken);
                    return PartialView("~/Views/InventoryManagement/_IssueDocumentDetailsModalBody.cshtml", refreshedSplitModel);
                }

                var resolvedAllocation = autoAllocationResult.Allocations[0];
                form.SourceLocationRef = resolvedAllocation.Bucket.LocationRef.ToString("D");
                form.QualityStatusRef = resolvedAllocation.Bucket.QualityStatusRef.ToString("D");
                form.LotBatchNo = resolvedAllocation.Bucket.LotBatchNo;
                form.Qty = resolvedAllocation.BaseQty;
                form.Serials = resolvedAllocation.Serials
                    .Select(item => new InventoryDocumentLineSerialModel
                    {
                        SerialItemBusinessKey = item.SerialItemBusinessKey,
                        SerialNo = item.SerialNo
                    })
                    .ToList();
            }
        }        if (resolvedSerialSelections.Count > 0)
        {
            var selectedGroups = resolvedSerialSelections
                .GroupBy(x => new
                {
                    LocationRef = NormalizeLookupKey(x.AvailableSerial.LocationRef),
                    LotBatchNo = NormalizeLookupKey(NormalizeLotBatchNo(x.AvailableSerial.LotBatchNo)),
                    QualityStatusRef = NormalizeLookupKey(x.AvailableSerial.QualityStatusRef)
                })
                .ToList();

            if (!string.IsNullOrWhiteSpace(form.LineId) && selectedGroups.Count > 1)
            {
                var splitLineErrorModel = await BuildIssueDocumentDetailsModalModelAsync(form.DocumentId, token);
                splitLineErrorModel.ErrorMessage = "برای ویرایش آیتم، انتخاب سریال‌ها از چند لات پشتیبانی نمی‌شود. لطفاً مورد را حذف و دوباره اضافه کنید.";
                splitLineErrorModel.LineForm = form;
                return PartialView("~/Views/InventoryManagement/_IssueDocumentDetailsModalBody.cshtml", splitLineErrorModel);
            }

            if (selectedGroups.Count > 1 && !string.IsNullOrWhiteSpace(NormalizeLotBatchNo(form.LotBatchNo)))
            {
                var lotMismatchModel = await BuildIssueDocumentDetailsModalModelAsync(form.DocumentId, token);
                lotMismatchModel.ErrorMessage = "در انتخاب همزمان سریال‌های چند لات، فیلد لات باید خالی بماند تا سیستم ردیف‌ها را تفکیک کند.";
                lotMismatchModel.LineForm = form;
                return PartialView("~/Views/InventoryManagement/_IssueDocumentDetailsModalBody.cshtml", lotMismatchModel);
            }

            if (selectedGroups.Count > 1 && string.IsNullOrWhiteSpace(form.LineId))
            {
                foreach (var group in selectedGroups)
                {
                    var splitForm = CloneInventoryDocumentLineForm(form);
                    splitForm.LineId = string.Empty;
                    splitForm.Qty = group.Count();
                    splitForm.SourceLocationRef = string.IsNullOrWhiteSpace(group.Key.LocationRef) ? null : group.Key.LocationRef;
                    splitForm.LotBatchNo = string.IsNullOrWhiteSpace(group.Key.LotBatchNo) ? null : NormalizeLotBatchNo(group.Key.LotBatchNo);
                    splitForm.QualityStatusRef = string.IsNullOrWhiteSpace(group.Key.QualityStatusRef) ? null : group.Key.QualityStatusRef;
                    splitForm.FromQualityStatusRef = splitForm.QualityStatusRef;
                    splitForm.Serials = group
                        .Select(item => new InventoryDocumentLineSerialModel
                        {
                            SerialItemBusinessKey = item.AvailableSerial.SerialItemBusinessKey,
                            SerialNo = item.AvailableSerial.SerialNo
                        })
                        .ToList();

                    var splitResult = await _apiService.AddInventoryDocumentLineAsync(splitForm.DocumentId, splitForm, token);
                    if (!splitResult.IsSuccess)
                    {
                        var splitFailureModel = await BuildIssueDocumentDetailsModalModelAsync(form.DocumentId, token, cancellationToken);
                        splitFailureModel.ErrorMessage = splitResult.ErrorMessage ?? "ذخیره آیتم سند انجام نشد.";
                        splitFailureModel.LineForm = form;
                        return PartialView("~/Views/InventoryManagement/_IssueDocumentDetailsModalBody.cshtml", splitFailureModel);
                    }
                }

                var refreshedSplitModel = await BuildIssueDocumentDetailsModalModelAsync(form.DocumentId, token, cancellationToken);
                return PartialView("~/Views/InventoryManagement/_IssueDocumentDetailsModalBody.cshtml", refreshedSplitModel);
            }

            var resolvedSerialGroup = selectedGroups[0];
            if (!string.IsNullOrWhiteSpace(form.SourceLocationRef) && !string.Equals(NormalizeLookupKey(form.SourceLocationRef), resolvedSerialGroup.Key.LocationRef, StringComparison.OrdinalIgnoreCase))
            {
                var locationMismatchModel = await BuildIssueDocumentDetailsModalModelAsync(form.DocumentId, token);
                locationMismatchModel.ErrorMessage = "انتخاب لوکیشن با سریال‌های انتخاب شده هم‌خوانی ندارد.";
                locationMismatchModel.LineForm = form;
                return PartialView("~/Views/InventoryManagement/_IssueDocumentDetailsModalBody.cshtml", locationMismatchModel);
            }

            var resolvedLotBatchNo = NormalizeLotBatchNo(form.LotBatchNo) ?? NormalizeLotBatchNo(resolvedSerialGroup.Key.LotBatchNo);
            var resolvedQualityStatusRef = string.IsNullOrWhiteSpace(form.QualityStatusRef)
                ? resolvedSerialGroup.Key.QualityStatusRef
                : form.QualityStatusRef;
            if (!string.IsNullOrWhiteSpace(form.LotBatchNo) && !string.Equals(NormalizeLookupKey(form.LotBatchNo), resolvedSerialGroup.Key.LotBatchNo, StringComparison.OrdinalIgnoreCase))
            {
                var lotMismatchModel = await BuildIssueDocumentDetailsModalModelAsync(form.DocumentId, token);
                lotMismatchModel.ErrorMessage = "انتخاب لات با سریال‌های انتخاب شده هم‌خوانی ندارد.";
                lotMismatchModel.LineForm = form;
                return PartialView("~/Views/InventoryManagement/_IssueDocumentDetailsModalBody.cshtml", lotMismatchModel);
            }

            if (!string.IsNullOrWhiteSpace(form.QualityStatusRef) && !string.Equals(NormalizeLookupKey(form.QualityStatusRef), resolvedSerialGroup.Key.QualityStatusRef, StringComparison.OrdinalIgnoreCase))
            {
                var qualityMismatchModel = await BuildIssueDocumentDetailsModalModelAsync(form.DocumentId, token);
                qualityMismatchModel.ErrorMessage = "انتخاب کیفیت با سریال‌های انتخاب شده هم‌خوانی ندارد.";
                qualityMismatchModel.LineForm = form;
                return PartialView("~/Views/InventoryManagement/_IssueDocumentDetailsModalBody.cshtml", qualityMismatchModel);
            }

            form.SourceLocationRef = string.IsNullOrWhiteSpace(resolvedSerialGroup.Key.LocationRef) ? null : resolvedSerialGroup.Key.LocationRef;
            form.LotBatchNo = resolvedLotBatchNo;
            form.QualityStatusRef = string.IsNullOrWhiteSpace(resolvedQualityStatusRef) ? null : resolvedQualityStatusRef;
            form.Qty = resolvedSerialSelections.Count;
            form.Serials = resolvedSerialGroup
                .Select(item => new InventoryDocumentLineSerialModel
                {
                    SerialItemBusinessKey = item.AvailableSerial.SerialItemBusinessKey,
                    SerialNo = item.AvailableSerial.SerialNo
                })
                .ToList();
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
            "ReturnFromSell",
            status,
            variantId,
            warehouseId,
            locationId,
            sellerId,
            occurredFrom,
            occurredTo,
            page,
            pageSize,
            cancellationToken,
            returnReferenceDocumentType: "Issue");

    [HttpGet("/InventoryManagement/Documents/Return/Purchase")]
    public Task<IActionResult> ReturnPurchaseDocuments(
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
            "ReturnPurchaseDocuments",
            documentId,
            editingLineId,
            null,
            documentNo,
            "ReturnFromBuy",
            status,
            variantId,
            warehouseId,
            locationId,
            sellerId,
            occurredFrom,
            occurredTo,
            page,
            pageSize,
            cancellationToken,
            returnReferenceDocumentType: "Receipt");

    [HttpGet("/InventoryManagement/Documents/Return/Transfer")]
    public Task<IActionResult> ReturnTransferDocuments(
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
            "ReturnTransferDocuments",
            documentId,
            editingLineId,
            null,
            documentNo,
            "ReturnFromTransfer",
            status,
            variantId,
            warehouseId,
            locationId,
            sellerId,
            occurredFrom,
            occurredTo,
            page,
            pageSize,
            cancellationToken,
            returnReferenceDocumentType: "Transfer");

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

        var documentResult = await _apiService.GetInventoryDocumentByBusinessKeyAsync(form.DocumentId, token);
        if (!documentResult.IsSuccess || documentResult.Data is null)
        {
            var invalidModel = await BuildReturnDocumentDetailsModalModelAsync(form.DocumentId, token, form.LineId, cancellationToken);
            invalidModel.ErrorMessage = documentResult.ErrorMessage ?? "سند برگشت یافت نشد.";
            invalidModel.LineForm = form;
            return PartialView("~/Views/InventoryManagement/_ReturnDocumentDetailsModalBody.cshtml", invalidModel);
        }

        var document = documentResult.Data;
        if (!IsReturnDocumentType(document.DocumentType))
        {
            var invalidModel = await BuildReturnDocumentDetailsModalModelAsync(form.DocumentId, token, form.LineId, cancellationToken);
            invalidModel.ErrorMessage = "این سند از نوع برگشتی نیست.";
            invalidModel.LineForm = form;
            return PartialView("~/Views/InventoryManagement/_ReturnDocumentDetailsModalBody.cshtml", invalidModel);
        }

        var sourceResolutionResult = await TryResolveReturnSourceSelectionAsync(token, document, form, cancellationToken);
        if (!sourceResolutionResult.Success)
        {
            var invalidModel = await BuildReturnDocumentDetailsModalModelAsync(form.DocumentId, token, form.LineId, cancellationToken);
            invalidModel.ErrorMessage = sourceResolutionResult.ErrorMessage ?? "امکان تعیین منبع برگشت وجود نداشت.";
            invalidModel.LineForm = form;
            return PartialView("~/Views/InventoryManagement/_ReturnDocumentDetailsModalBody.cshtml", invalidModel);
        }

        var locationValidationError = ValidateReturnLineLocations(document.DocumentType, form);
        if (!string.IsNullOrWhiteSpace(locationValidationError))
        {
            var invalidModel = await BuildReturnDocumentDetailsModalModelAsync(form.DocumentId, token, form.LineId, cancellationToken);
            invalidModel.ErrorMessage = locationValidationError;
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

    [HttpPost("/InventoryManagement/Documents/Conversion/Preview")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> PreviewConversionDocument(
        [Bind(Prefix = "CreateForm")] CreateInventoryDocumentForm form,
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

        if (!IsAuthorizedFor(token, "Inventory.Document.Create", "InventoryDocument.Create", "Document.Create"))
        {
            return Json(new
            {
                isSuccess = false,
                errorMessage = "شما دسترسی ایجاد سند را ندارید."
            });
        }

        var previewResult = await BuildConversionPreviewAsync(token, documentId, form, cancellationToken);
        if (!previewResult.Success || previewResult.Model is null)
        {
            return Json(new
            {
                isSuccess = false,
                errorMessage = previewResult.Error ?? "امکان آماده‌سازی پیش‌نمایش سند تبدیل وجود نداشت."
            });
        }

        return PartialView("~/Views/InventoryManagement/_ConversionDocumentPreviewBody.cshtml", previewResult.Model);
    }

    [HttpPost("/InventoryManagement/Documents/Conversion/CommitPreview")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CommitConversionPreview(
        string documentId,
        string previewSelectionsJson,
        string variantId,
        string operationType,
        decimal quantity,
        string? reasonCode,
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

        if (!IsAuthorizedFor(token, "Inventory.Document.Create", "InventoryDocument.Create", "Document.Create"))
        {
            return Json(new
            {
                isSuccess = false,
                errorMessage = "شما دسترسی ویرایش آیتم سند را ندارید."
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

        if (!string.Equals(documentResult.Data.DocumentType, "Conversion", StringComparison.OrdinalIgnoreCase))
        {
            return Json(new
            {
                isSuccess = false,
                errorMessage = "این سند تبدیل نیست."
            });
        }

        var previewResult = await BuildConversionPreviewAsync(
            token,
            documentId,
            new CreateInventoryDocumentForm
            {
                ConversionVariantId = variantId,
                ConversionOperationType = operationType,
                ConversionQuantity = quantity,
                ReasonCode = reasonCode
            },
            cancellationToken);

        if (!previewResult.Success || previewResult.Model is null)
        {
            return Json(new
            {
                isSuccess = false,
                errorMessage = previewResult.Error ?? "امکان تایید پیش‌نمایش سند تبدیل وجود نداشت."
            });
        }

        var selections = ParseConversionPreviewSelections(previewSelectionsJson);
        if (selections is null || selections.Count == 0)
        {
            return Json(new
            {
                isSuccess = false,
                errorMessage = "انتخاب لوکیشن‌های پیش‌نمایش معتبر نیست."
            });
        }

        var linesByKey = previewResult.Model.Lines
            .ToDictionary(x => x.LineKey, x => x, StringComparer.OrdinalIgnoreCase);
        var selectedGroup = previewResult.Model.SelectedSellerQualityKey;
        var selectedLocationLookup = selections
            .Where(x => !string.IsNullOrWhiteSpace(x.LineKey) && !string.IsNullOrWhiteSpace(x.LocationRef))
            .ToDictionary(x => x.LineKey, x => x.LocationRef, StringComparer.OrdinalIgnoreCase);

        if (selectedLocationLookup.Count == 0 || selectedLocationLookup.Any(pair => !linesByKey.ContainsKey(pair.Key)))
        {
            return Json(new
            {
                isSuccess = false,
                errorMessage = "انتخاب‌های پیش‌نمایش معتبر نیستند."
            });
        }

        var locationLookupResult = await _apiService.GetLocationLookupAsync(token, warehouseId: null, includeInactive: true);
        var locationLookup = locationLookupResult.Data ?? new List<LocationLookupItemModel>();
        var selectedLines = new List<InventoryDocumentLineForm>();

        foreach (var line in previewResult.Model.Lines)
        {
            if (!selectedLocationLookup.TryGetValue(line.LineKey, out var selectedLocationRef) || string.IsNullOrWhiteSpace(selectedLocationRef))
            {
                return Json(new
                {
                    isSuccess = false,
                    errorMessage = $"برای ردیف {line.LineKey} لوکیشن انتخاب نشده است."
                });
            }

            var lineLocation = line.LocationOptions.FirstOrDefault(x => string.Equals(x.LocationRef, selectedLocationRef, StringComparison.OrdinalIgnoreCase));
            if (lineLocation is null)
            {
                return Json(new
                {
                    isSuccess = false,
                    errorMessage = $"لوکیشن انتخاب‌شده برای ردیف {line.LineKey} معتبر نیست."
                });
            }

            if (string.IsNullOrWhiteSpace(line.BaseUomRef))
            {
                return Json(new
                {
                    isSuccess = false,
                    errorMessage = $"واحد پایه برای ردیف {line.LineKey} مشخص نشده است."
                });
            }

            var location = locationLookup.FirstOrDefault(x => string.Equals(x.LocationBusinessKey, selectedLocationRef, StringComparison.OrdinalIgnoreCase));
            if (location is null)
            {
                return Json(new
                {
                    isSuccess = false,
                    errorMessage = $"لوکیشن انتخاب‌شده برای ردیف {line.LineKey} در سیستم پیدا نشد."
                });
            }

            var targetQuality = TryParseSellerQualityKey(selectedGroup)?.QualityStatusRef;
            var baseUomRef = string.IsNullOrWhiteSpace(line.BaseUomRef) ? string.Empty : line.BaseUomRef;
            selectedLines.Add(new InventoryDocumentLineForm
            {
                VariantId = line.VariantId,
                Qty = line.RequiredQty,
                UomRef = baseUomRef,
                BaseUomRef = baseUomRef,
                SourceLocationRef = string.Equals(line.Role, "Source", StringComparison.OrdinalIgnoreCase) ? selectedLocationRef : null,
                DestinationLocationRef = string.Equals(line.Role, "Destination", StringComparison.OrdinalIgnoreCase) ? selectedLocationRef : null,
                QualityStatusRef = targetQuality?.ToString("D")
            });
        }

        var documentHeader = new CreateInventoryDocumentForm
        {
            DocumentId = documentId,
            DocumentType = "Conversion",
            DocumentNo = documentResult.Data.DocumentNo,
            ExternalReferenceNo = documentResult.Data.ExternalReferenceNo,
            ReferenceType = documentResult.Data.ReferenceType,
            ReferenceBusinessId = documentResult.Data.ReferenceBusinessId,
            WarehouseRef = locationLookup.FirstOrDefault(x => string.Equals(x.LocationBusinessKey, selectedLocationLookup.Values.First(), StringComparison.OrdinalIgnoreCase))?.WarehouseRef ?? string.Empty,
            SellerRef = TryParseSellerQualityKey(selectedGroup)?.SellerRef.ToString("D") ?? string.Empty,
            OccurredAt = documentResult.Data.OccurredAt,
            ReasonCode = documentResult.Data.ReasonCode,
            Lines = selectedLines.Select(line => new CreateInventoryDocumentLineForm
            {
                VariantId = line.VariantId,
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
            }).ToList()
        };

        var updateResult = await _apiService.UpdateInventoryDocumentAsync(documentHeader, token);
        if (!updateResult.IsSuccess)
        {
            return Json(new
            {
                isSuccess = false,
                errorMessage = updateResult.ErrorMessage ?? "تایید پیش‌نمایش سند تبدیل انجام نشد."
            });
        }

        foreach (var line in selectedLines)
        {
            var addResult = await _apiService.AddInventoryDocumentLineAsync(documentId, line, token);
            if (!addResult.IsSuccess)
            {
                return Json(new
                {
                    isSuccess = false,
                    errorMessage = addResult.ErrorMessage ?? "ذخیره ردیف‌های سند تبدیل انجام نشد."
                });
            }
        }

        return Json(new
        {
            isSuccess = true,
            redirectUrl = Url.Action(nameof(ConversionDocuments), new
            {
                documentId,
                documentType = "Conversion",
                tab = "list"
            })
        });
    }

    [HttpGet("/InventoryManagement/Documents/ReferenceSearch")]
    public async Task<IActionResult> SearchDocumentReferenceDocuments(
        string? term,
        string? referenceDocumentType,
        CancellationToken cancellationToken = default)
    {
        return await SearchDocumentReferenceDocumentsInternalAsync(term, referenceDocumentType, cancellationToken);
    }

    [HttpGet("/InventoryManagement/Documents/Return/ReferenceSearch")]
    public async Task<IActionResult> SearchReturnReferenceDocuments(
        string? term,
        string? referenceDocumentType,
        CancellationToken cancellationToken = default)
    {
        return await SearchDocumentReferenceDocumentsInternalAsync(term, referenceDocumentType, cancellationToken);
    }

    private async Task<IActionResult> SearchDocumentReferenceDocumentsInternalAsync(
        string? term,
        string? referenceDocumentType,
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
                errorMessage = "شما دسترسی مشاهده اسناد مبنا را ندارید."
            });
        }

        var resolvedReferenceDocumentType = string.IsNullOrWhiteSpace(referenceDocumentType)
            ? null
            : ResolveReturnReferenceDocumentType(referenceDocumentType);

        var searchResult = await _apiService.SearchInventoryDocumentsAsync(
            token,
            documentNo: string.IsNullOrWhiteSpace(term) ? null : term.Trim(),
            documentType: resolvedReferenceDocumentType,
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

        var documents = (searchResult.Data?.Items ?? new List<InventoryDocumentListItemModel>())
            .Select(document => new
            {
                documentBusinessKey = document.DocumentBusinessKey,
                documentType = document.DocumentType,
                documentNo = document.DocumentNo,
                status = document.Status,
                warehouseRef = document.WarehouseRef,
                sellerRef = document.SellerRef,
                occurredAt = document.OccurredAt,
                referenceType = document.ReferenceType,
                referenceBusinessId = document.ReferenceBusinessId
            })
            .ToList();

        return Json(new
        {
            isSuccess = true,
            items = documents,
            documents = documents
        });
    }

    [HttpGet("/InventoryManagement/Documents/Return/ReferenceDocument")]
    public async Task<IActionResult> GetReturnReferenceDocument(
        string documentId,
        string? returnDocumentType = null,
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

        var resolvedReturnDocumentType = ResolveReturnSelectionDocumentType(returnDocumentType, documentResult.Data.DocumentType);
        var allowedVariantIds = new List<string>();
        var serialsByVariant = new List<object>();
        var referenceLinesByVariant = new List<object>();

        foreach (var variantGroup in documentResult.Data.Lines
                     .Where(line => !string.IsNullOrWhiteSpace(line.VariantRef))
                     .GroupBy(line => line.VariantRef.Trim(), StringComparer.OrdinalIgnoreCase))
        {
            var variantId = variantGroup.Key;
            if (!allowedVariantIds.Any(x => string.Equals(x, variantId, StringComparison.OrdinalIgnoreCase)))
            {
                allowedVariantIds.Add(variantId);
            }

            var referenceLines = variantGroup
                .Select((line, lineIndex) =>
                {
                    var sourceLocationRef = ResolveReturnReferenceSourceLocationRef(resolvedReturnDocumentType, line);
                    var destinationLocationRef = ResolveReturnReferenceDestinationLocationRef(resolvedReturnDocumentType, line);
                    var qualityStatusRef = ResolveReturnReferenceQualityStatusRef(line);
                    var normalizedLotBatchNo = NormalizeLotBatchNo(line.LotBatchNo);

                    var lineSerials = line.Serials
                        .Select((serial, serialIndex) => new
                        {
                            serialItemBusinessKey = serial.SerialItemBusinessKey,
                            serialRef = serial.SerialRef,
                            serialNo = serial.SerialNo,
                            sourceLocationRef,
                            destinationLocationRef,
                            locationRef = sourceLocationRef ?? destinationLocationRef,
                            qualityStatusRef,
                            lotBatchNo = normalizedLotBatchNo,
                            status = "Available",
                            sequence = (lineIndex * 1000) + serialIndex
                        })
                        .ToList();

                    return new
                    {
                        lineBusinessKey = line.LineBusinessKey,
                        sourceLocationRef,
                        destinationLocationRef,
                        qualityStatusRef,
                        lotBatchNo = normalizedLotBatchNo,
                        serials = lineSerials
                    };
                })
                .ToList();

            referenceLinesByVariant.Add(new
            {
                variantId,
                lines = referenceLines
            });

            serialsByVariant.Add(new
            {
                variantId,
                serials = referenceLines
                    .SelectMany(line => line.serials)
                    .OrderBy(serial => serial.sequence)
                    .Select(serial => new
                    {
                        serial.serialItemBusinessKey,
                        serial.serialRef,
                        serial.serialNo,
                        serial.sourceLocationRef,
                        serial.destinationLocationRef,
                        serial.locationRef,
                        serial.qualityStatusRef,
                        serial.lotBatchNo,
                        serial.status
                    })
                    .ToList()
            });
        }

        return Json(new
        {
            isSuccess = true,
            document = documentResult.Data,
            allowedVariantIds,
            serialsByVariant,
            referenceLinesByVariant
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
        CancellationToken cancellationToken = default,
        string? returnReferenceDocumentType = null)
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
        if (string.IsNullOrWhiteSpace(createSellerId))
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
        ViewBag.ReturnReferenceDocumentType = returnReferenceDocumentType;
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
        var isReturnDocument = IsReturnDocumentType(form.DocumentType);
        var returnReferenceDocumentType = ResolveReturnReferenceDocumentType(form.ReferenceType, form.DocumentType);
        if (isReturnDocument && string.IsNullOrWhiteSpace(form.ReferenceType))
        {
            form.ReferenceType = returnReferenceDocumentType;
        }
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

        var routeActionName = ResolveDocumentRouteActionName(form.DocumentType, form.ReferenceType);

        if (!TryValidateModel(form))
        {
            TempData["CatalogError"] = ExtractModelError(ModelState);
            return RedirectToAction(routeActionName);
        }

        if (!isUpdate && (string.IsNullOrWhiteSpace(form.SellerRef) || !Guid.TryParse(form.SellerRef, out var parsedSellerRef) || parsedSellerRef == Guid.Empty))
        {
            var ownerSellerResult = await ResolveOwnerSellerAsync(token);
            if (!ownerSellerResult.IsSuccess || ownerSellerResult.Data is null)
            {
                TempData["CatalogError"] = ownerSellerResult.ErrorMessage ?? "Seller Owner فعال در سیستم پیدا نشد.";
                return RedirectToAction(routeActionName);
            }

            form.SellerRef = ownerSellerResult.Data.SellerBusinessKey.ToString("D");
        }

        if (string.Equals(form.DocumentType, "Receipt", StringComparison.OrdinalIgnoreCase)
            || string.Equals(form.DocumentType, "Issue", StringComparison.OrdinalIgnoreCase))
        {
            if (string.IsNullOrWhiteSpace(form.ReceivedBy) || string.IsNullOrWhiteSpace(form.DeliveredBy))
            {
                TempData["CatalogError"] = "برای رسید و حواله، انتخاب تحویل‌دهنده و تحویل‌گیرنده الزامی است.";
                return RedirectToAction(routeActionName);
            }
        }

        if (isReturnDocument
            && string.IsNullOrWhiteSpace(form.ReferenceBusinessId))
        {
            TempData["CatalogError"] = returnReferenceDocumentType switch
            {
                "Receipt" => "برای سند مرجوعی از خرید، انتخاب سند رسید مبنا الزامی است.",
                "Transfer" => "برای سند مرجوعی از انتقال، انتخاب سند انتقال مبنا الزامی است.",
                "Issue" => "برای سند مرجوعی از فروش، انتخاب سند خروج مبنا الزامی است.",
                _ => "برای سند مرجوعی، انتخاب سند حواله مبنا الزامی است."
            };
            return RedirectToAction(routeActionName);
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
                        routeActionName,
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
                        return RedirectToAction(routeActionName);
                    }

                    line.UomRef = variant.BaseUomRef;
                    line.BaseUomRef = variant.BaseUomRef;
                }

                if (isReturnDocument)
                {
                    var draftReturnDocument = new InventoryDocumentDetailsModel
                    {
                        DocumentType = form.DocumentType,
                        ReferenceBusinessId = form.ReferenceBusinessId
                    };

                    foreach (var line in form.Lines)
                    {
                        var lineResolutionForm = new InventoryDocumentLineForm
                        {
                            DocumentId = form.DocumentId ?? string.Empty,
                            DocumentType = form.DocumentType,
                            VariantId = line.VariantId,
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
                            Serials = line.Serials
                                .Select(serial => new InventoryDocumentLineSerialModel
                                {
                                    SerialItemBusinessKey = serial.SerialItemBusinessKey,
                                    SerialRef = serial.SerialRef,
                                    SerialNo = serial.SerialNo
                                })
                                .ToList()
                        };

                        var lineResolutionResult = await TryResolveReturnSourceSelectionAsync(token, draftReturnDocument, lineResolutionForm);
                        if (!lineResolutionResult.Success)
                        {
                            TempData["CatalogError"] = lineResolutionResult.ErrorMessage ?? "امکان تعیین منبع ردیف برگشت وجود نداشت.";
                            return RedirectToAction(routeActionName);
                        }

                        var returnLocationValidationError = ValidateReturnLineLocations(form.DocumentType, lineResolutionForm);
                        if (!string.IsNullOrWhiteSpace(returnLocationValidationError))
                        {
                            TempData["CatalogError"] = returnLocationValidationError;
                            return RedirectToAction(routeActionName);
                        }

                        line.SourceLocationRef = lineResolutionForm.SourceLocationRef;
                        line.DestinationLocationRef = lineResolutionForm.DestinationLocationRef;
                        line.QualityStatusRef = lineResolutionForm.QualityStatusRef;
                        line.FromQualityStatusRef = lineResolutionForm.FromQualityStatusRef;
                        line.ToQualityStatusRef = lineResolutionForm.ToQualityStatusRef;
                        line.LotBatchNo = lineResolutionForm.LotBatchNo;
                        line.Serials = lineResolutionForm.Serials
                            .Select(serial => new InventoryDocumentLineSerialModel
                            {
                                SerialItemBusinessKey = serial.SerialItemBusinessKey,
                                SerialRef = serial.SerialRef,
                                SerialNo = serial.SerialNo
                            })
                            .ToList();
                    }
                }
            }

            var result = await _apiService.CreateInventoryDocumentAsync(form, token);
            TempData[result.IsSuccess ? "CatalogSuccess" : "CatalogError"] =
                result.IsSuccess ? "سند موجودی با موفقیت ایجاد شد." : result.ErrorMessage ?? "ایجاد سند انجام نشد.";

            return RedirectToAction(routeActionName);
        }

        var updateResult = await _apiService.UpdateInventoryDocumentAsync(form, token);
        TempData[updateResult.IsSuccess ? "CatalogSuccess" : "CatalogError"] =
            updateResult.IsSuccess ? "اطلاعات عمومی سند با موفقیت به‌روزرسانی شد." : updateResult.ErrorMessage ?? "به‌روزرسانی سند انجام نشد.";

        return RedirectToAction(routeActionName);
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

        var result = await ExecuteChangeDocumentStatusAsync(documentId, "post", null, serialSelectionsJson, token);
        TempData[result.IsSuccess ? "CatalogSuccess" : "CatalogError"] =
            result.IsSuccess ? "سند پست شد." : result.ErrorMessage ?? "پست سند انجام نشد.";
        return await RedirectToDocumentPageAsync(documentId, token);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangeDocumentStatus(InventoryDocumentStatusChangeForm form)
    {
        if (!TryGetToken(out var token))
        {
            return Json(new
            {
                isSuccess = false,
                errorMessage = "نشست کاربری منقضی شده است. لطفاً دوباره وارد شوید."
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

        var normalizedAction = (form.Action ?? string.Empty).Trim().ToLowerInvariant();
        if (normalizedAction is not ("approve" or "reject" or "cancel" or "post"))
        {
            return Json(new
            {
                isSuccess = false,
                errorMessage = "نوع عملیات نامعتبر است."
            });
        }

        var isAuthorized = normalizedAction switch
        {
            "approve" => IsAuthorizedFor(token, "Inventory.Document.Approve", "InventoryDocument.Approve", "Document.Approve"),
            "reject" => IsAuthorizedFor(token, "Inventory.Document.Reject", "InventoryDocument.Reject", "Document.Reject"),
            "cancel" => IsAuthorizedFor(token, "Inventory.Document.Cancel", "InventoryDocument.Cancel", "Document.Cancel"),
            "post" => IsAuthorizedFor(token, "Inventory.Document.Post", "InventoryDocument.Post", "Document.Post"),
            _ => false
        };

        if (!isAuthorized)
        {
            return Json(new
            {
                isSuccess = false,
                errorMessage = "شما دسترسی انجام این عملیات را ندارید."
            });
        }

        var result = await ExecuteChangeDocumentStatusAsync(form.DocumentId, form.Action ?? string.Empty, form.ReasonCode, null, token);
        return Json(new
        {
            isSuccess = result.IsSuccess,
            errorMessage = result.IsSuccess ? null : result.ErrorMessage ?? "تغییر وضعیت سند انجام نشد."
        });
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
                    break;

                case "Transfer":
                    if (string.IsNullOrWhiteSpace(line.DestinationLocationRef))
                    {
                        return "برای سند انتقال، لوکیشن مقصد الزامی است.";
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
            ExternalReferenceNo = null,
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
        var action = ResolveDocumentRouteActionName(documentResult.Data?.DocumentType, documentResult.Data?.ReferenceType);
        return RedirectToAction(action, new { documentId, documentType = documentResult.Data?.DocumentType });
    }

    private static string ResolveDocumentRouteActionName(string? documentType)
        => ResolveDocumentRouteActionName(documentType, null);

    private static string ResolveDocumentRouteActionName(string? documentType, string? referenceType)
    {
        return documentType switch
        {
            "Receipt" => "ReceiptDocuments",
            "Issue" => "IssueDocuments",
            "Transfer" => "TransferDocuments",
            "Adjustment" => "AdjustmentDocuments",
            "ReturnFromSell" => "ReturnDocuments",
            "ReturnFromBuy" => "ReturnPurchaseDocuments",
            "ReturnFromTransfer" => "ReturnTransferDocuments",
            "Return" => string.Equals(referenceType, "Receipt", StringComparison.OrdinalIgnoreCase)
                ? "ReturnPurchaseDocuments"
                : "ReturnDocuments",
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
            "ReturnPurchaseDocuments" => "~/Views/InventoryManagement/ReturnDocuments.cshtml",
            "ReturnTransferDocuments" => "~/Views/InventoryManagement/ReturnDocuments.cshtml",
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
            "Receipt" or "Issue" or "Transfer" or "Adjustment" or "ReturnFromSell" or "ReturnFromBuy" or "ReturnFromTransfer" or "QualityChange" or "Conversion" => documentType,
            "Return" => "ReturnFromSell",
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
            "ReturnFromSell" => "return_documents",
            "ReturnFromBuy" => "return_purchase_documents",
            "ReturnFromTransfer" => "return_transfer_documents",
            "Return" => "return_documents",
            "QualityChange" => "quality_change_documents",
            "Conversion" => "conversion_documents",
            _ => "receipt_documents"
        };
    }

    private static bool IsReturnDocumentType(string? documentType)
        => string.Equals(documentType, "ReturnFromSell", StringComparison.OrdinalIgnoreCase)
            || string.Equals(documentType, "ReturnFromBuy", StringComparison.OrdinalIgnoreCase)
            || string.Equals(documentType, "ReturnFromTransfer", StringComparison.OrdinalIgnoreCase)
            || string.Equals(documentType, "Return", StringComparison.OrdinalIgnoreCase);

    private static string ResolveReturnReferenceDocumentType(string? referenceType, string? documentType = null)
    {
        if (string.Equals(referenceType, "Receipt", StringComparison.OrdinalIgnoreCase))
        {
            return "Receipt";
        }

        if (string.Equals(referenceType, "Transfer", StringComparison.OrdinalIgnoreCase))
        {
            return "Transfer";
        }

        if (string.Equals(referenceType, "Issue", StringComparison.OrdinalIgnoreCase))
        {
            return "Issue";
        }

        return string.Equals(documentType, "ReturnFromBuy", StringComparison.OrdinalIgnoreCase)
            ? "Receipt"
            : string.Equals(documentType, "ReturnFromTransfer", StringComparison.OrdinalIgnoreCase)
                ? "Transfer"
                : "Issue";
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
        var isAdjustmentDocument = string.Equals(documentType, "Adjustment", StringComparison.OrdinalIgnoreCase);
        var isConversionDocument = string.Equals(documentType, "Conversion", StringComparison.OrdinalIgnoreCase);

        return new CreateInventoryDocumentForm
        {
            DocumentId = isEditMode ? existingDocument?.DocumentBusinessKey : null,
            DocumentType = documentType,
            DocumentNo = isEditMode ? existingDocument?.DocumentNo : null,
            ExternalReferenceNo = isEditMode ? existingDocument?.ExternalReferenceNo : null,
            ReferenceType = isEditMode ? existingDocument?.ReferenceType : null,
            ReferenceBusinessId = isEditMode ? existingDocument?.ReferenceBusinessId?.ToString() : null,
            WarehouseRef = isConversionDocument
                ? isEditMode ? existingDocument?.WarehouseRef ?? string.Empty : string.Empty
                : isAdjustmentDocument
                    ? isEditMode ? existingDocument?.WarehouseRef ?? string.Empty : warehouseId ?? string.Empty
                : isReceiptDocument
                    ? string.Empty
                : isEditMode ? existingDocument?.WarehouseRef ?? string.Empty : warehouseId ?? string.Empty,
            SellerRef = isConversionDocument
                ? isEditMode ? existingDocument?.SellerRef ?? string.Empty : string.Empty
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
                UseUniqueSerialItems = false,
                LotBatchNo = string.Equals(documentType, "Receipt", StringComparison.OrdinalIgnoreCase)
                    ? ResolveReceiptDocumentLotBatchNo(document)
                    : null,
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
            LotBatchNo = string.Equals(documentType, "Receipt", StringComparison.OrdinalIgnoreCase)
                ? NormalizeLotBatchNo(line.LotBatchNo) ?? ResolveReceiptDocumentLotBatchNo(document)
                : line.LotBatchNo,
            ReasonCode = line.ReasonCode,
            AdjustmentDirection = line.AdjustmentDirection,
            UseUniqueSerialItems = string.Equals(documentType, "Receipt", StringComparison.OrdinalIgnoreCase) && line.Serials.Count > 0,
            Serials = line.Serials.Select(serial => new InventoryDocumentLineSerialModel
            {
                SerialItemBusinessKey = serial.SerialItemBusinessKey,
                SerialRef = serial.SerialRef,
                SerialNo = serial.SerialNo
            }).ToList()
        };
    }

    private static string? ResolveReceiptDocumentLotBatchNo(InventoryDocumentDetailsModel? document)
    {
        if (document is null)
        {
            return null;
        }

        var lots = document.Lines
            .Select(x => NormalizeLotBatchNo(x.LotBatchNo))
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Select(x => x!)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        return lots.Count == 1 ? lots[0] : null;
    }

    private static string? PrepareReceiptLineSerials(InventoryDocumentDetailsModel document, InventoryDocumentLineForm form)
    {
        if (!string.Equals(form.DocumentType, "Receipt", StringComparison.OrdinalIgnoreCase))
        {
            form.Serials.Clear();
            return null;
        }

        if (!form.UseUniqueSerialItems)
        {
            form.Serials.Clear();
            return null;
        }

        if (form.Qty <= 0 || form.Qty != decimal.Truncate(form.Qty))
        {
            return "برای صدور شناسه یکتا، تعداد ردیف باید عدد صحیح و بزرگ‌تر از صفر باشد.";
        }

        var lineNo = ResolveReceiptLineNumber(document, form.LineId);
        form.Serials = BuildReceiptLineSerials(document.DocumentNo, lineNo, (int)form.Qty);
        return null;
    }

    private static int ResolveReceiptLineNumber(InventoryDocumentDetailsModel document, string? lineId)
    {
        if (!string.IsNullOrWhiteSpace(lineId))
        {
            var existingIndex = document.Lines.FindIndex(x => string.Equals(x.LineBusinessKey, lineId, StringComparison.OrdinalIgnoreCase));
            if (existingIndex >= 0)
            {
                return existingIndex + 1;
            }
        }

        return document.Lines.Count + 1;
    }

    private static List<InventoryDocumentLineSerialModel> BuildReceiptLineSerials(string documentNo, int lineNo, int qty)
    {
        var serials = new List<InventoryDocumentLineSerialModel>(Math.Max(qty, 0));
        var documentPart = NormalizeSerialSegment(documentNo);

        for (var i = 0; i < qty; i++)
        {
            serials.Add(new InventoryDocumentLineSerialModel
            {
                SerialItemBusinessKey = string.Empty,
                SerialRef = null,
                SerialNo = $"SN-{documentPart}-L{lineNo:D2}-{(i + 1):D3}"
            });
        }

        return serials;
    }

    private static string NormalizeSerialSegment(string value)
    {
        var normalized = (value ?? string.Empty).Trim();
        if (string.IsNullOrWhiteSpace(normalized))
        {
            return "DOC";
        }

        normalized = normalized.Replace(' ', '-');
        normalized = normalized.Replace('/', '-');
        normalized = normalized.Replace('\\', '-');
        normalized = normalized.Replace(':', '-');
        normalized = normalized.Replace('.', '-');
        return normalized;
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

        var lineForm = BuildLineForm(document, documentId, editingLineId, document?.DocumentType.ToString() ?? "ReturnFromSell");
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

    private async Task<List<PostDocumentLineSerialSelectionModel>?> BuildDefaultPostDocumentSerialSelectionsAsync(
        string documentId,
        string token,
        CancellationToken cancellationToken = default)
    {
        var documentResult = await _apiService.GetInventoryDocumentByBusinessKeyAsync(documentId, token);
        if (!documentResult.IsSuccess || documentResult.Data is null || documentResult.Data.Lines.Count == 0)
        {
            return null;
        }

        var selections = new List<PostDocumentLineSerialSelectionModel>();
        foreach (var line in documentResult.Data.Lines)
        {
            var serials = line.Serials
                .Where(serial => !string.IsNullOrWhiteSpace(serial.SerialItemBusinessKey) || !string.IsNullOrWhiteSpace(serial.SerialNo))
                .Select(serial => new PostDocumentSerialItemModel
                {
                    SerialItemBusinessKey = serial.SerialItemBusinessKey?.Trim() ?? string.Empty,
                    SerialNo = serial.SerialNo?.Trim() ?? string.Empty
                })
                .ToList();

            selections.Add(new PostDocumentLineSerialSelectionModel
            {
                DocumentLineBusinessKey = line.LineBusinessKey?.Trim() ?? string.Empty,
                UseUniqueSerialItems = documentResult.Data.DocumentType.Equals("Receipt", StringComparison.OrdinalIgnoreCase) && serials.Count == 0,
                Serials = serials
            });
        }

        return selections.Where(selection => !string.IsNullOrWhiteSpace(selection.DocumentLineBusinessKey)).ToList();
    }

    private async Task<(bool IsSuccess, string? ErrorMessage)> ExecuteChangeDocumentStatusAsync(
        string documentId,
        string action,
        string? reasonCode,
        string? serialSelectionsJson,
        string token)
    {
        var actor = HttpContext.Session.GetString("UserName") ?? "dashboard";
        var serialSelections = ParsePostDocumentSerialSelections(serialSelectionsJson);
        if (string.Equals(action, "post", StringComparison.OrdinalIgnoreCase) && serialSelections is null)
        {
            serialSelections = await BuildDefaultPostDocumentSerialSelectionsAsync(documentId, token);
        }

        var result = await _apiService.ChangeInventoryDocumentStatusAsync(documentId, action, reasonCode, actor, serialSelections, token);
        return (result.IsSuccess, result.ErrorMessage);
    }

    private static string BuildVariantLookupLabel(ProductVariantSummaryModel variant)
    {
        var name = (variant.Name ?? string.Empty).Trim();
        var sku = (variant.Sku ?? string.Empty).Trim();

        if (!string.IsNullOrWhiteSpace(name) && !string.IsNullOrWhiteSpace(sku) && !string.Equals(name, sku, StringComparison.OrdinalIgnoreCase))
        {
            return $"{name} ({sku})";
        }

        if (!string.IsNullOrWhiteSpace(name))
        {
            return name;
        }

        if (!string.IsNullOrWhiteSpace(sku))
        {
            return sku;
        }

        if (!string.IsNullOrWhiteSpace(variant.Barcode))
        {
            return variant.Barcode.Trim();
        }

        return variant.Id;
    }

    private static (Guid SellerRef, Guid QualityStatusRef)? TryParseSellerQualityKey(string? key)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            return null;
        }

        var parts = key.Split('|', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        if (parts.Length != 2)
        {
            return null;
        }

        if (!Guid.TryParse(parts[0], out var sellerRef) || !Guid.TryParse(parts[1], out var qualityStatusRef))
        {
            return null;
        }

        return (sellerRef, qualityStatusRef);
    }

    private static List<ConversionDocumentPreviewSelectionModel>? ParseConversionPreviewSelections(string? previewSelectionsJson)
    {
        if (string.IsNullOrWhiteSpace(previewSelectionsJson))
        {
            return null;
        }

        try
        {
            var selections = JsonSerializer.Deserialize<List<ConversionDocumentPreviewSelectionModel>>(previewSelectionsJson, PostDocumentSerialSelectionsJsonOptions);
            if (selections is null || selections.Count == 0)
            {
                return null;
            }

            return selections
                .Where(selection => !string.IsNullOrWhiteSpace(selection.LineKey) && !string.IsNullOrWhiteSpace(selection.LocationRef))
                .Select(selection => new ConversionDocumentPreviewSelectionModel
                {
                    LineKey = selection.LineKey?.Trim() ?? string.Empty,
                    VariantId = selection.VariantId?.Trim() ?? string.Empty,
                    Role = selection.Role?.Trim() ?? string.Empty,
                    LocationRef = selection.LocationRef?.Trim() ?? string.Empty,
                    RequiredQty = selection.RequiredQty
                })
                .ToList();
        }
        catch
        {
            return null;
        }
    }

    private async Task<(bool Success, string? Error, ConversionDocumentPreviewViewModel? Model)> BuildConversionPreviewAsync(
        string token,
        string documentId,
        CreateInventoryDocumentForm form,
        CancellationToken cancellationToken = default)
    {
        var validationError = ValidateConversionDocumentForm(form);
        if (!string.IsNullOrWhiteSpace(validationError))
        {
            return (false, validationError, null);
        }

        if (string.IsNullOrWhiteSpace(documentId))
        {
            return (false, "شناسه سند معتبر نیست.", null);
        }

        var documentResult = await _apiService.GetInventoryDocumentByBusinessKeyAsync(documentId, token);
        if (!documentResult.IsSuccess || documentResult.Data is null)
        {
            return (false, documentResult.ErrorMessage ?? "سند یافت نشد.", null);
        }

        if (!string.Equals(documentResult.Data.DocumentType, "Conversion", StringComparison.OrdinalIgnoreCase))
        {
            return (false, "این سند تبدیل نیست.", null);
        }

        if (!string.Equals(documentResult.Data.Status, "Draft", StringComparison.OrdinalIgnoreCase))
        {
            return (false, "فقط سندهای پیش‌نویس قابل پیش‌نمایش هستند.", null);
        }

        var variantResult = await _apiService.GetProductVariantFullDetailsAsync(form.ConversionVariantId ?? string.Empty, token);
        if (!variantResult.IsSuccess || variantResult.Data is null)
        {
            return (false, variantResult.ErrorMessage ?? "جزئیات واریانت دریافت نشد.", null);
        }

        var variantsResult = await _apiService.SearchVariantsAsync(token, page: 1, pageSize: 2000);
        var variantLookup = (variantsResult.Data ?? new List<ProductVariantSummaryModel>())
            .ToDictionary(x => x.Id, x => x, StringComparer.OrdinalIgnoreCase);

        var uomLookupResult = await _apiService.GetUnitOfMeasureLookupAsync(token);
        var warehouseLookupResult = await _apiService.GetWarehouseLookupAsync(token, includeInactive: true);
        var locationLookupResult = await _apiService.GetLocationLookupAsync(token, warehouseId: null, includeInactive: true);
        var uomLookup = (uomLookupResult.Data ?? new List<UnitOfMeasureLookupModel>())
            .ToDictionary(x => x.Id, x => x, StringComparer.OrdinalIgnoreCase);
        var warehouseLookup = warehouseLookupResult.Data ?? new List<WarehouseLookupItemModel>();
        var locationLookup = locationLookupResult.Data ?? new List<LocationLookupItemModel>();

        var variant = variantResult.Data;
        var recipe = variant.Components
            .Where(x => !string.IsNullOrWhiteSpace(x.ComponentVariantId) && x.Quantity > 0)
            .ToList();

        if (recipe.Count == 0)
        {
            return (false, "برای این واریانت recipe یا جزء سازنده معتبر ثبت نشده است.", null);
        }

        var assemble = string.Equals(form.ConversionOperationType, "Assemble", StringComparison.OrdinalIgnoreCase);
        var previewLines = new List<ConversionDocumentPreviewLineModel>();
        string? selectedGroup = null;

        string BuildLocationLabel(LocationLookupItemModel location)
        {
            var warehouseLabel = warehouseLookup.FirstOrDefault(x => string.Equals(x.WarehouseBusinessKey, location.WarehouseRef, StringComparison.OrdinalIgnoreCase));
            var warehouseText = warehouseLabel is null ? location.WarehouseRef : $"{warehouseLabel.Code} - {warehouseLabel.Name}";
            var locationText = string.IsNullOrWhiteSpace(location.LocationType)
                ? location.LocationCode
                : $"{location.LocationCode} ({location.LocationType})";
            return string.IsNullOrWhiteSpace(warehouseText) ? locationText : $"{warehouseText} - {locationText}";
        }

        string BuildUomLabel(string uomRef)
        {
            if (string.IsNullOrWhiteSpace(uomRef))
            {
                return "-";
            }

            return uomLookup.TryGetValue(uomRef, out var uom)
                ? $"{uom.Code} - {uom.Name}"
                : uomRef;
        }

        List<StockDetailBucketModel> FilterBucketsByGroupAndQty(IEnumerable<StockDetailBucketModel> buckets, string groupKey, decimal requiredQty)
        {
            return buckets
                .Where(x => x.QuantityOnHand >= requiredQty && string.Equals(ToSellerQualityKey(x), groupKey, StringComparison.OrdinalIgnoreCase))
                .OrderByDescending(x => x.QuantityOnHand)
                .ThenBy(x => x.LocationRef)
                .ToList();
        }

        if (assemble)
        {
            var componentCandidates = new List<(string LineKey, string VariantId, string Label, string BaseUomRef, decimal RequiredQty, List<StockDetailBucketModel> Buckets)>();

            foreach (var component in recipe)
            {
                var requiredQty = component.Quantity * form.ConversionQuantity;
                if (!Guid.TryParse(component.ComponentVariantId, out var parsedVariantId))
                {
                    return (false, "شناسه واریانت جزء معتبر نیست.", null);
                }

                var componentVariant = variantLookup.TryGetValue(component.ComponentVariantId, out var componentLookupVariant)
                    ? componentLookupVariant
                    : null;

                var bucketsResult = await _apiService.GetAvailableStockBucketsAsync(token, variantRef: parsedVariantId, minQuantity: requiredQty);
                if (!bucketsResult.IsSuccess)
                {
                    return (false, bucketsResult.ErrorMessage ?? "موجودی جزء سازنده دریافت نشد.", null);
                }

                var buckets = (bucketsResult.Data?.Items ?? new List<StockDetailBucketModel>())
                    .Where(x => x.QuantityOnHand >= requiredQty)
                    .ToList();

                if (buckets.Count == 0)
                {
                    return (false, $"برای جزء سازنده {BuildVariantLookupLabel(componentVariant ?? new ProductVariantSummaryModel { Id = component.ComponentVariantId, Sku = component.ComponentVariantId })} موجودی کافی پیدا نشد.", null);
                }

                componentCandidates.Add((component.ComponentId, component.ComponentVariantId, BuildVariantLookupLabel(componentVariant ?? new ProductVariantSummaryModel { Id = component.ComponentVariantId, Sku = component.ComponentVariantId }), componentVariant?.BaseUomRef ?? string.Empty, requiredQty, buckets));
            }

            var groups = componentCandidates
                .Select(candidate => candidate.Buckets
                    .GroupBy(ToSellerQualityKey)
                    .Where(group => group.Sum(x => x.QuantityOnHand) >= candidate.RequiredQty)
                    .Select(group => group.Key)
                    .ToHashSet(StringComparer.OrdinalIgnoreCase))
                .ToList();

            if (groups.Any(group => group.Count == 0))
            {
                return (false, "برای یکی از اجزای سازنده موجودی کافی در انبار پیدا نشد.", null);
            }

            selectedGroup = groups
                .Skip(1)
                .Aggregate(new HashSet<string>(groups.First(), StringComparer.OrdinalIgnoreCase), (acc, set) =>
                {
                    acc.IntersectWith(set);
                    return acc;
                })
                .FirstOrDefault();

            if (string.IsNullOrWhiteSpace(selectedGroup))
            {
                return (false, "اجزای سازنده در یک seller/quality مشترک برای مونتاژ پیدا نشدند.", null);
            }

            foreach (var candidate in componentCandidates)
            {
                var options = FilterBucketsByGroupAndQty(candidate.Buckets, selectedGroup, candidate.RequiredQty);
                if (options.Count == 0)
                {
                    return (false, $"برای جزء سازنده {candidate.Label} لوکیشن کافی پیدا نشد.", null);
                }

                previewLines.Add(new ConversionDocumentPreviewLineModel
                {
                    LineKey = $"component-{candidate.LineKey}",
                    Role = "Source",
                    VariantId = candidate.VariantId,
                    VariantLabel = candidate.Label,
                    BaseUomRef = candidate.BaseUomRef,
                    BaseUomLabel = BuildUomLabel(candidate.BaseUomRef),
                    RequiredQty = candidate.RequiredQty,
                    SelectedLocationRef = options.First().LocationRef.ToString("D"),
                    LocationOptions = options.Select(bucket =>
                    {
                        var location = locationLookup.FirstOrDefault(x => string.Equals(x.LocationBusinessKey, bucket.LocationRef.ToString("D"), StringComparison.OrdinalIgnoreCase));
                        return new ConversionDocumentPreviewLocationModel
                        {
                            LocationRef = bucket.LocationRef.ToString("D"),
                            Label = location is null ? bucket.LocationRef.ToString("D") : BuildLocationLabel(location),
                            QuantityOnHand = bucket.QuantityOnHand,
                            IsDefault = false
                        };
                    }).ToList()
                });
            }

            var destinationOptions = locationLookup
                .Select(location => new ConversionDocumentPreviewLocationModel
                {
                    LocationRef = location.LocationBusinessKey,
                    Label = BuildLocationLabel(location),
                    QuantityOnHand = 0,
                    IsDefault = false
                })
                .ToList();

            previewLines.Insert(0, new ConversionDocumentPreviewLineModel
            {
                LineKey = "main-variant",
                Role = "Destination",
                VariantId = variant.Id,
                VariantLabel = BuildVariantLookupLabel(variantLookup.TryGetValue(variant.Id, out var mainLookupVariant) ? mainLookupVariant : new ProductVariantSummaryModel { Id = variant.Id, Sku = variant.Sku, Name = variant.Name }),
                BaseUomRef = variant.BaseUomRef,
                BaseUomLabel = BuildUomLabel(variant.BaseUomRef),
                RequiredQty = form.ConversionQuantity,
                SelectedLocationRef = destinationOptions.FirstOrDefault()?.LocationRef,
                LocationOptions = destinationOptions
            });
        }
        else
        {
            if (!Guid.TryParse(variant.Id, out var parsedVariantId))
            {
                return (false, "شناسه واریانت اصلی معتبر نیست.", null);
            }

            var mainBucketsResult = await _apiService.GetAvailableStockBucketsAsync(token, variantRef: parsedVariantId, minQuantity: form.ConversionQuantity);
            if (!mainBucketsResult.IsSuccess)
            {
                return (false, mainBucketsResult.ErrorMessage ?? "موجودی واریانت اصلی دریافت نشد.", null);
            }

            var mainBuckets = (mainBucketsResult.Data?.Items ?? new List<StockDetailBucketModel>())
                .Where(x => x.QuantityOnHand >= form.ConversionQuantity)
                .ToList();

            if (mainBuckets.Count == 0)
            {
                return (false, "برای واریانت اصلی موجودی کافی پیدا نشد.", null);
            }

            var groupKeys = mainBuckets
                .GroupBy(ToSellerQualityKey)
                .Where(group => group.Sum(x => x.QuantityOnHand) >= form.ConversionQuantity)
                .Select(group => group.Key)
                .ToList();

            if (groupKeys.Count == 0)
            {
                return (false, "برای دی‌اسمبل موجودی کافی از واریانت اصلی پیدا نشد.", null);
            }

            if (groupKeys.Count > 1)
            {
                return (false, "برای دی‌اسمبل بیش از یک seller/quality ممکن پیدا شد. ابتدا یکی را یکتا کنید.", null);
            }

            selectedGroup = groupKeys[0];
            var mainOptions = FilterBucketsByGroupAndQty(mainBuckets, selectedGroup, form.ConversionQuantity);
            if (mainOptions.Count == 0)
            {
                return (false, "لوکیشن مناسب برای واریانت اصلی پیدا نشد.", null);
            }

            previewLines.Add(new ConversionDocumentPreviewLineModel
            {
                LineKey = "main-variant",
                Role = "Source",
                VariantId = variant.Id,
                VariantLabel = BuildVariantLookupLabel(variantLookup.TryGetValue(variant.Id, out var mainLookupVariant) ? mainLookupVariant : new ProductVariantSummaryModel { Id = variant.Id, Sku = variant.Sku, Name = variant.Name }),
                BaseUomRef = variant.BaseUomRef,
                BaseUomLabel = BuildUomLabel(variant.BaseUomRef),
                RequiredQty = form.ConversionQuantity,
                SelectedLocationRef = mainOptions.First().LocationRef.ToString("D"),
                LocationOptions = mainOptions.Select(bucket =>
                {
                    var location = locationLookup.FirstOrDefault(x => string.Equals(x.LocationBusinessKey, bucket.LocationRef.ToString("D"), StringComparison.OrdinalIgnoreCase));
                    return new ConversionDocumentPreviewLocationModel
                    {
                        LocationRef = bucket.LocationRef.ToString("D"),
                        Label = location is null ? bucket.LocationRef.ToString("D") : BuildLocationLabel(location),
                        QuantityOnHand = bucket.QuantityOnHand,
                        IsDefault = false
                    };
                }).ToList()
            });

            foreach (var component in recipe)
            {
                var componentVariant = variantLookup.TryGetValue(component.ComponentVariantId, out var componentLookupVariant)
                    ? componentLookupVariant
                    : null;

                previewLines.Add(new ConversionDocumentPreviewLineModel
                {
                    LineKey = $"component-{component.ComponentId}",
                    Role = "Destination",
                    VariantId = component.ComponentVariantId,
                    VariantLabel = BuildVariantLookupLabel(componentVariant ?? new ProductVariantSummaryModel { Id = component.ComponentVariantId, Sku = component.ComponentVariantId }),
                    BaseUomRef = componentVariant?.BaseUomRef ?? string.Empty,
                    BaseUomLabel = BuildUomLabel(componentVariant?.BaseUomRef ?? string.Empty),
                    RequiredQty = component.Quantity * form.ConversionQuantity,
                    SelectedLocationRef = locationLookup.FirstOrDefault()?.LocationBusinessKey,
                    LocationOptions = locationLookup.Select(location => new ConversionDocumentPreviewLocationModel
                    {
                        LocationRef = location.LocationBusinessKey,
                        Label = BuildLocationLabel(location),
                        QuantityOnHand = 0,
                        IsDefault = false
                    }).ToList()
                });
            }
        }

        return (true, null, new ConversionDocumentPreviewViewModel
        {
            DocumentId = documentId,
            DocumentNo = documentResult.Data.DocumentNo,
            VariantId = variant.Id,
            VariantLabel = BuildVariantLookupLabel(variantLookup.TryGetValue(variant.Id, out var previewVariant) ? previewVariant : new ProductVariantSummaryModel { Id = variant.Id, Sku = variant.Sku, Name = variant.Name }),
            OperationType = form.ConversionOperationType,
            Quantity = form.ConversionQuantity,
            ReasonCode = form.ReasonCode,
            SelectedSellerQualityKey = selectedGroup,
            Lines = previewLines
        });
    }

    private static string? NormalizeLotBatchNo(string? lotBatchNo)
        => string.IsNullOrWhiteSpace(lotBatchNo) ? null : lotBatchNo.Trim();

    private static string NormalizeLookupKey(string? value)
        => string.IsNullOrWhiteSpace(value) ? string.Empty : value.Trim().ToUpperInvariant();

    private static IReadOnlyList<SerialItemLookupModel> FilterAvailableSerialsForLine(
        IEnumerable<SerialItemLookupModel> serials,
        string? sourceLocationRef,
        string? qualityStatusRef,
        string? lotBatchNo)
    {
        var filteredSerials = serials.ToList();

        if (Guid.TryParse(sourceLocationRef, out var parsedSourceLocationId))
        {
            var sourceLocationKey = parsedSourceLocationId.ToString("D");
            filteredSerials = filteredSerials
                .Where(x => string.Equals(x.LocationRef, sourceLocationKey, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        if (Guid.TryParse(qualityStatusRef, out var parsedQualityStatusId))
        {
            var qualityStatusKey = parsedQualityStatusId.ToString("D");
            filteredSerials = filteredSerials
                .Where(x => string.Equals(x.QualityStatusRef, qualityStatusKey, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        var normalizedLotBatchNo = NormalizeLotBatchNo(lotBatchNo);
        if (!string.IsNullOrWhiteSpace(normalizedLotBatchNo))
        {
            filteredSerials = filteredSerials
                .Where(x => string.Equals(NormalizeLotBatchNo(x.LotBatchNo), normalizedLotBatchNo, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        return filteredSerials;
    }

    private static bool TryResolveSelectedSerials(
        IEnumerable<InventoryDocumentLineSerialModel>? selectedSerials,
        IReadOnlyCollection<SerialItemLookupModel> availableSerials,
        out List<ResolvedSelectedSerialItem> resolvedSerials,
        out string? errorMessage)
    {
        resolvedSerials = new List<ResolvedSelectedSerialItem>();
        errorMessage = null;

        var selectionList = (selectedSerials ?? Array.Empty<InventoryDocumentLineSerialModel>())
            .Where(x => !string.IsNullOrWhiteSpace(x.SerialItemBusinessKey) || !string.IsNullOrWhiteSpace(x.SerialNo))
            .ToList();

        if (selectionList.Count == 0)
        {
            return true;
        }

        var availableByBusinessKey = availableSerials
            .Where(x => !string.IsNullOrWhiteSpace(x.SerialItemBusinessKey))
            .GroupBy(x => x.SerialItemBusinessKey, StringComparer.OrdinalIgnoreCase)
            .ToDictionary(group => group.Key, group => group.First(), StringComparer.OrdinalIgnoreCase);

        var availableBySerialNo = availableSerials
            .Where(x => !string.IsNullOrWhiteSpace(x.SerialNo))
            .GroupBy(x => x.SerialNo, StringComparer.OrdinalIgnoreCase)
            .ToDictionary(group => group.Key, group => group.First(), StringComparer.OrdinalIgnoreCase);

        var seenKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var selectedSerial in selectionList)
        {
            var lookupKey = !string.IsNullOrWhiteSpace(selectedSerial.SerialItemBusinessKey)
                ? $"id:{selectedSerial.SerialItemBusinessKey.Trim()}"
                : $"no:{selectedSerial.SerialNo.Trim()}";

            if (!seenKeys.Add(lookupKey))
            {
                continue;
            }

            SerialItemLookupModel? availableSerial = null;
            if (!string.IsNullOrWhiteSpace(selectedSerial.SerialItemBusinessKey))
            {
                availableByBusinessKey.TryGetValue(selectedSerial.SerialItemBusinessKey.Trim(), out availableSerial);
            }

            if (availableSerial is null && !string.IsNullOrWhiteSpace(selectedSerial.SerialNo))
            {
                availableBySerialNo.TryGetValue(selectedSerial.SerialNo.Trim(), out availableSerial);
            }

            if (availableSerial is null)
            {
                errorMessage = $"سریال انتخاب شده '{selectedSerial.SerialNo}' در موجودی قابل انتخاب پیدا نشد.";
                resolvedSerials.Clear();
                return false;
            }

            resolvedSerials.Add(new ResolvedSelectedSerialItem(selectedSerial, availableSerial));
        }

        return true;
    }

    private static bool TryAutoAllocateSerialsForLine(
        IReadOnlyCollection<SerialItemLookupModel> availableSerials,
        decimal requestedQty,
        out List<ResolvedSelectedSerialItem> resolvedSerials,
        out string? errorMessage)
    {
        resolvedSerials = new List<ResolvedSelectedSerialItem>();
        errorMessage = null;

        if (requestedQty <= 0)
        {
            errorMessage = "مقدار باید بزرگ‌تر از صفر باشد.";
            return false;
        }

        if (requestedQty != decimal.Truncate(requestedQty))
        {
            errorMessage = "برای کالاهای سریالی، مقدار باید عدد صحیح باشد.";
            return false;
        }

        var requestedSerialCount = (int)requestedQty;
        if (availableSerials.Count == 0)
        {
            return true;
        }

        if (availableSerials.Count < requestedSerialCount)
        {
            errorMessage = $"فقط {availableSerials.Count} سریال قابل انتخاب موجود است.";
            return false;
        }

        resolvedSerials = availableSerials
            .OrderBy(x => x.DateScannedIn)
            .ThenBy(x => x.LastUpdatedAt)
            .ThenBy(x => x.SerialNo, StringComparer.OrdinalIgnoreCase)
            .ThenBy(x => x.SerialItemBusinessKey, StringComparer.OrdinalIgnoreCase)
            .Take(requestedSerialCount)
            .Select(item => new ResolvedSelectedSerialItem(
                new InventoryDocumentLineSerialModel
                {
                    SerialItemBusinessKey = item.SerialItemBusinessKey,
                    SerialNo = item.SerialNo
                },
                item))
            .ToList();

        return true;
    }

    private async Task<AutoAllocationSourceResult> TryAutoAllocateSourceAllocationsAsync(
        string token,
        InventoryDocumentDetailsModel document,
        ProductVariantSummaryModel variant,
        InventoryDocumentLineForm form)
    {
        var result = new AutoAllocationSourceResult();

        if (!Guid.TryParse(variant.Id, out var parsedVariantId))
        {
            result.ErrorMessage = "واریانت انتخاب شده معتبر نیست.";
            return result;
        }

        if (!Guid.TryParse(document.WarehouseRef, out var parsedWarehouseRef))
        {
            result.ErrorMessage = "انبار سند معتبر نیست.";
            return result;
        }

        if (form.Qty <= 0)
        {
            result.ErrorMessage = "مقدار باید بزرگ‌تر از صفر باشد.";
            return result;
        }

        var bucketsResult = await _apiService.GetAvailableStockBucketsAsync(token, parsedVariantId);
        if (!bucketsResult.IsSuccess)
        {
            result.ErrorMessage = bucketsResult.ErrorMessage ?? "بارگذاری بالانس‌های قابل تخصیص انجام نشد.";
            return result;
        }

        var requestedLotBatchNo = NormalizeLotBatchNo(form.LotBatchNo);
        var sourceLocationKey = NormalizeLookupKey(form.SourceLocationRef);
        var qualityStatusKey = NormalizeLookupKey(form.QualityStatusRef);

        var candidateBuckets = (bucketsResult.Data?.Items ?? new List<StockDetailBucketModel>())
            .Where(bucket => bucket.WarehouseRef == parsedWarehouseRef)
            .Where(bucket => BucketMatchesScope(bucket, sourceLocationKey, qualityStatusKey, requestedLotBatchNo))
            .ToList();

        if (candidateBuckets.Count == 0)
        {
            result.ErrorMessage = "هیچ منبعی برای تخصیص خودکار در محدوده انتخاب شده پیدا نشد.";
            return result;
        }

        var serialsResult = await _apiService.GetAvailableSerialItemsAsync(token, variant.Id, document.WarehouseRef);
        if (!serialsResult.IsSuccess)
        {
            result.ErrorMessage = serialsResult.ErrorMessage ?? "بارگذاری سریال‌های قابل تخصیص انجام نشد.";
            return result;
        }

        var availableSerials = FilterAvailableSerialsForLine(
                serialsResult.Data ?? new List<SerialItemLookupModel>(),
                form.SourceLocationRef,
                form.QualityStatusRef,
                form.LotBatchNo)
            .OrderBy(x => x.DateScannedIn)
            .ThenBy(x => x.LastUpdatedAt)
            .ThenBy(x => x.SerialNo, StringComparer.OrdinalIgnoreCase)
            .ThenBy(x => x.SerialItemBusinessKey, StringComparer.OrdinalIgnoreCase)
            .ToList();

        var bucketCandidates = candidateBuckets
            .Select(bucket =>
            {
                var bucketSerialCount = availableSerials.Count(serial => SerialMatchesBucket(serial, bucket));
                var allocatableQty = bucketSerialCount > 0
                    ? Math.Min(bucket.QuantityOnHand, bucketSerialCount)
                    : bucket.QuantityOnHand;

                return new AutoAllocationBucketCandidate(bucket, allocatableQty);
            })
            .ToList();

        var totalAvailable = bucketCandidates.Sum(x => x.AllocatableQty);
        if (totalAvailable < form.Qty)
        {
            result.ErrorMessage = $"فقط {totalAvailable} واحد در محدوده انتخاب‌شده قابل تخصیص است.";
            return result;
        }

        var allocations = new List<AutoAllocationSourceChunk>();
        var remainingQty = form.Qty;
        var remainingSerialPool = availableSerials.ToList();

        foreach (var candidate in bucketCandidates)
        {
            if (remainingQty <= 0)
            {
                break;
            }

            var bucketQty = Math.Min(candidate.AllocatableQty, remainingQty);
            if (bucketQty <= 0)
            {
                continue;
            }

            var bucketSerials = remainingSerialPool
                .Where(serial => SerialMatchesBucket(serial, candidate.Bucket))
                .ToList();

            if (bucketSerials.Count > 0)
            {
                if (bucketQty != decimal.Truncate(bucketQty))
                {
                    result.ErrorMessage = $"منبع سریال‌دار '{candidate.Bucket.LotBatchNo ?? candidate.Bucket.StockDetailBusinessKey.ToString("D")}' فقط با تعداد صحیح قابل تخصیص است.";
                    return result;
                }

                var requiredSerialCount = (int)bucketQty;
                if (bucketSerials.Count < requiredSerialCount)
                {
                    result.ErrorMessage = $"برای منبع '{candidate.Bucket.LotBatchNo ?? candidate.Bucket.StockDetailBusinessKey.ToString("D")}' فقط {bucketSerials.Count} سریال قابل انتخاب موجود است.";
                    return result;
                }

                var allocatedSerials = bucketSerials.Take(requiredSerialCount).ToList();
                allocations.Add(new AutoAllocationSourceChunk(candidate.Bucket, bucketQty, allocatedSerials));

                foreach (var allocatedSerial in allocatedSerials)
                {
                    remainingSerialPool.Remove(allocatedSerial);
                }
            }
            else
            {
                allocations.Add(new AutoAllocationSourceChunk(candidate.Bucket, bucketQty, new List<SerialItemLookupModel>()));
            }

            remainingQty -= bucketQty;
        }

        if (remainingQty > 0)
        {
            result.ErrorMessage = $"فقط {totalAvailable} واحد در محدوده انتخاب‌شده قابل تخصیص است.";
            return result;
        }

        result.Success = true;
        result.Allocations = MergeAutoAllocationSourceChunks(allocations);
        return result;
    }

    private async Task<(bool Success, string? ErrorMessage)> TryResolveReturnSourceSelectionAsync(
        string token,
        InventoryDocumentDetailsModel document,
        InventoryDocumentLineForm form,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(document.ReferenceBusinessId))
        {
            return (false, "برای سند برگشت، سند مرجع باید انتخاب شود.");
        }

        var referenceResult = await _apiService.GetInventoryDocumentByBusinessKeyAsync(document.ReferenceBusinessId, token);
        if (!referenceResult.IsSuccess || referenceResult.Data is null)
        {
            return (false, referenceResult.ErrorMessage ?? "سند مرجع یافت نشد.");
        }

        var referenceDocument = referenceResult.Data;
        var referenceLines = referenceDocument.Lines
            .Where(line => string.Equals(line.VariantRef, form.VariantId, StringComparison.OrdinalIgnoreCase))
            .ToList();

        if (referenceLines.Count == 0)
        {
            return (false, $"سند مرجع برای واریانت انتخاب‌شده '{form.VariantId}' منبعی ندارد.");
        }

        var requestedSourceLocationRef = NormalizeLookupKey(form.SourceLocationRef);
        var requestedDestinationLocationRef = NormalizeLookupKey(form.DestinationLocationRef);
        var requestedLotBatchNo = NormalizeLotBatchNo(form.LotBatchNo);

        var scopedReferenceLines = referenceLines
            .Where(line => ReturnReferenceLineMatchesScope(
                document.DocumentType,
                line,
                requestedSourceLocationRef,
                requestedDestinationLocationRef,
                requestedLotBatchNo))
            .ToList();

        if (scopedReferenceLines.Count == 0)
        {
            return (false, "در سند مرجع، منبعی مطابق واریانت و فیلترهای انتخاب‌شده پیدا نشد.");
        }

        var availableSerials = BuildReturnReferenceSerialCandidates(referenceDocument, scopedReferenceLines, document.DocumentType);
        if (!TryResolveSelectedSerials(form.Serials, availableSerials, out var resolvedSerialSelections, out var selectedSerialError))
        {
            return (false, selectedSerialError ?? "انتخاب سریال‌های سند برگشت معتبر نیست.");
        }

        InventoryDocumentLineDetailsModel sourceLine;
        if (resolvedSerialSelections.Count > 0)
        {
            if (form.Qty != decimal.Truncate(form.Qty) || resolvedSerialSelections.Count != (int)form.Qty)
            {
                return (false, "برای ردیف‌های سریال‌دار، تعداد سریال‌های انتخاب‌شده باید با مقدار ردیف برابر باشد.");
            }

            var selectedLots = resolvedSerialSelections
                .Select(x => NormalizeLotBatchNo(x.AvailableSerial.LotBatchNo))
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            if (selectedLots.Count > 1)
            {
                return (false, "سریال‌های انتخاب‌شده به بیش از یک لات تعلق دارند. برای هر لات یک ردیف جدا ثبت کنید.");
            }

            sourceLine = ResolveReturnSourceLine(scopedReferenceLines, resolvedSerialSelections);
        }
        else
        {
            sourceLine = scopedReferenceLines[0];
            var fifoSourceLineSerials = BuildReturnReferenceSerialCandidates(referenceDocument, new[] { sourceLine }, document.DocumentType);
            if (fifoSourceLineSerials.Count > 0)
            {
                return (false, "برای منبع سریال‌دار انتخاب‌شده، انتخاب سریال الزامی است.");
            }
        }

        ApplyReturnSourceResolution(document.DocumentType, sourceLine, resolvedSerialSelections, form);
        return (true, null);
    }

    private static InventoryDocumentLineDetailsModel ResolveReturnSourceLine(
        IReadOnlyList<InventoryDocumentLineDetailsModel> referenceLines,
        IReadOnlyList<ResolvedSelectedSerialItem> resolvedSerialSelections)
    {
        if (resolvedSerialSelections.Count > 0)
        {
            var selectedSerial = resolvedSerialSelections[0].AvailableSerial;
            var selectedLine = referenceLines.FirstOrDefault(line => line.Serials.Any(serial =>
                string.Equals(serial.SerialItemBusinessKey, selectedSerial.SerialItemBusinessKey, StringComparison.OrdinalIgnoreCase)
                || string.Equals(serial.SerialNo, selectedSerial.SerialNo, StringComparison.OrdinalIgnoreCase)));

            if (selectedLine is not null)
            {
                return selectedLine;
            }
        }

        return referenceLines[0];
    }

    private static IReadOnlyList<SerialItemLookupModel> BuildReturnReferenceSerialCandidates(
        InventoryDocumentDetailsModel referenceDocument,
        IReadOnlyList<InventoryDocumentLineDetailsModel> referenceLines,
        string returnDocumentType)
    {
        var serials = new List<SerialItemLookupModel>();

        foreach (var (line, lineIndex) in referenceLines.Select((line, index) => (line, index)))
        {
            var lineLocationRef = ResolveReturnReferenceSourceLocationRef(returnDocumentType, line)
                ?? ResolveReturnReferenceDestinationLocationRef(returnDocumentType, line);
            var lineQualityStatusRef = ResolveReturnReferenceQualityStatusRef(line);

            foreach (var (serial, serialIndex) in line.Serials.Select((serial, index) => (serial, index)))
            {
                if (string.IsNullOrWhiteSpace(serial.SerialItemBusinessKey) && string.IsNullOrWhiteSpace(serial.SerialNo))
                {
                    continue;
                }

                serials.Add(new SerialItemLookupModel
                {
                    SerialItemBusinessKey = serial.SerialItemBusinessKey ?? string.Empty,
                    SerialNo = serial.SerialNo ?? string.Empty,
                    VariantRef = line.VariantRef,
                    SellerRef = referenceDocument.SellerRef,
                    WarehouseRef = referenceDocument.WarehouseRef,
                    LocationRef = lineLocationRef ?? string.Empty,
                    QualityStatusRef = lineQualityStatusRef ?? string.Empty,
                    LotBatchNo = NormalizeLotBatchNo(line.LotBatchNo),
                    Status = "Available",
                    DateScannedIn = referenceDocument.OccurredAt.AddTicks((lineIndex * 1000L) + serialIndex),
                    LastUpdatedAt = referenceDocument.OccurredAt.AddTicks((lineIndex * 1000L) + serialIndex)
                });
            }
        }

        return serials;
    }

    private static string ResolveReturnSelectionDocumentType(string? returnDocumentType, string? referenceDocumentType)
    {
        var normalizedReturnDocumentType = NormalizeDocumentType(returnDocumentType);
        if (IsReturnDocumentType(normalizedReturnDocumentType))
        {
            return normalizedReturnDocumentType;
        }

        return NormalizeDocumentType(referenceDocumentType) switch
        {
            "Receipt" => "ReturnFromBuy",
            "Transfer" => "ReturnFromTransfer",
            _ => "ReturnFromSell"
        };
    }

    private static string? ResolveReturnReferenceSourceLocationRef(string returnDocumentType, InventoryDocumentLineDetailsModel line)
    {
        return returnDocumentType switch
        {
            "ReturnFromBuy" => line.DestinationLocationRef ?? line.SourceLocationRef,
            "ReturnFromTransfer" => line.DestinationLocationRef ?? line.SourceLocationRef,
            _ => null
        };
    }

    private static string? ResolveReturnReferenceDestinationLocationRef(string returnDocumentType, InventoryDocumentLineDetailsModel line)
    {
        return returnDocumentType switch
        {
            "ReturnFromSell" => line.SourceLocationRef ?? line.DestinationLocationRef,
            "ReturnFromTransfer" => line.SourceLocationRef ?? line.DestinationLocationRef,
            _ => null
        };
    }

    private static string? ResolveReturnReferenceQualityStatusRef(InventoryDocumentLineDetailsModel line)
        => line.QualityStatusRef ?? line.FromQualityStatusRef ?? line.ToQualityStatusRef;

    private static bool ReturnReferenceLineMatchesScope(
        string returnDocumentType,
        InventoryDocumentLineDetailsModel line,
        string? sourceLocationRef,
        string? destinationLocationRef,
        string? lotBatchNo)
    {
        var lineSourceLocationRef = NormalizeLookupKey(ResolveReturnReferenceSourceLocationRef(returnDocumentType, line));
        var lineDestinationLocationRef = NormalizeLookupKey(ResolveReturnReferenceDestinationLocationRef(returnDocumentType, line));

        if (!string.IsNullOrWhiteSpace(sourceLocationRef)
            && !string.Equals(lineSourceLocationRef, sourceLocationRef, StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        if (!string.IsNullOrWhiteSpace(destinationLocationRef)
            && !string.Equals(lineDestinationLocationRef, destinationLocationRef, StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        if (!string.IsNullOrWhiteSpace(lotBatchNo)
            && !string.Equals(NormalizeLotBatchNo(line.LotBatchNo), lotBatchNo, StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        return true;
    }

    private static void ApplyReturnSourceResolution(
        string returnDocumentType,
        InventoryDocumentLineDetailsModel sourceLine,
        IReadOnlyList<ResolvedSelectedSerialItem> resolvedSerialSelections,
        InventoryDocumentLineForm form)
    {
        form.Serials = resolvedSerialSelections
            .Select(x => x.RequestedSerial)
            .ToList();

        if (string.IsNullOrWhiteSpace(form.LotBatchNo))
        {
            form.LotBatchNo = NormalizeLotBatchNo(sourceLine.LotBatchNo);
        }

        if (string.IsNullOrWhiteSpace(form.QualityStatusRef))
        {
            form.QualityStatusRef = ResolveReturnReferenceQualityStatusRef(sourceLine);
        }

        switch (returnDocumentType)
        {
            case "ReturnFromBuy":
                if (string.IsNullOrWhiteSpace(form.SourceLocationRef))
                {
                    form.SourceLocationRef = ResolveReturnReferenceSourceLocationRef(returnDocumentType, sourceLine);
                }
                break;
            case "ReturnFromSell":
                if (string.IsNullOrWhiteSpace(form.DestinationLocationRef))
                {
                    form.DestinationLocationRef = ResolveReturnReferenceDestinationLocationRef(returnDocumentType, sourceLine);
                }
                break;
            case "ReturnFromTransfer":
                if (string.IsNullOrWhiteSpace(form.SourceLocationRef))
                {
                    form.SourceLocationRef = ResolveReturnReferenceSourceLocationRef(returnDocumentType, sourceLine);
                }

                if (string.IsNullOrWhiteSpace(form.DestinationLocationRef))
                {
                    form.DestinationLocationRef = ResolveReturnReferenceDestinationLocationRef(returnDocumentType, sourceLine);
                }
                break;
        }
    }

    private static string? ValidateReturnLineLocations(string returnDocumentType, InventoryDocumentLineForm form)
    {
        return returnDocumentType switch
        {
            "ReturnFromBuy" when string.IsNullOrWhiteSpace(form.SourceLocationRef)
                => "برای سند برگشت از خرید، لوکیشن مبدأ الزامی است.",
            "ReturnFromSell" when string.IsNullOrWhiteSpace(form.DestinationLocationRef)
                => "برای سند برگشت از فروش، لوکیشن مقصد الزامی است.",
            "ReturnFromTransfer" when string.IsNullOrWhiteSpace(form.SourceLocationRef) || string.IsNullOrWhiteSpace(form.DestinationLocationRef)
                => "برای سند برگشت از انتقال، لوکیشن مبدأ و مقصد الزامی هستند.",
            _ => null
        };
    }

    private static bool BucketMatchesScope(
        StockDetailBucketModel bucket,
        string? sourceLocationKey,
        string? qualityStatusKey,
        string? lotBatchNo)
    {
        if (!string.IsNullOrWhiteSpace(sourceLocationKey) && !string.Equals(NormalizeLookupKey(bucket.LocationRef.ToString("D")), sourceLocationKey, StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        if (!string.IsNullOrWhiteSpace(qualityStatusKey) && !string.Equals(NormalizeLookupKey(bucket.QualityStatusRef.ToString("D")), qualityStatusKey, StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        if (!string.IsNullOrWhiteSpace(lotBatchNo) && !string.Equals(NormalizeLotBatchNo(bucket.LotBatchNo), lotBatchNo, StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        return true;
    }

    private static bool SerialMatchesBucket(SerialItemLookupModel serial, StockDetailBucketModel bucket)
    {
        if (Guid.TryParse(serial.StockDetailRef, out var serialBucketRef))
        {
            return serialBucketRef == bucket.StockDetailBusinessKey;
        }

        return string.Equals(NormalizeLookupKey(serial.LocationRef), NormalizeLookupKey(bucket.LocationRef.ToString("D")), StringComparison.OrdinalIgnoreCase)
            && string.Equals(NormalizeLookupKey(serial.QualityStatusRef), NormalizeLookupKey(bucket.QualityStatusRef.ToString("D")), StringComparison.OrdinalIgnoreCase)
            && string.Equals(NormalizeLotBatchNo(serial.LotBatchNo), NormalizeLotBatchNo(bucket.LotBatchNo), StringComparison.OrdinalIgnoreCase);
    }

    private sealed class AutoAllocationSourceResult
    {
        public bool Success { get; set; }
        public string? ErrorMessage { get; set; }
        public List<AutoAllocationSourceChunk> Allocations { get; set; } = new();
    }

    private sealed record AutoAllocationBucketCandidate(
        StockDetailBucketModel Bucket,
        decimal AllocatableQty);

    private sealed record AutoAllocationSourceChunk(
        StockDetailBucketModel Bucket,
        decimal BaseQty,
        List<SerialItemLookupModel> Serials);

    private static List<AutoAllocationSourceChunk> MergeAutoAllocationSourceChunks(IEnumerable<AutoAllocationSourceChunk> allocations)
    {
        var merged = new List<AutoAllocationSourceChunk>();
        var indexByKey = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

        foreach (var allocation in allocations)
        {
            var key = string.Join("|",
                NormalizeLookupKey(allocation.Bucket.LocationRef.ToString("D")) ?? string.Empty,
                NormalizeLookupKey(allocation.Bucket.QualityStatusRef.ToString("D")) ?? string.Empty,
                NormalizeLotBatchNo(allocation.Bucket.LotBatchNo) ?? string.Empty,
                allocation.Serials.Count > 0 ? "serial" : "qty");

            if (!indexByKey.TryGetValue(key, out var index))
            {
                indexByKey[key] = merged.Count;
                merged.Add(new AutoAllocationSourceChunk(
                    allocation.Bucket,
                    allocation.BaseQty,
                    allocation.Serials.ToList()));
                continue;
            }

            var current = merged[index];
            merged[index] = new AutoAllocationSourceChunk(
                current.Bucket,
                current.BaseQty + allocation.BaseQty,
                current.Serials
                    .Concat(allocation.Serials)
                    .GroupBy(x => string.IsNullOrWhiteSpace(x.SerialItemBusinessKey)
                        ? $"no:{x.SerialNo}"
                        : $"id:{x.SerialItemBusinessKey}", StringComparer.OrdinalIgnoreCase)
                    .Select(group => group.First())
                    .ToList());
        }

        return merged;
    }

    private static InventoryDocumentLineForm CloneInventoryDocumentLineForm(InventoryDocumentLineForm source)
    {
        return new InventoryDocumentLineForm
        {
            DocumentId = source.DocumentId,
            DocumentType = source.DocumentType,
            LineId = source.LineId,
            VariantId = source.VariantId,
            Qty = source.Qty,
            UomRef = source.UomRef,
            BaseUomRef = source.BaseUomRef,
            SourceLocationRef = source.SourceLocationRef,
            DestinationLocationRef = source.DestinationLocationRef,
            QualityStatusRef = source.QualityStatusRef,
            FromQualityStatusRef = source.FromQualityStatusRef,
            ToQualityStatusRef = source.ToQualityStatusRef,
            LotBatchNo = source.LotBatchNo,
            ReasonCode = source.ReasonCode,
            AdjustmentDirection = source.AdjustmentDirection,
            Serials = source.Serials
                .Select(serial => new InventoryDocumentLineSerialModel
                {
                    SerialItemBusinessKey = serial.SerialItemBusinessKey,
                    SerialNo = serial.SerialNo
                })
                .ToList()
        };
    }

    private sealed record ResolvedSelectedSerialItem(
        InventoryDocumentLineSerialModel RequestedSerial,
        SerialItemLookupModel AvailableSerial);

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

