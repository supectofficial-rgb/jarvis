namespace Insurance.InventoryService.AppCore.Domain.Catalog.Events;

using OysterFx.AppCore.Domain.Events;
using OysterFx.AppCore.Domain.ValueObjects;

public sealed record AttributeDefinitionOptionRemovedEvent : IDomainEvent
{
    public BusinessKey AttributeDefinitionBusinessKey { get; }
    public Guid OptionBusinessKey { get; }
    public string Value { get; }
    public DateTime OccurredOn { get; }

    public AttributeDefinitionOptionRemovedEvent(BusinessKey attributeDefinitionBusinessKey, Guid optionBusinessKey, string value)
    {
        AttributeDefinitionBusinessKey = attributeDefinitionBusinessKey;
        OptionBusinessKey = optionBusinessKey;
        Value = value;
        OccurredOn = DateTime.UtcNow;
    }
}
