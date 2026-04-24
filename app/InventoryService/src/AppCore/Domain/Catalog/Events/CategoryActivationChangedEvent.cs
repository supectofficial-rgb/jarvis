namespace Insurance.InventoryService.AppCore.Domain.Catalog.Events;

using OysterFx.AppCore.Domain.Events;
using OysterFx.AppCore.Domain.ValueObjects;

public sealed record CategoryActivationChangedEvent : IDomainEvent
{
    public BusinessKey CategoryBusinessKey { get; }
    public bool IsActive { get; }
    public DateTime OccurredOn { get; }

    public CategoryActivationChangedEvent(BusinessKey categoryBusinessKey, bool isActive)
    {
        CategoryBusinessKey = categoryBusinessKey;
        IsActive = isActive;
        OccurredOn = DateTime.UtcNow;
    }
}
