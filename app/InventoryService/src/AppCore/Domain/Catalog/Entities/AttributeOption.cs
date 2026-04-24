namespace Insurance.InventoryService.AppCore.Domain.Catalog.Entities;

using OysterFx.AppCore.Domain.Aggregates;

public sealed class AttributeOption : Aggregate
{
    public Guid AttributeRef { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string Value { get; private set; } = string.Empty;
    public int DisplayOrder { get; private set; }
    public bool IsActive { get; private set; }

    private AttributeOption()
    {
    }

    internal static AttributeOption Create(Guid attributeRef, string name, string value, int displayOrder)
    {
        return Create(attributeRef, Guid.NewGuid(), name, value, displayOrder);
    }

    internal static AttributeOption Create(Guid attributeRef, Guid optionBusinessKey, string name, string value, int displayOrder)
    {
        if (optionBusinessKey == Guid.Empty)
            throw new ArgumentException("OptionBusinessKey is required.", nameof(optionBusinessKey));

        var normalizedName = NormalizeRequired(name, nameof(name));
        var normalizedValue = NormalizeRequired(value, nameof(value));

        return new AttributeOption
        {
            AttributeRef = attributeRef,
            BusinessKey = optionBusinessKey,
            Name = normalizedName,
            Value = normalizedValue,
            DisplayOrder = displayOrder,
            IsActive = true
        };
    }

    public void Rename(string name)
    {
        Name = NormalizeRequired(name, nameof(name));
    }

    public void ChangeValue(string value)
    {
        Value = NormalizeRequired(value, nameof(value));
    }

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
