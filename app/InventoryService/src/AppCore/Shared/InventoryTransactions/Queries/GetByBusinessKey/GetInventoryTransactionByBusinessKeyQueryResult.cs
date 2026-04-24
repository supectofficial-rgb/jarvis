namespace Insurance.InventoryService.AppCore.Shared.InventoryTransactions.Queries.GetByBusinessKey;

public class GetInventoryTransactionByBusinessKeyQueryResult
{
    public Guid TransactionBusinessKey { get; set; }
    public string TransactionNo { get; set; } = string.Empty;
    public string TransactionType { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string? ReferenceType { get; set; }
    public Guid? ReferenceBusinessId { get; set; }
    public Guid WarehouseRef { get; set; }
    public Guid SellerRef { get; set; }
    public DateTime OccurredAt { get; set; }
    public DateTime? PostedAt { get; set; }
    public string? ReasonCode { get; set; }
    public Guid? ReversedTransactionRef { get; set; }
}
