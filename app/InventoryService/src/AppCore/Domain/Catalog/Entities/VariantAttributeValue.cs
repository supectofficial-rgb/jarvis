namespace Insurance.InventoryService.AppCore.Domain.Catalog.Entities;

using OysterFx.AppCore.Domain.Aggregates;

public sealed class VariantAttributeValue : Aggregate
{
    public Guid VariantRef { get; private set; }
    public Guid AttributeRef { get; private set; }
    public string? Value { get; private set; }
    public Guid? OptionRef { get; private set; }

    private VariantAttributeValue()
    {
    }

    internal static VariantAttributeValue Create(Guid variantRef, Guid attributeRef, string? value, Guid? optionRef)
    {
        return new VariantAttributeValue
        {
            VariantRef = variantRef,
            AttributeRef = attributeRef,
            Value = Normalize(value),
            OptionRef = optionRef
        };
    }

    internal void Update(string? value, Guid? optionRef)
    {
        Value = Normalize(value);
        OptionRef = optionRef;
    }

    private static string? Normalize(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
