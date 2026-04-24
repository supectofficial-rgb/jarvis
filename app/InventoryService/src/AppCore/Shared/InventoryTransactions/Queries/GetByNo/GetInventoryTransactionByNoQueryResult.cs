namespace Insurance.InventoryService.AppCore.Shared.InventoryTransactions.Queries.GetByNo;

using Insurance.InventoryService.AppCore.Shared.InventoryTransactions.Queries.Common;

public class GetInventoryTransactionByNoQueryResult
{
    public InventoryTransactionListItem Item { get; set; } = new();
}
