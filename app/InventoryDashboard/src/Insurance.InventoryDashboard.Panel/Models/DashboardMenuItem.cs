namespace Insurance.InventoryDashboard.Panel.Models;

public sealed class DashboardMenuItem
{
    public string ItemId { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Icon { get; set; } = "simple-icon-grid";
    public bool IsEnabled { get; set; } = true;
    public string? Description { get; set; }
    public string? Route { get; set; }
    public List<string> Roles { get; set; } = new();
}
