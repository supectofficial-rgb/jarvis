namespace Insurance.InventoryService.AppCore.Domain.Catalog.Events;

using OysterFx.AppCore.Domain.Events;
using OysterFx.AppCore.Domain.ValueObjects;

public sealed record UnitOfMeasureActivationChangedEvent : IDomainEvent
{
    public BusinessKey UnitOfMeasureBusinessKey { get; }
    public bool IsActive { get; }
    public DateTime OccurredOn { get; }

    public UnitOfMeasureActivationChangedEvent(BusinessKey unitOfMeasureBusinessKey, bool isActive)
    {
        UnitOfMeasureBusinessKey = unitOfMeasureBusinessKey;
        IsActive = isActive;
        OccurredOn = DateTime.UtcNow;
    }
}
