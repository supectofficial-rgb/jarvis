namespace Insurance.InventoryService.AppCore.Shared.Warehouse.Warehouses.Queries.GetByBusinessKey;

using OysterFx.AppCore.Shared.Queries;

public class GetWarehouseByBusinessKeyQuery : IQuery<GetWarehouseByBusinessKeyQueryResult>
{
    public GetWarehouseByBusinessKeyQuery(Guid warehouseBusinessKey)
    {
        WarehouseBusinessKey = warehouseBusinessKey;
    }

    public Guid WarehouseBusinessKey { get; }
}
