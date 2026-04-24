namespace Insurance.InventoryService.AppCore.Shared.Warehouse.Warehouses.Queries.SearchWarehouses;

using OysterFx.AppCore.Shared.Queries;

public class SearchWarehousesQuery : IQuery<SearchWarehousesQueryResult>
{
    public string? Code { get; set; }
    public string? Name { get; set; }
    public bool? IsActive { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}
