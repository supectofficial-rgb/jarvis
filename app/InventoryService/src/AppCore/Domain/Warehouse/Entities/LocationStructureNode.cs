namespace Insurance.InventoryService.AppCore.Domain.Warehouse.Entities;

using OysterFx.AppCore.Domain.Aggregates;

public sealed class LocationStructureNode : AggregateRoot
{
    public Guid WarehouseRef { get; private set; }
    public Guid? ParentStructureRef { get; private set; }
    public string Code { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public int DisplayOrder { get; private set; }
    public bool IsActive { get; private set; }

    private LocationStructureNode()
    {
    }

    private LocationStructureNode(
        Guid warehouseRef,
        Guid? parentStructureRef,
        string code,
        string name,
        int displayOrder)
    {
        WarehouseRef = warehouseRef;
        ParentStructureRef = parentStructureRef;
        Code = NormalizeRequired(code, nameof(code));
        Name = NormalizeRequired(name, nameof(name));
        DisplayOrder = displayOrder;
        IsActive = true;
    }

    public static LocationStructureNode Create(
        Guid warehouseRef,
        string code,
        string name,
        int displayOrder = 0,
        Guid? parentStructureRef = null)
    {
        return new LocationStructureNode(warehouseRef, parentStructureRef, code, name, displayOrder);
    }

    public void ChangeWarehouse(Guid warehouseRef) => WarehouseRef = warehouseRef;

    public void ChangeParent(Guid? parentStructureRef) => ParentStructureRef = parentStructureRef;

    public void ChangeCode(string code) => Code = NormalizeRequired(code, nameof(code));

    public void Rename(string name) => Name = NormalizeRequired(name, nameof(name));

    public void ChangeDisplayOrder(int displayOrder) => DisplayOrder = displayOrder;

    public void Activate() => IsActive = true;

    public void Deactivate() => IsActive = false;

    private static string NormalizeRequired(string value, string paramName)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Value is required.", paramName);

        return value.Trim();
    }
}
