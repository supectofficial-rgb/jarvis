namespace Insurance.InventoryService.AppCore.Domain.Catalog.Events;

using OysterFx.AppCore.Domain.Events;
using OysterFx.AppCore.Domain.ValueObjects;

public sealed record ProductVariantActivationChangedEvent : IDomainEvent
{
    public BusinessKey ProductVariantBusinessKey { get; }
    public bool IsActive { get; }
    public DateTime OccurredOn { get; }

    public ProductVariantActivationChangedEvent(BusinessKey productVariantBusinessKey, bool isActive)
    {
        ProductVariantBusinessKey = productVariantBusinessKey;
        IsActive = isActive;
        OccurredOn = DateTime.UtcNow;
    }
}
