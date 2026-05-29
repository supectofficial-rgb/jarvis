using System.Text.Json;
using Insurance.InventoryDashboard.Panel.Models;
using Insurance.InventoryDashboard.Panel.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Insurance.InventoryDashboard.Panel.Controllers;

public sealed partial class InventoryManagementController : Controller
{
    private static readonly int[] PageSizeOptions = [10, 25, 50];

    private readonly IApiService _apiService;
    private readonly IDashboardConfigService _dashboardConfigService;

    public InventoryManagementController(IApiService apiService, IDashboardConfigService dashboardConfigService)
    {
        _apiService = apiService;
        _dashboardConfigService = dashboardConfigService;
    }

    [HttpGet]
    public IActionResult Index() => RedirectToAction(nameof(Warehouses));

    [HttpGet]
    public Task<IActionResult> Locations(
        string? warehouseId,
        string? locationId,
        string? warehouseCode,
        string? warehouseName,
        string? warehouseStatus,
        string? locationCode,
        string[]? locationTypes,
        string? locationAisle,
        string? locationRack,
        string? locationShelf,
        string? locationBin,
        string? locationStatus,
        string? tab,
        int warehousePage = 1,
        int warehousePageSize = 10,
        int locationPage = 1,
        int locationPageSize = 10,
        CancellationToken cancellationToken = default)
        => BuildInventoryManagementPageAsync(
            warehouseId,
            locationId,
            warehouseCode,
            warehouseName,
            warehouseStatus,
            locationCode,
            locationTypes,
            locationAisle,
            locationRack,
            locationShelf,
            locationBin,
            locationStatus,
            "locations",
            includeWarehouseDetails: false,
            includeLocations: true,
            warehousePage,
            warehousePageSize,
            locationPage,
            locationPageSize,
            tab,
            cancellationToken);

    [HttpGet]
    public async Task<IActionResult> LocationList(
        string? warehouseId,
        string? locationCode,
        string[]? locationTypes,
        string? structureSelectionsJson,
        string? locationAisle,
        string? locationRack,
        string? locationShelf,
        string? locationBin,
        string? locationStatus,
        int locationPage = 1,
        int locationPageSize = 10,
        CancellationToken cancellationToken = default)
    {
        if (!TryGetToken(out var token))
        {
            return Unauthorized(new { isSuccess = false, errorMessage = "نشست کاربر در دسترس نیست." });
        }

        if (!IsAuthorizedFor(token, "Inventory.Warehouse.View", "Warehouse.Read", "Warehouse.Search"))
        {
            return StatusCode(403, new { isSuccess = false, errorMessage = "شما دسترسی مشاهده لوکیشن‌ها را ندارید." });
        }

        locationPageSize = NormalizePageSize(locationPageSize);
        var normalizedLocationTypes = NormalizeLocationTypes(locationTypes);
        var locationStatusValue = ParseStatus(locationStatus);
        var selectedWarehouseId = string.IsNullOrWhiteSpace(warehouseId) ? null : warehouseId.Trim();

        var lookupResult = await _apiService.GetWarehouseLookupAsync(token, includeInactive: true);
        var warehouseLookup = lookupResult.Data ?? new List<WarehouseLookupItemModel>();
        var selectedWarehouseName = warehouseLookup.FirstOrDefault(x => string.Equals(x.WarehouseBusinessKey, selectedWarehouseId, StringComparison.OrdinalIgnoreCase)) is { } warehouse
            ? $"{warehouse.Code} - {warehouse.Name}"
            : null;

        if (string.IsNullOrWhiteSpace(selectedWarehouseId))
        {
            return Json(new LocationListResponseModel
            {
                SelectedWarehouseId = null,
                SelectedWarehouseName = null,
                Page = Math.Max(locationPage, 1),
                PageSize = locationPageSize,
                TotalCount = 0,
                TotalPages = 1,
                Items = new List<LocationListItemModel>()
            });
        }

        var locationsResult = await _apiService.SearchLocationsAsync(
            token,
            selectedWarehouseId,
            locationCode,
            normalizedLocationTypes,
            locationAisle,
            locationRack,
            locationShelf,
            locationBin,
            locationStatusValue,
            Math.Max(locationPage, 1),
            locationPageSize,
            structureSelectionsJson);

        var locations = locationsResult.Data?.Items ?? new List<LocationListItemModel>();
        var totalCount = locationsResult.Data?.TotalCount ?? locations.Count;
        var pageSize = locationsResult.Data?.PageSize ?? locationPageSize;

        return Json(new LocationListResponseModel
        {
            SelectedWarehouseId = selectedWarehouseId,
            SelectedWarehouseName = selectedWarehouseName,
            Page = locationsResult.Data?.Page ?? Math.Max(locationPage, 1),
            PageSize = pageSize,
            TotalCount = totalCount,
            TotalPages = CalculateTotalPages(totalCount, pageSize),
            Items = locations
        });
    }

