namespace Insurance.InventoryService.AppCore.Domain.Catalog.Events;

using OysterFx.AppCore.Domain.Events;
using OysterFx.AppCore.Domain.ValueObjects;

public sealed record AttributeDefinitionOptionActivationChangedEvent : IDomainEvent
{
    public BusinessKey AttributeDefinitionBusinessKey { get; }
    public Guid OptionBusinessKey { get; }
    public bool IsActive { get; }
    public DateTime OccurredOn { get; }

    public AttributeDefinitionOptionActivationChangedEvent(
        BusinessKey attributeDefinitionBusinessKey,
        Guid optionBusinessKey,
        bool isActive)
    {
        AttributeDefinitionBusinessKey = attributeDefinitionBusinessKey;
        OptionBusinessKey = optionBusinessKey;
        IsActive = isActive;
        OccurredOn = DateTime.UtcNow;
    }
}
