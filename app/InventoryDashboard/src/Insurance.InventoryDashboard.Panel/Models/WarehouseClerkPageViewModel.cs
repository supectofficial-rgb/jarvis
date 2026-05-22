namespace Insurance.InventoryDashboard.Panel.Models;

public sealed class WarehouseClerkPageViewModel
{
    public string UserName { get; set; } = "کاربر";
    public IReadOnlyList<string> Roles { get; set; } = Array.Empty<string>();
    public IReadOnlyList<DashboardMenuModule> Modules { get; set; } = Array.Empty<DashboardMenuModule>();
    public DashboardMenuModule? ActiveModule { get; set; }
    public DashboardMenuItem? ActiveItem { get; set; }

    public string PageKey { get; set; } = string.Empty;
    public string PageTitle { get; set; } = string.Empty;
    public string PageSubtitle { get; set; } = string.Empty;
    public string PageDescription { get; set; } = string.Empty;
    public string BadgeText { get; set; } = string.Empty;

    public IReadOnlyList<WarehouseClerkActionCard> ActionCards { get; set; } = Array.Empty<WarehouseClerkActionCard>();
    public IReadOnlyList<WarehouseClerkNote> Notes { get; set; } = Array.Empty<WarehouseClerkNote>();
}

public sealed class WarehouseClerkActionCard
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? Route { get; set; }
    public string Icon { get; set; } = "simple-icon-grid";
    public bool IsPrimary { get; set; }
}

public sealed class WarehouseClerkNote
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}
