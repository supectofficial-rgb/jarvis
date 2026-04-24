namespace Insurance.InventoryService.AppCore.Domain.Catalog.Events;

using OysterFx.AppCore.Domain.Events;
using OysterFx.AppCore.Domain.ValueObjects;

public sealed record AttributeDefinitionActivationChangedEvent : IDomainEvent
{
    public BusinessKey AttributeDefinitionBusinessKey { get; }
    public bool IsActive { get; }
    public DateTime OccurredOn { get; }

    public AttributeDefinitionActivationChangedEvent(BusinessKey attributeDefinitionBusinessKey, bool isActive)
    {
        AttributeDefinitionBusinessKey = attributeDefinitionBusinessKey;
        IsActive = isActive;
        OccurredOn = DateTime.UtcNow;
    }
}
