namespace Insurance.InventoryService.AppCore.Shared.Warehouse.Warehouses.Queries.GetById;

using Insurance.InventoryService.AppCore.Shared.Warehouse.Warehouses.Queries.GetByBusinessKey;
using OysterFx.AppCore.Shared.Queries;

public class GetWarehouseByIdQuery : IQuery<GetWarehouseByBusinessKeyQueryResult>
{
    public GetWarehouseByIdQuery(Guid warehouseId)
    {
        WarehouseId = warehouseId;
    }

    public Guid WarehouseId { get; }
}
