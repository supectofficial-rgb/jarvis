namespace Insurance.InventoryService.AppCore.Shared.Warehouse.Locations.Queries.GetActiveByWarehouseId;

using Insurance.InventoryService.AppCore.Shared.Warehouse.Locations.Queries.Common;

public class GetActiveLocationsByWarehouseIdQueryResult
{
    public List<LocationListItem> Items { get; set; } = new();
}
