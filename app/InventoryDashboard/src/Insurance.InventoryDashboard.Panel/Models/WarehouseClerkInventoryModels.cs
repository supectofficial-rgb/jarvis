namespace Insurance.InventoryDashboard.Panel.Models;

public sealed class WarehouseClerkInventoryPageViewModel
{
    public string UserName { get; set; } = "کاربر";
    public IReadOnlyList<string> Roles { get; set; } = Array.Empty<string>();
    public IReadOnlyList<string> Permissions { get; set; } = Array.Empty<string>();
    public IReadOnlyList<DashboardMenuModule> Modules { get; set; } = Array.Empty<DashboardMenuModule>();
    public DashboardMenuModule? ActiveModule { get; set; }
    public DashboardMenuItem? ActiveItem { get; set; }

    public string? ErrorMessage { get; set; }
    public IReadOnlyList<WarehouseLookupItemModel> WarehouseLookup { get; set; } = Array.Empty<WarehouseLookupItemModel>();
    public string? SelectedWarehouseId { get; set; }
    public string? SelectedWarehouseName { get; set; }
    public string? StructureSelectionsJson { get; set; }
    public IReadOnlyList<LocationStructureTreeItemModel> Structures { get; set; } = Array.Empty<LocationStructureTreeItemModel>();
    public IReadOnlyList<WarehouseClerkInventoryItemModel> Items { get; set; } = Array.Empty<WarehouseClerkInventoryItemModel>();

    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public int TotalCount { get; set; }
    public int TotalPages { get; set; } = 1;
    public IReadOnlyList<int> PageSizeOptions { get; set; } = new[] { 10, 25, 50 };
}

public sealed class WarehouseClerkInventoryItemModel
{
    public string VariantRef { get; set; } = string.Empty;
    public string VariantSku { get; set; } = string.Empty;
    public string VariantName { get; set; } = string.Empty;
    public decimal QuantityOnHand { get; set; }
    public int BucketCount { get; set; }
    public int SerialCount { get; set; }
}

public sealed class WarehouseClerkInventorySerialItemModel
{
    public string SerialItemBusinessKey { get; set; } = string.Empty;
    public string SerialNo { get; set; } = string.Empty;
    public string WarehouseRef { get; set; } = string.Empty;
    public string WarehouseLabel { get; set; } = string.Empty;
    public string LocationRef { get; set; } = string.Empty;
    public string LocationLabel { get; set; } = string.Empty;
    public string QualityStatusRef { get; set; } = string.Empty;
    public string QualityStatusLabel { get; set; } = string.Empty;
    public string? LotBatchNo { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime DateScannedIn { get; set; }
    public DateTime LastUpdatedAt { get; set; }
}
