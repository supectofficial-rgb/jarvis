namespace Insurance.InventoryService.AppCore.Shared.Warehouse.Locations.Queries.GetByBusinessKey;

using Insurance.InventoryService.AppCore.Shared.Warehouse.LocationStructures.Queries.Common;

public class GetLocationByBusinessKeyQueryResult
{
    public Guid LocationBusinessKey { get; set; }
    public Guid WarehouseRef { get; set; }
    public string LocationCode { get; set; } = string.Empty;
    public string LocationType { get; set; } = string.Empty;
    public string? Aisle { get; set; }
    public string? Rack { get; set; }
    public string? Shelf { get; set; }
    public string? Bin { get; set; }
    public string StructureSummary { get; set; } = string.Empty;
    public List<LocationStructureSelectionItem> StructureSelections { get; set; } = new();
    public bool IsActive { get; set; }
}
