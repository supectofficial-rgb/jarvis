namespace Insurance.InventoryService.AppCore.Domain.Catalog.Events;

using OysterFx.AppCore.Domain.Events;
using OysterFx.AppCore.Domain.ValueObjects;

public sealed record ProductVariantAttributeValueUpsertedEvent : IDomainEvent
{
    public BusinessKey ProductVariantBusinessKey { get; }
    public Guid AttributeRef { get; }
    public string? Value { get; }
    public Guid? OptionRef { get; }
    public DateTime OccurredOn { get; }

    public ProductVariantAttributeValueUpsertedEvent(BusinessKey productVariantBusinessKey, Guid attributeRef, string? value, Guid? optionRef)
    {
        ProductVariantBusinessKey = productVariantBusinessKey;
        AttributeRef = attributeRef;
        Value = value;
        OptionRef = optionRef;
        OccurredOn = DateTime.UtcNow;
    }
}
