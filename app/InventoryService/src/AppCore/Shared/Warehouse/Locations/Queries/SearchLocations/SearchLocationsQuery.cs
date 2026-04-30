namespace Insurance.InventoryService.AppCore.Shared.Warehouse.Locations.Queries.SearchLocations;

using OysterFx.AppCore.Shared.Queries;

public class SearchLocationsQuery : IQuery<SearchLocationsQueryResult>
{
    public Guid? WarehouseRef { get; set; }
    public string? LocationCode { get; set; }
    public string? LocationType { get; set; }
    public string? Aisle { get; set; }
    public string? Rack { get; set; }
    public string? Shelf { get; set; }
    public string? Bin { get; set; }
    public bool? IsActive { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}
