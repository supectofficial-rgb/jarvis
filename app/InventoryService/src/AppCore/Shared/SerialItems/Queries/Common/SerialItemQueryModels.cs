namespace Insurance.InventoryService.AppCore.Shared.SerialItems.Queries.Common;

public class SerialItemListItem
{
    public Guid SerialItemBusinessKey { get; set; }
    public string SerialNo { get; set; } = string.Empty;
    public Guid VariantRef { get; set; }
    public Guid SellerRef { get; set; }
    public Guid WarehouseRef { get; set; }
    public Guid LocationRef { get; set; }
    public Guid? StockDetailRef { get; set; }
    public Guid QualityStatusRef { get; set; }
    public string? LotBatchNo { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime DateScannedIn { get; set; }
    public Guid? LastTransactionRef { get; set; }
    public DateTime LastUpdatedAt { get; set; }
}
