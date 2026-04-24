namespace Insurance.InventoryService.AppCore.Shared.InventoryTransactions.Queries.GetByVariant;

using Insurance.InventoryService.AppCore.Shared.InventoryTransactions.Queries.Common;

public class GetInventoryTransactionsByVariantQueryResult
{
    public List<InventoryTransactionListItem> Items { get; set; } = new();
}
