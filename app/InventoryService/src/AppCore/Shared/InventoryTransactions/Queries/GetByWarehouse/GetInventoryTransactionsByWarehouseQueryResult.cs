namespace Insurance.InventoryService.AppCore.Shared.InventoryTransactions.Queries.GetByWarehouse;

using Insurance.InventoryService.AppCore.Shared.InventoryTransactions.Queries.Common;

public class GetInventoryTransactionsByWarehouseQueryResult
{
    public List<InventoryTransactionListItem> Items { get; set; } = new();
}
