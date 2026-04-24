namespace Insurance.InventoryService.AppCore.Shared.Warehouse.Locations.Queries.SearchLocations;

using Insurance.InventoryService.AppCore.Shared.Warehouse.Locations.Queries.Common;

public class SearchLocationsQueryResult
{
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public List<LocationListItem> Items { get; set; } = new();
}
