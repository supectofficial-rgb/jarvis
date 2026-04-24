namespace Insurance.InventoryService.AppCore.Shared.InventoryTransactions.Queries.SearchInventoryTransactions;

using Insurance.InventoryService.AppCore.Shared.InventoryTransactions.Queries.Common;

public class SearchInventoryTransactionsQueryResult
{
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public List<InventoryTransactionListItem> Items { get; set; } = new();
}
