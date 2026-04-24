namespace Insurance.InventoryService.AppCore.Shared.Warehouse.Locations.Queries.GetLocationsByType;

using OysterFx.AppCore.Shared.Queries;

public class GetLocationsByTypeQuery : IQuery<GetLocationsByTypeQueryResult>
{
    public GetLocationsByTypeQuery(string locationType, Guid? warehouseRef = null, bool onlyActive = false)
    {
        LocationType = locationType;
        WarehouseRef = warehouseRef;
        OnlyActive = onlyActive;
    }

    public string LocationType { get; }
    public Guid? WarehouseRef { get; }
    public bool OnlyActive { get; }
}
