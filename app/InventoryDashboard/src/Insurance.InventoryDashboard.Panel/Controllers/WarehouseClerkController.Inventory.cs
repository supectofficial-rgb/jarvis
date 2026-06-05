using System.Text.Json;
using Insurance.InventoryDashboard.Panel.Models;
using Microsoft.AspNetCore.Mvc;

namespace Insurance.InventoryDashboard.Panel.Controllers;

public sealed partial class WarehouseClerkController
{
    [HttpGet]
    public async Task<IActionResult> InventorySerials(
        string variantId,
        string? warehouseId = null,
        string? structureSelectionsJson = null,
        CancellationToken cancellationToken = default)
    {
        if (!TryGetToken(out var token))
        {
            return Unauthorized(new { isSuccess = false, errorMessage = "نشست کاربر در دسترس نیست." });
        }

        if (!Guid.TryParse(variantId, out var variantRef))
        {
            return BadRequest(new { isSuccess = false, errorMessage = "شناسه واریانت معتبر نیست." });
        }

        var normalizedWarehouseId = string.IsNullOrWhiteSpace(warehouseId) ? null : warehouseId.Trim();
        var selectedLocationIds = await LoadMatchingLocationIdsAsync(token, normalizedWarehouseId, structureSelectionsJson, cancellationToken);
        var hasStructureFilters = !string.IsNullOrWhiteSpace(normalizedWarehouseId) && !string.IsNullOrWhiteSpace(structureSelectionsJson);

        var serialResult = await _apiService.SearchSerialItemsAsync(
            token,
            variantRef.ToString("D"),
            normalizedWarehouseId,
            status: "Available");
        var warehouseLookupResult = await _apiService.GetWarehouseLookupAsync(token, includeInactive: true);
        var locationLookupResult = await _apiService.GetLocationLookupAsync(token, warehouseId: null, includeInactive: true);
        var qualityStatusLookupResult = await _apiService.GetQualityStatusLookupAsync(token, includeInactive: true);

        if (!serialResult.IsSuccess || !warehouseLookupResult.IsSuccess || !locationLookupResult.IsSuccess || !qualityStatusLookupResult.IsSuccess)
        {
            var errorMessage = string.Join(" | ", new[]
            {
                serialResult.ErrorMessage,
                warehouseLookupResult.ErrorMessage,
                locationLookupResult.ErrorMessage,
                qualityStatusLookupResult.ErrorMessage
            }.Where(x => !string.IsNullOrWhiteSpace(x)));

            return Json(new { isSuccess = false, errorMessage });
        }

        var warehouseLookup = (warehouseLookupResult.Data ?? new List<WarehouseLookupItemModel>())
            .ToDictionary(x => x.WarehouseBusinessKey, x => x, StringComparer.OrdinalIgnoreCase);
        var locationLookup = (locationLookupResult.Data ?? new List<LocationLookupItemModel>())
            .ToDictionary(x => x.LocationBusinessKey, x => x, StringComparer.OrdinalIgnoreCase);
        var qualityStatusLookup = (qualityStatusLookupResult.Data ?? new List<QualityStatusLookupItemModel>())
            .ToDictionary(x => x.QualityStatusBusinessKey, x => x, StringComparer.OrdinalIgnoreCase);

        var items = (serialResult.Data ?? new List<SerialItemLookupModel>())
            .Where(x => string.IsNullOrWhiteSpace(normalizedWarehouseId) || string.Equals(x.WarehouseRef, normalizedWarehouseId, StringComparison.OrdinalIgnoreCase))
            .Where(x => !hasStructureFilters || selectedLocationIds.Contains(x.LocationRef))
            .OrderBy(x => x.SerialNo)
            .Select(x =>
            {
                warehouseLookup.TryGetValue(x.WarehouseRef, out var warehouse);
                locationLookup.TryGetValue(x.LocationRef, out var location);
                qualityStatusLookup.TryGetValue(x.QualityStatusRef, out var qualityStatus);

                return new WarehouseClerkInventorySerialItemModel
                {
                    SerialItemBusinessKey = x.SerialItemBusinessKey,
                    SerialNo = x.SerialNo,
                    WarehouseRef = x.WarehouseRef,
                    WarehouseLabel = warehouse is null ? x.WarehouseRef : $"{warehouse.Code} - {warehouse.Name}",
                    LocationRef = x.LocationRef,
                    LocationLabel = location is null
                        ? x.LocationRef
                        : string.IsNullOrWhiteSpace(location.LocationType)
                            ? location.LocationCode
                            : $"{location.LocationCode} ({location.LocationType})",
                    QualityStatusRef = x.QualityStatusRef,
                    QualityStatusLabel = qualityStatus is null ? x.QualityStatusRef : $"{qualityStatus.Code} - {qualityStatus.Name}",
                    LotBatchNo = x.LotBatchNo,
                    Status = x.Status,
                    DateScannedIn = x.DateScannedIn,
                    LastUpdatedAt = x.LastUpdatedAt
                };
            })
            .ToList();

        return Json(new
        {
            isSuccess = true,
            items
        });
    }

