namespace Insurance.InventoryService.AppCore.Shared.InventoryTransactions.Queries.GetById;

using Insurance.InventoryService.AppCore.Shared.InventoryTransactions.Queries.Common;

public class GetInventoryTransactionByIdQueryResult
{
    public InventoryTransactionListItem Item { get; set; } = new();
}
