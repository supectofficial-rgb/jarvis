namespace Insurance.InventoryService.Infra.Persistence.RDB.Queries.SourceTracing.Entities;

using Insurance.InventoryService.AppCore.Domain.SourceTracing.Entities;

public class InventorySourceBalanceReadModel
{
    public long Id { get; set; }
    public Guid BusinessKey { get; set; }
    public InventorySourceType SourceType { get; set; }
    public Guid VariantRef { get; set; }
    public Guid SellerRef { get; set; }
    public Guid WarehouseRef { get; set; }
    public Guid LocationRef { get; set; }
    public Guid QualityStatusRef { get; set; }
    public string? LotBatchNo { get; set; }
    public decimal ReceivedQty { get; set; }
    public decimal AllocatedQty { get; set; }
    public decimal ConsumedQty { get; set; }
    public decimal AvailableQty { get; set; }
    public decimal RemainingQty { get; set; }
    public InventorySourceBalanceStatus Status { get; set; }
}
