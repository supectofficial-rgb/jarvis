using System.Text.Json;
using Insurance.InventoryDashboard.Panel.Models;
using Insurance.InventoryDashboard.Panel.Services;
using Microsoft.AspNetCore.Mvc;

namespace Insurance.InventoryDashboard.Panel.Controllers;

public sealed class PricingManagementController : Controller
{
    private readonly IDashboardConfigService _dashboardConfigService;

    public PricingManagementController(IDashboardConfigService dashboardConfigService)
    {
        _dashboardConfigService = dashboardConfigService;
    }

    [HttpGet]
    public async Task<IActionResult> Index(string? item = null, CancellationToken cancellationToken = default)
    {
        var token = HttpContext.Session.GetString("Token");
        if (string.IsNullOrWhiteSpace(token))
        {
            return RedirectToAction("Login", "Auth");
        }

        var roles = ResolveRolesFromSession(token);
        var modules = await _dashboardConfigService.GetMenuByRolesAsync(roles, cancellationToken);
        var model = new DashboardViewModel
        {
            UserName = HttpContext.Session.GetString("UserName") ?? "کاربر",
            Roles = roles,
            Modules = modules,
            ActiveModule = modules.FirstOrDefault(x => string.Equals(x.ModuleId, "pricing_management", StringComparison.OrdinalIgnoreCase)),
        };

        var activeItem = string.IsNullOrWhiteSpace(item) ? "variant_prices" : item.Trim();
        model.ActiveItem = model.ActiveModule?.Items.FirstOrDefault(x => string.Equals(x.ItemId, activeItem, StringComparison.OrdinalIgnoreCase))
            ?? model.ActiveModule?.Items.FirstOrDefault(x => string.Equals(x.ItemId, "variant_prices", StringComparison.OrdinalIgnoreCase));

        ViewBag.UserDisplayName = model.UserName;
        ViewBag.MenuModules = model.Modules;
        ViewBag.ActiveModuleId = model.ActiveModule?.ModuleId;
        ViewBag.ActiveItemId = model.ActiveItem?.ItemId;

        return View(model);
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
                        .Where(x => !string.IsNullOrWhiteSpace(x))
                        .Select(x => x.Trim())
                        .Distinct(StringComparer.OrdinalIgnoreCase)
                        .ToList();
                }
            }
            catch
            {
                // Recover from malformed session state by reading the JWT again.
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
}
