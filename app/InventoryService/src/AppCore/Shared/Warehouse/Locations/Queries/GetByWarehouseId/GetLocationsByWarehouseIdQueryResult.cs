namespace Insurance.InventoryService.AppCore.Shared.Warehouse.Locations.Queries.GetByWarehouseId;

using Insurance.InventoryService.AppCore.Shared.Warehouse.Locations.Queries.Common;

public class GetLocationsByWarehouseIdQueryResult
{
    public List<LocationListItem> Items { get; set; } = new();
}
