using System.Text.Json;
using Insurance.InventoryDashboard.Panel.Models;
using Insurance.InventoryDashboard.Panel.Services;
using Microsoft.AspNetCore.Mvc;

namespace Insurance.InventoryDashboard.Panel.Controllers;

public sealed class DashboardController : Controller
{
    private readonly IDashboardConfigService _dashboardConfigService;
    private readonly IUserManagementApiService _apiService;

    public DashboardController(IDashboardConfigService dashboardConfigService, IUserManagementApiService apiService)
    {
        _dashboardConfigService = dashboardConfigService;
        _apiService = apiService;
    }

    [HttpGet]
    public async Task<IActionResult> Index(string? module, string? item, CancellationToken cancellationToken)
    {
        var token = HttpContext.Session.GetString("Token");
        if (string.IsNullOrWhiteSpace(token))
        {
            return RedirectToAction("Login", "Auth");
        }

        var roles = ResolveRolesFromSession(token);
        var modules = await _dashboardConfigService.GetMenuByRolesAsync(roles, cancellationToken);

        var activeModule = ResolveActiveModule(modules, module);
        var activeItem = ResolveActiveItem(activeModule, item);

        var model = new DashboardViewModel
        {
            UserName = HttpContext.Session.GetString("UserName") ?? "کاربر",
            Roles = roles,
            Modules = modules,
            ActiveModule = activeModule,
            ActiveItem = activeItem
        };

        if (activeModule is { IsEnabled: true } && activeModule.ModuleId == "people_management")
        {
            model.PeopleSnapshot = await BuildPeopleSnapshotAsync(token);
        }

        ViewBag.UserDisplayName = model.UserName;
        ViewBag.MenuModules = modules;
        ViewBag.ActiveModuleId = activeModule?.ModuleId;
        ViewBag.ActiveItemId = activeItem?.ItemId;

        return View(model);
    }

    private async Task<PeopleManagementSnapshot> BuildPeopleSnapshotAsync(string token)
    {
        var usersTask = _apiService.GetUsersAsync(token);
        var personasTask = _apiService.GetPersonasAsync(token);
        var permissionsTask = _apiService.GetPermissionsAsync(token);
        await Task.WhenAll(usersTask, personasTask, permissionsTask);

        var usersResult = await usersTask;
        var personasResult = await personasTask;
        var permissionsResult = await permissionsTask;

        var errors = new[]
            {
                usersResult.ErrorMessage,
                personasResult.ErrorMessage,
                permissionsResult.ErrorMessage
            }
            .Where(message => !string.IsNullOrWhiteSpace(message))
            .Distinct()
            .ToList();

        return new PeopleManagementSnapshot
        {
            Users = usersResult.Data ?? new List<UserSummaryModel>(),
            Personas = personasResult.Data ?? new List<PersonaSummaryModel>(),
            Permissions = permissionsResult.Data ?? new List<PermissionSummaryModel>(),
            ErrorMessage = errors.Count == 0 ? null : string.Join(" | ", errors)
        };
    }

    private static DashboardMenuModule? ResolveActiveModule(IReadOnlyList<DashboardMenuModule> modules, string? moduleId)
    {
        if (modules.Count == 0)
        {
            return null;
        }

        var resolved = !string.IsNullOrWhiteSpace(moduleId)
            ? modules.FirstOrDefault(m =>
                m.IsEnabled &&
                string.Equals(m.ModuleId, moduleId, StringComparison.OrdinalIgnoreCase))
            : null;

        return resolved ?? modules.FirstOrDefault(m => m.IsEnabled) ?? modules[0];
    }

    private static DashboardMenuItem? ResolveActiveItem(DashboardMenuModule? module, string? itemId)
    {
        if (module is null || !module.IsEnabled || module.Items.Count == 0)
        {
            return null;
        }

        var resolved = !string.IsNullOrWhiteSpace(itemId)
            ? module.Items.FirstOrDefault(i =>
                i.IsEnabled &&
                string.Equals(i.ItemId, itemId, StringComparison.OrdinalIgnoreCase))
            : null;

        return resolved ?? module.Items.FirstOrDefault(i => i.IsEnabled) ?? module.Items[0];
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
                // Ignore malformed session value and recover from token.
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
}
