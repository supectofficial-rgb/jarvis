namespace Insurance.InventoryService.Infra.Persistence.RDB.Queries.InventoryTransactions.Entities;

public class InventoryTransactionLineReadModel
{
    public long Id { get; set; }
    public long InventoryTransactionId { get; set; }
    public Guid BusinessKey { get; set; }
    public Guid? StockDetailRef { get; set; }
    public Guid VariantRef { get; set; }
    public decimal InputQty { get; set; }
    public Guid InputUomRef { get; set; }
    public decimal BaseQtyDelta { get; set; }
    public Guid BaseUomRef { get; set; }
    public Guid? SourceLocationRef { get; set; }
    public Guid? DestinationLocationRef { get; set; }
    public Guid? OldQualityStatusRef { get; set; }
    public Guid? NewQualityStatusRef { get; set; }
    public string? LotBatchNo { get; set; }
    public Guid? SerialRef { get; set; }
    public string? ReasonCode { get; set; }
}
