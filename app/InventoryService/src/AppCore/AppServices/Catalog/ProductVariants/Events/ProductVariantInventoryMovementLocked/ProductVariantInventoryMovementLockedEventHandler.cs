namespace Insurance.InventoryService.AppCore.AppServices.Catalog.ProductVariants.Events.ProductVariantInventoryMovementLocked;

using Insurance.InventoryService.AppCore.Domain.Catalog.Events;
using OysterFx.AppCore.Shared.Events;

public sealed class ProductVariantInventoryMovementLockedEventHandler : IDomainEventHandler<ProductVariantInventoryMovementLockedEvent>
{
    public Task Handle(ProductVariantInventoryMovementLockedEvent @event) => Task.CompletedTask;
}
