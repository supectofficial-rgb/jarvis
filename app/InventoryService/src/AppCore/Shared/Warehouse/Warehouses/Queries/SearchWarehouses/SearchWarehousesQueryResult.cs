namespace Insurance.InventoryService.AppCore.Shared.Warehouse.Warehouses.Queries.SearchWarehouses;

using Insurance.InventoryService.AppCore.Shared.Warehouse.Warehouses.Queries.Common;

public class SearchWarehousesQueryResult
{
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public List<WarehouseListItem> Items { get; set; } = new();
}
