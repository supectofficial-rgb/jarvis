namespace Insurance.InventoryService.Infra.Persistence.RDB.Queries.SourceTracing.Entities;

public class InventorySourceAllocationReadModel
{
    public long Id { get; set; }
    public Guid BusinessKey { get; set; }
    public Guid SourceBalanceRef { get; set; }
    public Guid ReservationRef { get; set; }
    public Guid? ReservationAllocationRef { get; set; }
    public Guid VariantRef { get; set; }
    public decimal AllocatedQty { get; set; }
    public decimal ReleasedQty { get; set; }
    public decimal ConsumedQty { get; set; }
    public DateTime CreatedAt { get; set; }
}
