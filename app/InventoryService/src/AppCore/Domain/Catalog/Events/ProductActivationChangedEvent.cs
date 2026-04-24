namespace Insurance.InventoryService.AppCore.Domain.Catalog.Events;

using OysterFx.AppCore.Domain.Events;
using OysterFx.AppCore.Domain.ValueObjects;

public sealed record ProductActivationChangedEvent : IDomainEvent
{
    public BusinessKey ProductBusinessKey { get; }
    public bool IsActive { get; }
    public DateTime OccurredOn { get; }

    public ProductActivationChangedEvent(BusinessKey productBusinessKey, bool isActive)
    {
        ProductBusinessKey = productBusinessKey;
        IsActive = isActive;
        OccurredOn = DateTime.UtcNow;
    }
}
