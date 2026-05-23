namespace Insurance.InventoryService.AppCore.Domain.Warehouse.Entities;

using OysterFx.AppCore.Domain.Aggregates;

public sealed class LocationStructureValue : AggregateRoot
{
    public Guid StructureRef { get; private set; }
    public string Code { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public int DisplayOrder { get; private set; }
    public bool IsActive { get; private set; }

    private LocationStructureValue()
    {
    }

    private LocationStructureValue(Guid structureRef, string code, string name, int displayOrder)
    {
        StructureRef = structureRef;
        Code = NormalizeRequired(code, nameof(code));
        Name = NormalizeRequired(name, nameof(name));
        DisplayOrder = displayOrder;
        IsActive = true;
    }

    public static LocationStructureValue Create(Guid structureRef, string code, string name, int displayOrder = 0)
    {
        return new LocationStructureValue(structureRef, code, name, displayOrder);
    }

    public void ChangeStructure(Guid structureRef) => StructureRef = structureRef;

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
