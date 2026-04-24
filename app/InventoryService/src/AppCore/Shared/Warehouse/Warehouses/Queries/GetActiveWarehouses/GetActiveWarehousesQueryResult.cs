namespace Insurance.InventoryService.AppCore.Shared.Warehouse.Warehouses.Queries.GetActiveWarehouses;

using Insurance.InventoryService.AppCore.Shared.Warehouse.Warehouses.Queries.Common;

public class GetActiveWarehousesQueryResult
{
    public List<WarehouseListItem> Items { get; set; } = new();
}
