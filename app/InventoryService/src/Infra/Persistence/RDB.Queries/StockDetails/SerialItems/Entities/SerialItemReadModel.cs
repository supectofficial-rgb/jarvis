namespace Insurance.InventoryService.Infra.Persistence.RDB.Queries.StockDetails.SerialItems.Entities;

using Insurance.InventoryService.AppCore.Domain.StockDetails.Entities;

public class SerialItemReadModel
{
    public long Id { get; set; }
    public Guid BusinessKey { get; set; }
    public string SerialNo { get; set; } = string.Empty;
    public Guid VariantRef { get; set; }
    public Guid SellerRef { get; set; }
    public Guid WarehouseRef { get; set; }
    public Guid LocationRef { get; set; }
    public Guid? StockDetailRef { get; set; }
    public Guid QualityStatusRef { get; set; }
    public string? LotBatchNo { get; set; }
    public SerialItemStatus Status { get; set; }
    public DateTime DateScannedIn { get; set; }
    public Guid? LastTransactionRef { get; set; }
    public DateTime LastUpdatedAt { get; set; }
}
