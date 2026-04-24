namespace Insurance.InventoryService.AppCore.Shared.Warehouse.Locations.Queries.GetByWarehouseId;

using OysterFx.AppCore.Shared.Queries;

public class GetLocationsByWarehouseIdQuery : IQuery<GetLocationsByWarehouseIdQueryResult>
{
    public GetLocationsByWarehouseIdQuery(Guid warehouseId)
    {
        WarehouseId = warehouseId;
    }

    public Guid WarehouseId { get; }
}
