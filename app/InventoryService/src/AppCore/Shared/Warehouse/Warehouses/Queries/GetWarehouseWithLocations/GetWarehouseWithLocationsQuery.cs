namespace Insurance.InventoryService.AppCore.Shared.Warehouse.Warehouses.Queries.GetWarehouseWithLocations;

using OysterFx.AppCore.Shared.Queries;

public class GetWarehouseWithLocationsQuery : IQuery<GetWarehouseWithLocationsQueryResult>
{
    public GetWarehouseWithLocationsQuery(Guid warehouseBusinessKey, bool includeInactiveLocations = false)
    {
        WarehouseBusinessKey = warehouseBusinessKey;
        IncludeInactiveLocations = includeInactiveLocations;
    }

    public Guid WarehouseBusinessKey { get; }
    public bool IncludeInactiveLocations { get; }
}
