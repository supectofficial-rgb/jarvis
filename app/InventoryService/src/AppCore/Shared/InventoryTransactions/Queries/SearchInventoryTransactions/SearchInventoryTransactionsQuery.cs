namespace Insurance.InventoryService.AppCore.Shared.InventoryTransactions.Queries.SearchInventoryTransactions;

using OysterFx.AppCore.Shared.Queries;

public class SearchInventoryTransactionsQuery : IQuery<SearchInventoryTransactionsQueryResult>
{
    public string? TransactionNo { get; set; }
    public string? TransactionType { get; set; }
    public string? Status { get; set; }
    public Guid? WarehouseRef { get; set; }
    public Guid? SellerRef { get; set; }
    public DateTime? OccurredFrom { get; set; }
    public DateTime? OccurredTo { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}