    [HttpGet]
    public Task<IActionResult> Warehouses(
        string? warehouseId,
        string? locationId,
        string? warehouseCode,
        string? warehouseName,
        string? warehouseStatus,
        string? locationCode,
        string? locationType,
        string? locationAisle,
        string? locationRack,
        string? locationShelf,
        string? locationBin,
        string? locationStatus,
        string? activeItem,
        int warehousePage = 1,
        int warehousePageSize = 10,
        int locationPage = 1,
        int locationPageSize = 10,
        string? tab = null,
        CancellationToken cancellationToken = default)
        => BuildInventoryManagementPageAsync(
            warehouseId,
            locationId,
            warehouseCode,
            warehouseName,
            warehouseStatus,
            locationCode,
            string.IsNullOrWhiteSpace(locationType) ? null : new[] { locationType },
            locationAisle,
            locationRack,
            locationShelf,
            locationBin,
            locationStatus,
            string.IsNullOrWhiteSpace(activeItem) ? "warehouses" : activeItem,
            includeWarehouseDetails: true,
            includeLocations: false,
            warehousePage,
            warehousePageSize,
            locationPage,
            locationPageSize,
            tab,
            cancellationToken);

    private async Task<IActionResult> BuildInventoryManagementPageAsync(
        string? warehouseId,
        string? locationId,
        string? warehouseCode,
        string? warehouseName,
        string? warehouseStatus,
        string? locationCode,
        string[]? locationTypes,
        string? locationAisle,
        string? locationRack,
        string? locationShelf,
        string? locationBin,
        string? locationStatus,
        string activeItem,
        bool includeWarehouseDetails,
        bool includeLocations,
        int warehousePage,
        int warehousePageSize,
        int locationPage,
        int locationPageSize,
        string? tab,
        CancellationToken cancellationToken)
    {
        if (!TryGetToken(out var token))
        {
            return RedirectToAction("Login", "Auth");
        }

        var roles = ResolveRolesFromSession(token);
        if (!IsAuthorizedFor(token, "Inventory.Warehouse.View", "Warehouse.Read", "Warehouse.Search"))
        {
            TempData["CatalogError"] = "شما دسترسی مشاهده مدیریت انبار را ندارید.";
            return RedirectToAction("Index", "Dashboard");
        }

        var modules = await _dashboardConfigService.GetMenuByRolesAsync(roles, cancellationToken);
        var menu = ResolveMenu(
            modules,
            "inventory_management",
            string.Equals(activeItem, "locations", StringComparison.OrdinalIgnoreCase) ? "locations" : "warehouses");
        var isLocationsPage = includeLocations;

        warehousePageSize = NormalizePageSize(warehousePageSize);
        locationPageSize = NormalizePageSize(locationPageSize);

        var warehouseStatusValue = ParseStatus(warehouseStatus);
        var locationStatusValue = ParseStatus(locationStatus);
        var normalizedLocationTypes = NormalizeLocationTypes(locationTypes);

        var warehousesResult = await _apiService.SearchWarehousesAsync(
            token,
            warehouseCode,
            warehouseName,
            warehouseStatusValue,
            Math.Max(warehousePage, 1),
            warehousePageSize);
        var warehouses = warehousesResult.Data?.Items ?? new List<WarehouseListItemModel>();

        var lookupResult = await _apiService.GetWarehouseLookupAsync(token, includeInactive: true);
        var warehouseLookup = lookupResult.Data ?? new List<WarehouseLookupItemModel>();

        var selectedWarehouseId = ResolveSelectedWarehouseId(warehouseId, warehouses, warehouseLookup);

        var locationsResult = includeLocations && !string.IsNullOrWhiteSpace(selectedWarehouseId)
            ? await _apiService.SearchLocationsAsync(
                token,
                selectedWarehouseId,
                locationCode,
                normalizedLocationTypes,
                locationAisle,
                locationRack,
                locationShelf,
                locationBin,
                locationStatusValue,
                Math.Max(locationPage, 1),
                locationPageSize)
            : new ApiResponse<LocationSearchResultModel>
            {
                IsSuccess = true,
                Data = new LocationSearchResultModel
                {
                    Items = new List<LocationListItemModel>(),
                    Page = Math.Max(locationPage, 1),
                    PageSize = locationPageSize,
                    TotalCount = 0
                }
            };
        var locations = locationsResult.Data?.Items ?? new List<LocationListItemModel>();

        var selectedLocationId = ResolveSelectedLocationId(locationId, locations);
        var selectedLocation = locations.FirstOrDefault(x => string.Equals(x.LocationBusinessKey, selectedLocationId, StringComparison.OrdinalIgnoreCase));
        var selectedLocationDetailResult = !string.IsNullOrWhiteSpace(selectedLocationId)
            ? await _apiService.GetLocationByBusinessKeyAsync(selectedLocationId, token)
            : null;
        var selectedLocationDetail = selectedLocationDetailResult?.Data;

        var selectedWarehouseDetailsResult = includeWarehouseDetails && !string.IsNullOrWhiteSpace(selectedWarehouseId)
            ? await _apiService.GetWarehouseWithLocationsAsync(selectedWarehouseId, token, includeInactiveLocations: true)
            : new ApiResponse<WarehouseWithLocationsModel> { IsSuccess = true };

        var selectedWarehouse = warehouses.FirstOrDefault(x => string.Equals(x.WarehouseBusinessKey, selectedWarehouseId, StringComparison.OrdinalIgnoreCase));
        var selectedWarehouseDetails = selectedWarehouseDetailsResult.Data;

        var model = new WarehouseManagementPageViewModel
        {
            UserName = HttpContext.Session.GetString("UserName") ?? "کاربر",
            Roles = roles,
            Permissions = ResolvePermissionsFromSession(),
            Modules = modules,
            ActiveModule = menu.Module,
            ActiveItem = menu.Item,

            SelectedWarehouseId = selectedWarehouseId,
            SelectedLocationId = selectedLocationId,
            LocationTab = tab,
            Warehouses = warehouses,
            WarehouseLookup = warehouseLookup,
            Locations = locations,
            SelectedWarehouseDetails = includeWarehouseDetails ? selectedWarehouseDetails : null,

            WarehouseCodeFilter = warehouseCode,
            WarehouseNameFilter = warehouseName,
            WarehouseStatusFilter = warehouseStatus,
            WarehousePage = warehousesResult.Data?.Page ?? Math.Max(warehousePage, 1),
            WarehousePageSize = warehousesResult.Data?.PageSize ?? warehousePageSize,
            WarehouseTotalCount = warehousesResult.Data?.TotalCount ?? warehouses.Count,
            WarehouseTotalPages = CalculateTotalPages(warehousesResult.Data?.TotalCount ?? warehouses.Count, warehousesResult.Data?.PageSize ?? warehousePageSize),

            LocationCodeFilter = locationCode,
            LocationTypesFilter = normalizedLocationTypes,
            LocationAisleFilter = locationAisle,
            LocationRackFilter = locationRack,
            LocationShelfFilter = locationShelf,
            LocationBinFilter = locationBin,
            LocationStatusFilter = locationStatus,
            LocationPage = locationsResult.Data?.Page ?? Math.Max(locationPage, 1),
            LocationPageSize = locationsResult.Data?.PageSize ?? locationPageSize,
            LocationTotalCount = locationsResult.Data?.TotalCount ?? locations.Count,
            LocationTotalPages = CalculateTotalPages(locationsResult.Data?.TotalCount ?? locations.Count, locationsResult.Data?.PageSize ?? locationPageSize),
            PageSizeOptions = PageSizeOptions,

            ErrorMessage = JoinErrors(
                warehousesResult.ErrorMessage,
                lookupResult.ErrorMessage,
                locationsResult.ErrorMessage,
                selectedWarehouseDetailsResult.ErrorMessage),

            WarehouseForm = new WarehouseForm
            {
                WarehouseId = selectedWarehouseId,
                Code = selectedWarehouseDetails?.Code ?? selectedWarehouse?.Code ?? string.Empty,
                Name = selectedWarehouseDetails?.Name ?? selectedWarehouse?.Name ?? string.Empty,
                IsActive = selectedWarehouseDetails?.IsActive ?? selectedWarehouse?.IsActive ?? true
            },
            LocationForm = new LocationForm
            {
                LocationId = selectedLocationDetail?.LocationBusinessKey ?? selectedLocation?.LocationBusinessKey,
                WarehouseId = selectedLocationDetail?.WarehouseId ?? selectedLocation?.WarehouseRef ?? selectedWarehouseId ?? string.Empty,
                LocationCode = selectedLocationDetail?.LocationCode ?? selectedLocation?.LocationCode ?? string.Empty,
                LocationType = NormalizeLocationType(selectedLocationDetail?.LocationType ?? selectedLocation?.LocationType) ?? "Bulk",
                StructureSelections = selectedLocationDetail?.StructureSelections?
                    .Select(x => new LocationStructureSelectionForm
                    {
                        StructureRef = x.StructureRef.ToString(),
                        StructureValueRef = x.StructureValueRef.ToString()
                    })
                    .ToList() ?? new List<LocationStructureSelectionForm>(),
                IsActive = selectedLocationDetail?.IsActive ?? selectedLocation?.IsActive ?? true
            }
        };

        SetLayoutViewBag(model.Modules, model.ActiveModule?.ModuleId, model.ActiveItem?.ItemId, model.UserName);
        var viewPath = includeLocations
            ? "~/Views/InventoryManagement/Locations.cshtml"
            : "~/Views/InventoryManagement/Warehouses.cshtml";
        return View(viewPath, model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SaveWarehouse([Bind(Prefix = "WarehouseForm")] WarehouseForm form)
    {
        if (!TryGetToken(out var token))
        {
            return RedirectToAction("Login", "Auth");
        }

        if (!IsAuthorizedFor(token, "Inventory.Warehouse.Create", "Inventory.Warehouse.Update", "Warehouse.Create", "Warehouse.Update"))
        {
            TempData["CatalogError"] = "شما دسترسی ذخیره انبار را ندارید.";
            return RedirectToAction(nameof(Warehouses), new { warehouseId = form.WarehouseId });
        }

        if (!TryValidateModel(form))
        {
            TempData["CatalogError"] = ExtractModelError(ModelState);
            return RedirectToAction(nameof(Warehouses), new { warehouseId = form.WarehouseId });
        }

        var request = new UpsertWarehouseRequest
        {
            Code = form.Code,
            Name = form.Name,
            IsActive = form.IsActive
        };

        var result = string.IsNullOrWhiteSpace(form.WarehouseId)
            ? await _apiService.CreateWarehouseAsync(request, token)
            : await _apiService.UpdateWarehouseAsync(form.WarehouseId, request, token);

        TempData[result.IsSuccess ? "CatalogSuccess" : "CatalogError"] =
            result.IsSuccess ? "انبار با موفقیت ذخیره شد." : result.ErrorMessage ?? "ذخیره انبار با خطا مواجه شد.";

        if (string.IsNullOrWhiteSpace(form.WarehouseId))
        {
            return RedirectToAction(nameof(Warehouses), new { warehouseCode = form.Code });
        }

        return RedirectToAction(nameof(Warehouses), new { warehouseId = form.WarehouseId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SaveLocation([Bind(Prefix = "LocationForm")] LocationForm form)
    {
        if (!TryGetToken(out var token))
        {
            return RedirectToAction("Login", "Auth");
        }

        if (!IsAuthorizedFor(token, "Inventory.Location.Create", "Inventory.Location.Update", "Location.Create", "Location.Update"))
        {
            TempData["CatalogError"] = "شما دسترسی ذخیره لوکیشن را ندارید.";
            return RedirectToAction(nameof(Locations), new { warehouseId = form.WarehouseId, locationId = form.LocationId, tab = "create" });
        }

        if (!TryValidateModel(form))
        {
            TempData["CatalogError"] = ExtractModelError(ModelState);
            return RedirectToAction(nameof(Locations), new { warehouseId = form.WarehouseId, locationId = form.LocationId, tab = "create" });
        }

        if (!Guid.TryParse(form.WarehouseId, out _))
        {
            TempData["CatalogError"] = "انبار انتخاب شده معتبر نیست.";
            return RedirectToAction(nameof(Locations), new { warehouseId = form.WarehouseId, locationId = form.LocationId, tab = "create" });
        }

        var request = new UpsertLocationRequest
        {
            WarehouseId = form.WarehouseId,
            LocationCode = form.LocationCode,
            LocationType = NormalizeLocationType(form.LocationType) ?? form.LocationType,
            StructureSelections = form.StructureSelections
                .Where(x => Guid.TryParse(x.StructureRef, out _) && Guid.TryParse(x.StructureValueRef, out _))
                .Select(x => new LocationStructureSelectionForm
                {
                    StructureRef = x.StructureRef.Trim(),
                    StructureValueRef = x.StructureValueRef.Trim()
                })
                .ToList(),
            IsActive = form.IsActive
        };

        var result = string.IsNullOrWhiteSpace(form.LocationId)
            ? await _apiService.CreateLocationAsync(request, token)
            : await _apiService.UpdateLocationAsync(form.LocationId, request, token);

        TempData[result.IsSuccess ? "CatalogSuccess" : "CatalogError"] =
            result.IsSuccess ? "لوکیشن با موفقیت ذخیره شد." : result.ErrorMessage ?? "ذخیره لوکیشن با خطا مواجه شد.";

        if (string.IsNullOrWhiteSpace(form.LocationId))
        {
            return RedirectToAction(nameof(Locations), new { warehouseId = form.WarehouseId, tab = "create" });
        }

        return RedirectToAction(nameof(Locations), new { warehouseId = form.WarehouseId, locationId = form.LocationId, tab = "create" });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public Task<IActionResult> ActivateWarehouse(string warehouseId) =>
        ChangeWarehouseStatus(warehouseId, activate: true);

    [HttpPost]
    [ValidateAntiForgeryToken]
    public Task<IActionResult> DeactivateWarehouse(string warehouseId) =>
        ChangeWarehouseStatus(warehouseId, activate: false);

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteWarehouse(string warehouseId)
    {
        if (!TryGetToken(out var token))
        {
            return RedirectToAction("Login", "Auth");
        }

        if (!IsAuthorizedFor(token, "Inventory.Warehouse.Delete", "Warehouse.Delete"))
        {
            TempData["CatalogError"] = "شما دسترسی حذف انبار را ندارید.";
            return RedirectToAction(nameof(Warehouses), new { warehouseId });
        }

        var result = await _apiService.DeleteWarehouseAsync(warehouseId, token);
        TempData[result.IsSuccess ? "CatalogSuccess" : "CatalogError"] =
            result.IsSuccess ? "انبار حذف شد." : result.ErrorMessage ?? "حذف انبار با خطا مواجه شد.";
        return RedirectToAction(nameof(Warehouses));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public Task<IActionResult> ActivateLocation(
        string warehouseId,
        string locationId,
        string? locationCode = null,
        string[]? locationTypes = null,
        string? locationAisle = null,
        string? locationRack = null,
        string? locationShelf = null,
        string? locationBin = null,
        string? locationStatus = null) =>
        ChangeLocationStatus(warehouseId, locationId, activate: true, locationCode, locationTypes, locationAisle, locationRack, locationShelf, locationBin, locationStatus);

    [HttpPost]
    [ValidateAntiForgeryToken]
    public Task<IActionResult> DeactivateLocation(
        string warehouseId,
        string locationId,
        string? locationCode = null,
        string[]? locationTypes = null,
        string? locationAisle = null,
        string? locationRack = null,
        string? locationShelf = null,
        string? locationBin = null,
        string? locationStatus = null) =>
        ChangeLocationStatus(warehouseId, locationId, activate: false, locationCode, locationTypes, locationAisle, locationRack, locationShelf, locationBin, locationStatus);

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteLocation(
        string warehouseId,
        string locationId,
        string? locationCode = null,
        string[]? locationTypes = null,
        string? locationAisle = null,
        string? locationRack = null,
        string? locationShelf = null,
        string? locationBin = null,
        string? locationStatus = null)
    {
        if (!TryGetToken(out var token))
        {
            return RedirectToAction("Login", "Auth");
        }

        if (!IsAuthorizedFor(token, "Inventory.Location.Delete", "Location.Delete"))
        {
            TempData["CatalogError"] = "شما دسترسی حذف لوکیشن را ندارید.";
            return RedirectToAction(nameof(Warehouses), new { warehouseId });
        }

        var result = await _apiService.DeleteLocationAsync(locationId, token);
        TempData[result.IsSuccess ? "CatalogSuccess" : "CatalogError"] =
            result.IsSuccess ? "لوکیشن حذف شد." : result.ErrorMessage ?? "حذف لوکیشن با خطا مواجه شد.";
        if (IsAjaxRequest())
        {
            return Json(new
            {
                isSuccess = result.IsSuccess,
                errorMessage = result.IsSuccess ? (string?)null : result.ErrorMessage ?? "Ø­Ø°Ù Ù„ÙˆÚ©ÛŒØ´Ù† Ø¨Ø§ Ø®Ø·Ø§ Ù…ÙˆØ§Ø¬Ù‡ Ø´Ø¯."
            });
        }

        return RedirectToAction(nameof(Locations), new { warehouseId, locationCode, locationTypes, locationAisle, locationRack, locationShelf, locationBin, locationStatus });
    }

    private async Task<IActionResult> ChangeLocationStatus(
        string warehouseId,
        string locationId,
        bool activate,
        string? locationCode = null,
        string[]? locationTypes = null,
        string? locationAisle = null,
        string? locationRack = null,
        string? locationShelf = null,
        string? locationBin = null,
        string? locationStatus = null)
    {
        if (!TryGetToken(out var token))
        {
            return RedirectToAction("Login", "Auth");
        }

        var permission = activate
            ? new[] { "Inventory.Location.Activate", "Location.Activate" }
            : new[] { "Inventory.Location.Deactivate", "Location.Deactivate" };

        if (!IsAuthorizedFor(token, permission))
        {
            TempData["CatalogError"] = "شما دسترسی تغییر وضعیت انبار را ندارید.";
            return RedirectToAction(nameof(Warehouses), new { warehouseId });
        }

        var result = activate
            ? await _apiService.ActivateLocationAsync(locationId, token)
            : await _apiService.DeactivateLocationAsync(locationId, token);

        TempData[result.IsSuccess ? "CatalogSuccess" : "CatalogError"] =
            result.IsSuccess ? "وضعیت انبار تغییر کرد." : result.ErrorMessage ?? "تغییر وضعیت انبار با خطا مواجه شد.";
        if (IsAjaxRequest())
        {
            return Json(new
            {
                isSuccess = result.IsSuccess,
                errorMessage = result.IsSuccess ? (string?)null : result.ErrorMessage ?? "ØªØºÛŒÛŒØ± ÙˆØ¶Ø¹ÛŒØª Ù„ÙˆÚ©ÛŒØ´Ù† Ø¨Ø§ Ø®Ø·Ø§ Ù…ÙˆØ§Ø¬Ù‡ Ø´Ø¯."
            });
        }

        return RedirectToAction(nameof(Locations), new { warehouseId, locationId, locationCode, locationTypes, locationAisle, locationRack, locationShelf, locationBin, locationStatus, tab = "list" });
    }

    private async Task<IActionResult> ChangeWarehouseStatus(string warehouseId, bool activate)
    {
        if (!TryGetToken(out var token))
        {
            return RedirectToAction("Login", "Auth");
        }

        var permission = activate
            ? new[] { "Inventory.Warehouse.Activate", "Warehouse.Activate" }
            : new[] { "Inventory.Warehouse.Deactivate", "Warehouse.Deactivate" };

        if (!IsAuthorizedFor(token, permission))
        {
            TempData["CatalogError"] = "Ø´Ù…Ø§ Ø¯Ø³ØªØ±Ø³ÛŒ ØªØºÛŒÛŒØ± ÙˆØ¶Ø¹ÛŒØª Ù„ÙˆÚ©ÛŒØ´Ù† Ø±Ø§ Ù†Ø¯Ø§Ø±ÛŒØ¯.";
            return RedirectToAction(nameof(Warehouses), new { warehouseId });
        }

        var result = activate
            ? await _apiService.ActivateWarehouseAsync(warehouseId, token)
            : await _apiService.DeactivateWarehouseAsync(warehouseId, token);

        TempData[result.IsSuccess ? "CatalogSuccess" : "CatalogError"] =
            result.IsSuccess ? "ÙˆØ¶Ø¹ÛŒØª Ù„ÙˆÚ©ÛŒØ´Ù† ØªØºÛŒÛŒØ± Ú©Ø±Ø¯." : result.ErrorMessage ?? "ØªØºÛŒÛŒØ± ÙˆØ¶Ø¹ÛŒØª Ù„ÙˆÚ©ÛŒØ´Ù† Ø¨Ø§ Ø®Ø·Ø§ Ù…ÙˆØ§Ø¬Ù‡ Ø´Ø¯.";
        return RedirectToAction(nameof(Warehouses), new { warehouseId });
    }

    private static string? NormalizeLocationType(string? locationType)
    {
        if (string.IsNullOrWhiteSpace(locationType))
        {
            return null;
        }

        return locationType.Trim().ToLowerInvariant() switch
        {
            "pick" or "picking" => "Pick",
            "bulk" or "storage" => "Bulk",
            "return" or "returns" => "Return",
            "damage" or "damaged" => "Damage",
            "quarantine" => "Quarantine",
            _ => locationType.Trim()
        };
    }

    private static IReadOnlyList<string> NormalizeLocationTypes(IEnumerable<string>? locationTypes)
    {
        if (locationTypes is null)
        {
            return Array.Empty<string>();
        }

        return locationTypes
            .Select(NormalizeLocationType)
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Select(x => x!)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    private async Task<IActionResult> ChangeLocationStatus(string warehouseId, string locationId, bool activate)
    {
        if (!TryGetToken(out var token))
        {
            return RedirectToAction("Login", "Auth");
        }

        var permission = activate
            ? new[] { "Inventory.Location.Activate", "Location.Activate" }
            : new[] { "Inventory.Location.Deactivate", "Location.Deactivate" };

        if (!IsAuthorizedFor(token, permission))
        {
            TempData["CatalogError"] = "شما دسترسی تغییر وضعیت لوکیشن را ندارید.";
            return RedirectToAction(nameof(Locations), new { warehouseId, locationId, tab = "list" });
        }

        var result = activate
            ? await _apiService.ActivateLocationAsync(locationId, token)
            : await _apiService.DeactivateLocationAsync(locationId, token);

        TempData[result.IsSuccess ? "CatalogSuccess" : "CatalogError"] =
            result.IsSuccess ? "وضعیت لوکیشن تغییر کرد." : result.ErrorMessage ?? "تغییر وضعیت لوکیشن با خطا مواجه شد.";
        return RedirectToAction(nameof(Locations), new { warehouseId, locationId, tab = "list" });
    }

    private static string? ResolveSelectedWarehouseId(
        string? warehouseId,
        IReadOnlyList<WarehouseListItemModel> warehouses,
        IReadOnlyList<WarehouseLookupItemModel> lookup)
    {
        if (!string.IsNullOrWhiteSpace(warehouseId))
        {
            return warehouseId;
        }

        return null;
    }

    private static string? ResolveSelectedLocationId(string? locationId, IReadOnlyList<LocationListItemModel> locations)
    {
        if (!string.IsNullOrWhiteSpace(locationId))
        {
            return locationId;
        }

        return null;
    }

    private static bool? ParseStatus(string? status)
    {
        return status?.Trim().ToLowerInvariant() switch
        {
            "active" => true,
            "inactive" => false,
            _ => null
        };
    }

    private static int NormalizePageSize(int pageSize)
    {
        return PageSizeOptions.Contains(pageSize) ? pageSize : 10;
    }

    private static int CalculateTotalPages(int totalCount, int pageSize)
    {
        return Math.Max(1, (int)Math.Ceiling(totalCount / (double)Math.Max(pageSize, 1)));
    }

    private bool IsAjaxRequest()
    {
        return string.Equals(Request.Headers["X-Requested-With"], "XMLHttpRequest", StringComparison.OrdinalIgnoreCase)
            || Request.Headers.Accept.Any(value => value.Contains("application/json", StringComparison.OrdinalIgnoreCase));
    }

    private static (DashboardMenuModule? Module, DashboardMenuItem? Item) ResolveMenu(
        IReadOnlyList<DashboardMenuModule> modules,
        string moduleId,
        string itemId)
    {
        var module = modules.FirstOrDefault(m => string.Equals(m.ModuleId, moduleId, StringComparison.OrdinalIgnoreCase));
        var item = FindMenuItem(module?.Items ?? new List<DashboardMenuItem>(), itemId);
        return (module, item);
    }

    private static DashboardMenuItem? FindMenuItem(IEnumerable<DashboardMenuItem> items, string itemId)
    {
        foreach (var item in items)
        {
            if (string.Equals(item.ItemId, itemId, StringComparison.OrdinalIgnoreCase))
            {
                return item;
            }

            var childMatch = FindMenuItem(item.Children, itemId);
            if (childMatch is not null)
            {
                return childMatch;
            }
        }

        return null;
    }

    private static string? JoinErrors(params string?[] errors)
    {
        var clean = errors.Where(x => !string.IsNullOrWhiteSpace(x)).Distinct().ToList();
        return clean.Count == 0 ? null : string.Join(" | ", clean);
    }

    private static string ExtractModelError(ModelStateDictionary modelState)
    {
        return modelState.Values
            .SelectMany(v => v.Errors)
            .Select(e => string.IsNullOrWhiteSpace(e.ErrorMessage) ? e.Exception?.Message : e.ErrorMessage)
            .FirstOrDefault(message => !string.IsNullOrWhiteSpace(message))
            ?? "ورودی نامعتبر است.";
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
                    return roles.Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => x.Trim()).Distinct(StringComparer.OrdinalIgnoreCase).ToList();
                }
            }
            catch
            {
                // Ignore malformed session value.
            }
        }

        var extractedRoles = JwtRoleExtractor.ExtractRoles(token)
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Select(x => x.Trim())
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
