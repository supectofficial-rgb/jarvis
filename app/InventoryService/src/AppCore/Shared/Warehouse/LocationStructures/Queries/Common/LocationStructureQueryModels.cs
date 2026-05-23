namespace Insurance.InventoryService.AppCore.Shared.Warehouse.LocationStructures.Queries.Common;

public class LocationStructureValueItem
{
    public Guid LocationStructureValueBusinessKey { get; set; }
    public Guid StructureRef { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; }
}

public class LocationStructureTreeItem
{
    public Guid LocationStructureBusinessKey { get; set; }
    public Guid WarehouseRef { get; set; }
    public Guid? ParentStructureRef { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; }
    public List<LocationStructureTreeItem> Children { get; set; } = new();
    public List<LocationStructureValueItem> Values { get; set; } = new();
}

public class LocationStructureSelectionItem
{
    public Guid LocationStructureSelectionBusinessKey { get; set; }
    public Guid LocationRef { get; set; }
    public Guid StructureRef { get; set; }
    public Guid StructureValueRef { get; set; }
}
