namespace Insurance.InventoryService.AppCore.Shared.Warehouse.LocationStructures.Queries.GetWarehouseLocationStructureTree;

using Insurance.InventoryService.AppCore.Shared.Warehouse.LocationStructures.Queries.Common;

public sealed class GetWarehouseLocationStructureTreeQueryResult
{
    public List<LocationStructureTreeItem> Items { get; set; } = new();
}
