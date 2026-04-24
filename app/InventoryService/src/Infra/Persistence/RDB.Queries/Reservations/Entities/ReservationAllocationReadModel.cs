namespace Insurance.InventoryService.Infra.Persistence.RDB.Queries.Reservations.Entities;

public class ReservationAllocationReadModel
{
    public long Id { get; set; }
    public Guid BusinessKey { get; set; }
    public Guid ReservationRef { get; set; }
    public Guid? StockDetailRef { get; set; }
    public Guid VariantRef { get; set; }
    public Guid WarehouseRef { get; set; }
    public Guid LocationRef { get; set; }
    public Guid QualityStatusRef { get; set; }
    public string? LotBatchNo { get; set; }
    public Guid? SerialRef { get; set; }
    public decimal AllocatedQty { get; set; }
    public DateTime AllocatedAt { get; set; }
    public decimal ReleasedQty { get; set; }
    public decimal ConsumedQty { get; set; }
}
