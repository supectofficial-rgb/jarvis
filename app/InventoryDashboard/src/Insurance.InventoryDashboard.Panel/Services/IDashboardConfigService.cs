using Insurance.InventoryDashboard.Panel.Models;

namespace Insurance.InventoryDashboard.Panel.Services;

public interface IDashboardConfigService
{
    Task<IReadOnlyList<DashboardMenuModule>> GetMenuByRolesAsync(
        IEnumerable<string> roles,
        CancellationToken cancellationToken = default);
}
