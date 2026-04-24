namespace Insurance.InventoryService.AppCore.Shared.InventoryTransactions.Queries.GetByWarehouse;

using OysterFx.AppCore.Shared.Queries;

public class GetInventoryTransactionsByWarehouseQuery : IQuery<GetInventoryTransactionsByWarehouseQueryResult>
{
    public GetInventoryTransactionsByWarehouseQuery(Guid warehouseRef)
    {
        WarehouseRef = warehouseRef;
    }

    public Guid WarehouseRef { get; }
}
