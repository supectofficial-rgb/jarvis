namespace Insurance.InventoryService.Infra.Persistence.RDB.Queries.InventoryTransactions.Entities;

using Insurance.InventoryService.AppCore.Domain.InventoryTransactions.Entities;

public class InventoryTransactionReadModel
{
    public long Id { get; set; }
    public Guid BusinessKey { get; set; }
    public string TransactionNo { get; set; } = string.Empty;
    public InventoryTransactionType TransactionType { get; set; }
    public string? ReferenceType { get; set; }
    public Guid? ReferenceBusinessId { get; set; }
    public Guid WarehouseRef { get; set; }
    public Guid SellerRef { get; set; }
    public DateTime OccurredAt { get; set; }
    public DateTime? PostedAt { get; set; }
    public InventoryTransactionStatus Status { get; set; }
    public string? CorrelationId { get; set; }
    public string? IdempotencyKey { get; set; }
    public string? ReasonCode { get; set; }
    public Guid? ReversedTransactionRef { get; set; }
}
