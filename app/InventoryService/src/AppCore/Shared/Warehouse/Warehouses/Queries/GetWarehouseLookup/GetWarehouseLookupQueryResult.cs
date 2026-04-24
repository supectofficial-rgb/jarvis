namespace Insurance.InventoryService.AppCore.Shared.Warehouse.Warehouses.Queries.GetWarehouseLookup;

using Insurance.InventoryService.AppCore.Shared.Warehouse.Warehouses.Queries.Common;

public class GetWarehouseLookupQueryResult
{
    public List<WarehouseLookupItem> Items { get; set; } = new();
}
