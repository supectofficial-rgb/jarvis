using System.Text.Json;
using Insurance.InventoryDashboard.Panel.Models;
using Insurance.InventoryDashboard.Panel.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Insurance.InventoryDashboard.Panel.Controllers;

public sealed class InventoryManagementController : Controller
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
    public async Task<IActionResult> Warehouses(
        string? warehouseId,
        string? locationId,
        string? warehouseCode,
        string? warehouseName,
        string? warehouseStatus,
        string? locationCode,
        string? locationType,
        string? locationStatus,
        string? activeItem,
        int warehousePage = 1,
        int warehousePageSize = 10,
        int locationPage = 1,
        int locationPageSize = 10,
        CancellationToken cancellationToken = default)
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

        warehousePageSize = NormalizePageSize(warehousePageSize);
        locationPageSize = NormalizePageSize(locationPageSize);

        var warehouseStatusValue = ParseStatus(warehouseStatus);
        var locationStatusValue = ParseStatus(locationStatus);

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

        var locationsResult = await _apiService.SearchLocationsAsync(
            token,
            selectedWarehouseId,
            locationCode,
            locationType,
            locationStatusValue,
            Math.Max(locationPage, 1),
            locationPageSize);
        var locations = locationsResult.Data?.Items ?? new List<LocationListItemModel>();

        var selectedLocationId = ResolveSelectedLocationId(locationId, locations);
        var selectedLocation = locations.FirstOrDefault(x => string.Equals(x.LocationBusinessKey, selectedLocationId, StringComparison.OrdinalIgnoreCase));

        var selectedWarehouseDetailsResult = !string.IsNullOrWhiteSpace(selectedWarehouseId)
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
            Warehouses = warehouses,
            WarehouseLookup = warehouseLookup,
            Locations = locations,
            SelectedWarehouseDetails = selectedWarehouseDetails,

            WarehouseCodeFilter = warehouseCode,
            WarehouseNameFilter = warehouseName,
            WarehouseStatusFilter = warehouseStatus,
            WarehousePage = warehousesResult.Data?.Page ?? Math.Max(warehousePage, 1),
            WarehousePageSize = warehousesResult.Data?.PageSize ?? warehousePageSize,
            WarehouseTotalCount = warehousesResult.Data?.TotalCount ?? warehouses.Count,
            WarehouseTotalPages = CalculateTotalPages(warehousesResult.Data?.TotalCount ?? warehouses.Count, warehousesResult.Data?.PageSize ?? warehousePageSize),

            LocationCodeFilter = locationCode,
            LocationTypeFilter = locationType,
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
                LocationId = selectedLocation?.LocationBusinessKey,
                WarehouseId = selectedLocation?.WarehouseRef ?? selectedWarehouseId ?? string.Empty,
                LocationCode = selectedLocation?.LocationCode ?? string.Empty,
                LocationType = selectedLocation?.LocationType ?? "Storage",
                Aisle = selectedLocation?.Aisle,
                Rack = selectedLocation?.Rack,
                Shelf = selectedLocation?.Shelf,
                Bin = selectedLocation?.Bin,
                IsActive = selectedLocation?.IsActive ?? true
            }
        };

        SetLayoutViewBag(model.Modules, model.ActiveModule?.ModuleId, model.ActiveItem?.ItemId, model.UserName);
        return View("~/Views/InventoryManagement/Warehouses.cshtml", model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SaveWarehouse(WarehouseForm form)
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

        return RedirectToAction(nameof(Warehouses), new { warehouseId = form.WarehouseId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SaveLocation(LocationForm form)
    {
        if (!TryGetToken(out var token))
        {
            return RedirectToAction("Login", "Auth");
        }

        if (!IsAuthorizedFor(token, "Inventory.Location.Create", "Inventory.Location.Update", "Location.Create", "Location.Update"))
        {
            TempData["CatalogError"] = "شما دسترسی ذخیره لوکیشن را ندارید.";
            return RedirectToAction(nameof(Warehouses), new { warehouseId = form.WarehouseId, locationId = form.LocationId });
        }

        if (!TryValidateModel(form))
        {
            TempData["CatalogError"] = ExtractModelError(ModelState);
            return RedirectToAction(nameof(Warehouses), new { warehouseId = form.WarehouseId, locationId = form.LocationId });
        }

        var request = new UpsertLocationRequest
        {
            WarehouseId = form.WarehouseId,
            LocationCode = form.LocationCode,
            LocationType = form.LocationType,
            Aisle = form.Aisle,
            Rack = form.Rack,
            Shelf = form.Shelf,
            Bin = form.Bin,
            IsActive = form.IsActive
        };

        var result = string.IsNullOrWhiteSpace(form.LocationId)
            ? await _apiService.CreateLocationAsync(request, token)
            : await _apiService.UpdateLocationAsync(form.LocationId, request, token);

        TempData[result.IsSuccess ? "CatalogSuccess" : "CatalogError"] =
            result.IsSuccess ? "لوکیشن با موفقیت ذخیره شد." : result.ErrorMessage ?? "ذخیره لوکیشن با خطا مواجه شد.";

        return RedirectToAction(nameof(Warehouses), new { warehouseId = form.WarehouseId, locationId = form.LocationId });
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
    public Task<IActionResult> ActivateLocation(string warehouseId, string locationId) =>
        ChangeLocationStatus(warehouseId, locationId, activate: true);

    [HttpPost]
    [ValidateAntiForgeryToken]
    public Task<IActionResult> DeactivateLocation(string warehouseId, string locationId) =>
        ChangeLocationStatus(warehouseId, locationId, activate: false);

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteLocation(string warehouseId, string locationId)
    {
        if (!TryGetToken(out var token))
        {
            return RedirectToAction("Login", "Auth");
        }

        if (!IsAuthorizedFor(token, "Inventory.Location.Delete", "Location.Delete"))
        {
            TempData["CatalogError"] = "شما دسترسی حذف لوکیشن را ندارید.";
            return RedirectToAction(nameof(Warehouses), new { warehouseId, locationId });
        }

        var result = await _apiService.DeleteLocationAsync(locationId, token);
        TempData[result.IsSuccess ? "CatalogSuccess" : "CatalogError"] =
            result.IsSuccess ? "لوکیشن حذف شد." : result.ErrorMessage ?? "حذف لوکیشن با خطا مواجه شد.";
        return RedirectToAction(nameof(Warehouses), new { warehouseId });
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
            TempData["CatalogError"] = "شما دسترسی تغییر وضعیت انبار را ندارید.";
            return RedirectToAction(nameof(Warehouses), new { warehouseId });
        }

        var result = activate
            ? await _apiService.ActivateWarehouseAsync(warehouseId, token)
            : await _apiService.DeactivateWarehouseAsync(warehouseId, token);

        TempData[result.IsSuccess ? "CatalogSuccess" : "CatalogError"] =
            result.IsSuccess ? "وضعیت انبار تغییر کرد." : result.ErrorMessage ?? "تغییر وضعیت انبار با خطا مواجه شد.";
        return RedirectToAction(nameof(Warehouses), new { warehouseId });
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
            return RedirectToAction(nameof(Warehouses), new { warehouseId, locationId });
        }

        var result = activate
            ? await _apiService.ActivateLocationAsync(locationId, token)
            : await _apiService.DeactivateLocationAsync(locationId, token);

        TempData[result.IsSuccess ? "CatalogSuccess" : "CatalogError"] =
            result.IsSuccess ? "وضعیت لوکیشن تغییر کرد." : result.ErrorMessage ?? "تغییر وضعیت لوکیشن با خطا مواجه شد.";
        return RedirectToAction(nameof(Warehouses), new { warehouseId, locationId });
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

        return warehouses.FirstOrDefault()?.WarehouseBusinessKey ?? lookup.FirstOrDefault()?.WarehouseBusinessKey;
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

    private static (DashboardMenuModule? Module, DashboardMenuItem? Item) ResolveMenu(
        IReadOnlyList<DashboardMenuModule> modules,
        string moduleId,
        string itemId)
    {
        var module = modules.FirstOrDefault(m => string.Equals(m.ModuleId, moduleId, StringComparison.OrdinalIgnoreCase));
        var item = module?.Items.FirstOrDefault(i => string.Equals(i.ItemId, itemId, StringComparison.OrdinalIgnoreCase));
        return (module, item);
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
