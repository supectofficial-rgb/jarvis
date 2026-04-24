namespace Insurance.InventoryService.AppCore.Shared.Warehouse.Locations.Queries.GetActiveByWarehouseId;

using OysterFx.AppCore.Shared.Queries;

public class GetActiveLocationsByWarehouseIdQuery : IQuery<GetActiveLocationsByWarehouseIdQueryResult>
{
    public GetActiveLocationsByWarehouseIdQuery(Guid warehouseId)
    {
        WarehouseId = warehouseId;
    }

    public Guid WarehouseId { get; }
}
