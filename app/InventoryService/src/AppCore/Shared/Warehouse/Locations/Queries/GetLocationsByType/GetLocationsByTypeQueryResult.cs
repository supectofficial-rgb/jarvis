namespace Insurance.InventoryService.AppCore.Shared.Warehouse.Locations.Queries.GetLocationsByType;

using Insurance.InventoryService.AppCore.Shared.Warehouse.Locations.Queries.Common;

public class GetLocationsByTypeQueryResult
{
    public List<LocationListItem> Items { get; set; } = new();
}
