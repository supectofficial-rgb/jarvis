using System.Text.Json;
using Insurance.InventoryDashboard.Panel.Models;

namespace Insurance.InventoryDashboard.Panel.Services;

public sealed class DashboardConfigService : IDashboardConfigService
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private readonly IWebHostEnvironment _webHostEnvironment;
    private readonly IConfiguration _configuration;
    private readonly ILogger<DashboardConfigService> _logger;

    public DashboardConfigService(
        IWebHostEnvironment webHostEnvironment,
        IConfiguration configuration,
        ILogger<DashboardConfigService> logger)
    {
        _webHostEnvironment = webHostEnvironment;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<IReadOnlyList<DashboardMenuModule>> GetMenuByRolesAsync(
        IEnumerable<string> roles,
        CancellationToken cancellationToken = default)
    {
        var roleSet = roles
            .Where(role => !string.IsNullOrWhiteSpace(role))
            .Select(role => role.Trim())
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        var config = await LoadConfigAsync(cancellationToken);
        if (config.Dashboard.Count == 0)
        {
            return Array.Empty<DashboardMenuModule>();
        }

        var modules = new List<DashboardMenuModule>();
        foreach (var module in config.Dashboard)
        {
            if (!IsAllowed(module.Roles, roleSet))
            {
                continue;
            }

            var allowedItems = module.Items
                .Where(item => IsAllowed(item.Roles, roleSet))
                .ToList();

            if (module.IsEnabled && allowedItems.Count == 0)
            {
                continue;
            }

            modules.Add(new DashboardMenuModule
            {
                ModuleId = module.ModuleId,
                Title = module.Title,
                Icon = module.Icon,
                IsEnabled = module.IsEnabled,
                Description = module.Description,
                Roles = module.Roles,
                Items = allowedItems
            });
        }

        return modules;
    }

    private static bool IsAllowed(IEnumerable<string> allowedRoles, HashSet<string> userRoles)
    {
        var roles = allowedRoles
            .Where(role => !string.IsNullOrWhiteSpace(role))
            .Select(role => role.Trim())
            .ToList();

        if (roles.Count == 0)
        {
            return true;
        }

        if (userRoles.Count == 0)
        {
            return false;
        }

        return roles.Any(userRoles.Contains);
    }

    private async Task<DashboardMenuConfigRoot> LoadConfigAsync(CancellationToken cancellationToken)
    {
        var configuredPath = _configuration["DashboardConfig:WidgetsFile"] ?? "dashboard-widgets.json";
        var path = Path.Combine(_webHostEnvironment.ContentRootPath, configuredPath);

        if (!File.Exists(path))
        {
            _logger.LogWarning("Dashboard menu config file was not found at {Path}", path);
            return new DashboardMenuConfigRoot();
        }

        try
        {
            await using var stream = File.OpenRead(path);
            var config = await JsonSerializer.DeserializeAsync<DashboardMenuConfigRoot>(stream, JsonOptions, cancellationToken);
            return config ?? new DashboardMenuConfigRoot();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to read dashboard menu config file from {Path}", path);
            return new DashboardMenuConfigRoot();
        }
    }
}
