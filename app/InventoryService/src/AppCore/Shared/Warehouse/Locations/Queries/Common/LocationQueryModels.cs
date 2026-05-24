namespace Insurance.InventoryService.AppCore.Shared.Warehouse.Locations.Queries.Common;

public class LocationListItem
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
    public bool IsActive { get; set; }
}

public class LocationStructureSelectionSummaryItem
{
    public Guid StructureRef { get; set; }
    public string StructureName { get; set; } = string.Empty;
    public Guid StructureValueRef { get; set; }
    public string StructureValueName { get; set; } = string.Empty;
}

public class LocationLookupItem
{
    public Guid LocationBusinessKey { get; set; }
    public Guid WarehouseRef { get; set; }
    public string LocationCode { get; set; } = string.Empty;
    public string LocationType { get; set; } = string.Empty;
}
