namespace Insurance.InventoryService.AppCore.Domain.Catalog.Entities;

using OysterFx.AppCore.Domain.Aggregates;

public sealed class ProductAttributeValue : Aggregate
{
    public Guid ProductRef { get; private set; }
    public Guid AttributeRef { get; private set; }
    public string? Value { get; private set; }
    public Guid? OptionRef { get; private set; }

    private ProductAttributeValue()
    {
    }

    internal static ProductAttributeValue Create(Guid productRef, Guid attributeRef, string? value, Guid? optionRef)
    {
        return new ProductAttributeValue
        {
            ProductRef = productRef,
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