    private async Task<IActionResult> BuildInventoryPageAsync(
        string? warehouseId,
        string? structureSelectionsJson,
        int page,
        int pageSize,
        CancellationToken cancellationToken)
    {
        if (!TryGetToken(out var token))
        {
            return RedirectToAction("Login", "Auth");
        }

        var roles = ResolveRolesFromSession(token);
        if (!IsAuthorizedFor(token, "Inventory.Warehouse.View", "Warehouse.Read", "Warehouse.Search"))
        {
            TempData["CatalogError"] = "شما دسترسی مشاهده موجودی را ندارید.";
            return RedirectToAction("Index", "Dashboard");
        }

        var modules = await _dashboardConfigService.GetMenuByRolesAsync(roles, cancellationToken);
        var menu = ResolveMenu(modules, "warehouse_operations", "inventory");

        pageSize = NormalizePageSize(pageSize);
        page = Math.Max(page, 1);

        var warehouseLookupResult = await _apiService.GetWarehouseLookupAsync(token, includeInactive: true);
        var warehouseLookup = warehouseLookupResult.Data ?? new List<WarehouseLookupItemModel>();
        var selectedWarehouseId = string.IsNullOrWhiteSpace(warehouseId) ? null : warehouseId.Trim();
        var selectedWarehouse = warehouseLookup.FirstOrDefault(x => string.Equals(x.WarehouseBusinessKey, selectedWarehouseId, StringComparison.OrdinalIgnoreCase));
        var selectedWarehouseName = selectedWarehouse is null
            ? null
            : $"{selectedWarehouse.Code} - {selectedWarehouse.Name}";

        var structureTreeResult = !string.IsNullOrWhiteSpace(selectedWarehouseId)
            ? await _apiService.GetWarehouseLocationStructureTreeAsync(selectedWarehouseId, token, includeInactive: true)
            : new ApiResponse<List<LocationStructureTreeItemModel>> { IsSuccess = true, Data = new List<LocationStructureTreeItemModel>() };

        var matchingLocationIds = !string.IsNullOrWhiteSpace(selectedWarehouseId) && !string.IsNullOrWhiteSpace(structureSelectionsJson)
            ? await LoadMatchingLocationIdsAsync(token, selectedWarehouseId, structureSelectionsJson, cancellationToken)
            : new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var hasStructureFilters = !string.IsNullOrWhiteSpace(selectedWarehouseId) && !string.IsNullOrWhiteSpace(structureSelectionsJson);

        var stockResult = !string.IsNullOrWhiteSpace(selectedWarehouseId)
            ? await LoadWarehouseStockDetailsAsync(token, selectedWarehouseId, cancellationToken)
            : new ApiResponse<List<StockDetailListItemModel>> { IsSuccess = true, Data = new List<StockDetailListItemModel>() };

        var variantLookupResult = await _apiService.SearchVariantsAsync(token, isActive: null, page: 1, pageSize: 2000);

        var stockError = stockResult.IsSuccess ? null : stockResult.ErrorMessage;
        var variantError = variantLookupResult.IsSuccess ? null : variantLookupResult.ErrorMessage;
        var structureError = structureTreeResult.ErrorMessage;

        var filteredStock = (stockResult.Data ?? new List<StockDetailListItemModel>())
            .Where(x => x.QuantityOnHand > 0)
            .Where(x => !hasStructureFilters || matchingLocationIds.Contains(x.LocationRef))
            .ToList();

        var grouped = filteredStock
            .GroupBy(x => x.VariantRef, StringComparer.OrdinalIgnoreCase)
            .Select(group => new WarehouseClerkInventoryItemModel
            {
                VariantRef = group.Key,
                QuantityOnHand = group.Sum(x => x.QuantityOnHand),
                BucketCount = group.Count()
            })
            .ToList();

        var variantLookup = (variantLookupResult.Data ?? new List<ProductVariantSummaryModel>())
            .Where(x => !string.IsNullOrWhiteSpace(x.Id))
            .GroupBy(x => x.Id, StringComparer.OrdinalIgnoreCase)
            .ToDictionary(g => g.Key, g => g.First(), StringComparer.OrdinalIgnoreCase);

        grouped = grouped
            .OrderByDescending(x => x.QuantityOnHand)
            .ThenBy(x => GetVariantLabel(x.VariantRef, variantLookup), StringComparer.OrdinalIgnoreCase)
            .ToList();

        var totalCount = grouped.Count;
        var totalPages = CalculateTotalPages(totalCount, pageSize);
        var pagedItems = grouped
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        var serialCountLookup = await BuildSerialCountLookupAsync(
            token,
            pagedItems.Select(x => x.VariantRef),
            selectedWarehouseId,
            matchingLocationIds,
            cancellationToken);

        foreach (var item in pagedItems)
        {
            variantLookup.TryGetValue(item.VariantRef, out var variant);
            item.VariantSku = variant?.Sku ?? string.Empty;
            item.VariantName = variant?.Name ?? string.Empty;
            item.SerialCount = serialCountLookup.TryGetValue(item.VariantRef, out var serialCount) ? serialCount : 0;
        }

        var model = new WarehouseClerkInventoryPageViewModel
        {
            UserName = HttpContext.Session.GetString("UserName") ?? "کاربر",
            Roles = roles,
            Permissions = ResolvePermissionsFromSession(),
            Modules = modules,
            ActiveModule = menu.Module,
            ActiveItem = menu.Item,
            ErrorMessage = JoinErrors(stockError, variantError, structureError, warehouseLookupResult.ErrorMessage),
            WarehouseLookup = warehouseLookup,
            SelectedWarehouseId = selectedWarehouseId,
            SelectedWarehouseName = selectedWarehouseName,
            StructureSelectionsJson = structureSelectionsJson,
            Structures = structureTreeResult.Data ?? new List<LocationStructureTreeItemModel>(),
            Items = pagedItems,
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount,
            TotalPages = totalPages
        };

        SetLayoutViewBag(model.Modules, model.ActiveModule?.ModuleId, model.ActiveItem?.ItemId, model.UserName);
        return View("~/Views/WarehouseClerk/Inventory.cshtml", model);
    }

