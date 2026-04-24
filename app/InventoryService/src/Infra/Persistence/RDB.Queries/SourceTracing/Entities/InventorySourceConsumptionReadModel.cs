namespace Insurance.InventoryService.Infra.Persistence.RDB.Queries.SourceTracing.Entities;

public class InventorySourceConsumptionReadModel
{
    public long Id { get; set; }
    public Guid BusinessKey { get; set; }
    public Guid OutboundTransactionRef { get; set; }
    public Guid? OutboundTransactionLineRef { get; set; }
    public Guid SourceBalanceRef { get; set; }
    public Guid VariantRef { get; set; }
    public decimal ConsumedQty { get; set; }
    public string? ReasonCode { get; set; }
    public DateTime CreatedAt { get; set; }
}
