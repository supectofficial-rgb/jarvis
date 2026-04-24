namespace Insurance.InventoryDashboard.Panel.Models;

public sealed class DashboardMenuModule
{
    public string ModuleId { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Icon { get; set; } = "iconsminds-three-arrow-fork";
    public bool IsEnabled { get; set; } = true;
    public string? Description { get; set; }
    public List<string> Roles { get; set; } = new();
    public List<DashboardMenuItem> Items { get; set; } = new();
}