    private async Task<ApiResponse<List<StockDetailListItemModel>>> LoadWarehouseStockDetailsAsync(
        string token,
        string warehouseId,
        CancellationToken cancellationToken)
    {
        var allItems = new List<StockDetailListItemModel>();
        var page = 1;
        const int pageSize = 200;

        while (true)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var result = await _apiService.SearchStockDetailsAsync(
                token,
                warehouseId: warehouseId,
                isEmpty: false,
                page: page,
                pageSize: pageSize);

            if (!result.IsSuccess)
            {
                return new ApiResponse<List<StockDetailListItemModel>>
                {
                    IsSuccess = false,
                    ErrorMessage = result.ErrorMessage,
                    Data = allItems
                };
            }

            var items = result.Data?.Items ?? new List<StockDetailListItemModel>();
            allItems.AddRange(items);

            var returnedPageSize = result.Data?.PageSize ?? pageSize;
            var totalPages = CalculateTotalPages(result.Data?.TotalCount ?? allItems.Count, returnedPageSize);
            if (page >= totalPages || items.Count < returnedPageSize)
            {
                break;
            }

            page++;
        }

        return new ApiResponse<List<StockDetailListItemModel>>
        {
            IsSuccess = true,
            Data = allItems
        };
    }

    private async Task<HashSet<string>> LoadMatchingLocationIdsAsync(
        string token,
        string? warehouseId,
        string? structureSelectionsJson,
        CancellationToken cancellationToken)
    {
        var locationIds = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        if (string.IsNullOrWhiteSpace(warehouseId) || string.IsNullOrWhiteSpace(structureSelectionsJson))
        {
            return locationIds;
        }

        var page = 1;
        const int pageSize = 200;

        while (true)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var result = await _apiService.SearchLocationsAsync(
                token,
                warehouseId,
                page: page,
                pageSize: pageSize,
                structureSelectionsJson: structureSelectionsJson);

            if (!result.IsSuccess)
            {
                return locationIds;
            }

            var items = result.Data?.Items ?? new List<LocationListItemModel>();
            foreach (var item in items)
            {
                if (!string.IsNullOrWhiteSpace(item.LocationBusinessKey))
                {
                    locationIds.Add(item.LocationBusinessKey);
                }
            }

            var returnedPageSize = result.Data?.PageSize ?? pageSize;
            var totalPages = CalculateTotalPages(result.Data?.TotalCount ?? locationIds.Count, returnedPageSize);
            if (page >= totalPages || items.Count < returnedPageSize)
            {
                break;
            }

            page++;
        }

        return locationIds;
    }

    private async Task<Dictionary<string, int>> BuildSerialCountLookupAsync(
        string token,
        IEnumerable<string> variantIds,
        string? warehouseId,
        IReadOnlyCollection<string> selectedLocationIds,
        CancellationToken cancellationToken)
    {
        var distinctVariantIds = variantIds
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Select(x => x.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        if (distinctVariantIds.Count == 0)
        {
            return new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        }

        var tasks = distinctVariantIds.Select(async variantId =>
        {
            cancellationToken.ThrowIfCancellationRequested();

            var result = await _apiService.SearchSerialItemsAsync(token, variantId, warehouseId, status: "Available");
            if (!result.IsSuccess)
            {
                return (VariantId: variantId, Count: 0);
            }

            var serials = (result.Data ?? new List<SerialItemLookupModel>())
                .Where(x => selectedLocationIds.Count == 0 || selectedLocationIds.Contains(x.LocationRef))
                .ToList();

            return (VariantId: variantId, Count: serials.Count);
        });

        var results = await Task.WhenAll(tasks);
        return results.ToDictionary(x => x.VariantId, x => x.Count, StringComparer.OrdinalIgnoreCase);
    }

    private static string GetVariantLabel(string variantId, IReadOnlyDictionary<string, ProductVariantSummaryModel> variantLookup)
    {
        if (variantLookup.TryGetValue(variantId, out var variant))
        {
            if (string.IsNullOrWhiteSpace(variant.Sku))
            {
                return string.IsNullOrWhiteSpace(variant.Name) ? variantId : variant.Name;
            }

            return string.IsNullOrWhiteSpace(variant.Name) ? variant.Sku : $"{variant.Sku} - {variant.Name}";
        }

        return variantId;
    }

    private static int NormalizePageSize(int pageSize)
    {
        return new[] { 10, 25, 50 }.Contains(pageSize) ? pageSize : 10;
    }

    private static int CalculateTotalPages(int totalCount, int pageSize)
    {
        return Math.Max(1, (int)Math.Ceiling(totalCount / (double)Math.Max(pageSize, 1)));
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

    private bool TryGetToken(out string token)
    {
        token = HttpContext.Session.GetString("Token") ?? string.Empty;
        return !string.IsNullOrWhiteSpace(token);
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
            return (JsonSerializer.Deserialize<List<string>>(permissionsJson) ?? new List<string>())
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(x => x.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();
        }
        catch
        {
            return Array.Empty<string>();
        }
    }

    private static string JoinErrors(params string?[] errors)
    {
        var clean = errors.Where(x => !string.IsNullOrWhiteSpace(x)).Distinct().ToList();
        return clean.Count == 0 ? string.Empty : string.Join(" | ", clean);
    }

    private bool IsAuthorizedFor(string token, params string[] aliases)
    {
        var roles = ResolveRolesFromSession(token);
        if (roles.Any(x => string.Equals(x, "SysAdmin", StringComparison.OrdinalIgnoreCase) || string.Equals(x, "Admin", StringComparison.OrdinalIgnoreCase)))
        {
            return true;
        }

        var permissions = ResolvePermissionsFromSession();
        if (permissions.Count == 0 || permissions.Any(x => string.Equals(x, "*", StringComparison.OrdinalIgnoreCase)))
        {
            return true;
        }

        return aliases.Any(alias => permissions.Any(permission => string.Equals(permission, alias, StringComparison.OrdinalIgnoreCase)));
    }
}
