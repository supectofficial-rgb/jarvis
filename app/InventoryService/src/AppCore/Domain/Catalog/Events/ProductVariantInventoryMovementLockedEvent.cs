namespace Insurance.InventoryService.AppCore.Domain.Catalog.Events;

using OysterFx.AppCore.Domain.Events;
using OysterFx.AppCore.Domain.ValueObjects;

public sealed record ProductVariantInventoryMovementLockedEvent : IDomainEvent
{
    public BusinessKey ProductVariantBusinessKey { get; }
    public bool InventoryMovementLocked { get; }
    public DateTime OccurredOn { get; }

    public ProductVariantInventoryMovementLockedEvent(BusinessKey productVariantBusinessKey, bool inventoryMovementLocked)
    {
        ProductVariantBusinessKey = productVariantBusinessKey;
        InventoryMovementLocked = inventoryMovementLocked;
        OccurredOn = DateTime.UtcNow;
    }
}
