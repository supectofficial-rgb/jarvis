namespace Insurance.InventoryService.AppCore.Shared.Warehouse.Locations.Queries.GetLocationLookup;

using OysterFx.AppCore.Shared.Queries;

public class GetLocationLookupQuery : IQuery<GetLocationLookupQueryResult>
{
    public Guid? WarehouseRef { get; set; }
    public bool IncludeInactive { get; set; }
}
