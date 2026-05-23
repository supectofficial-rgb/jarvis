namespace Insurance.InventoryService.AppCore.Shared.Warehouse.LocationStructures.Queries.GetLocationStructureValues;

using Insurance.InventoryService.AppCore.Shared.Warehouse.LocationStructures.Queries.Common;

public sealed class GetLocationStructureValuesQueryResult
{
    public List<LocationStructureValueItem> Items { get; set; } = new();
}
