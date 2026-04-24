namespace Insurance.InventoryService.AppCore.Shared.Warehouse.Locations.Queries.GetLocationLookup;

using Insurance.InventoryService.AppCore.Shared.Warehouse.Locations.Queries.Common;

public class GetLocationLookupQueryResult
{
    public List<LocationLookupItem> Items { get; set; } = new();
}
