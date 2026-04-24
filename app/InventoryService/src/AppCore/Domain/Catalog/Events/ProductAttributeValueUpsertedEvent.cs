namespace Insurance.InventoryService.AppCore.Domain.Catalog.Events;

using OysterFx.AppCore.Domain.Events;
using OysterFx.AppCore.Domain.ValueObjects;

public sealed record ProductAttributeValueUpsertedEvent : IDomainEvent
{
    public BusinessKey ProductBusinessKey { get; }
    public Guid AttributeRef { get; }
    public string? Value { get; }
    public Guid? OptionRef { get; }
    public DateTime OccurredOn { get; }

    public ProductAttributeValueUpsertedEvent(BusinessKey productBusinessKey, Guid attributeRef, string? value, Guid? optionRef)
    {
        ProductBusinessKey = productBusinessKey;
        AttributeRef = attributeRef;
        Value = value;
        OptionRef = optionRef;
        OccurredOn = DateTime.UtcNow;
    }
}
