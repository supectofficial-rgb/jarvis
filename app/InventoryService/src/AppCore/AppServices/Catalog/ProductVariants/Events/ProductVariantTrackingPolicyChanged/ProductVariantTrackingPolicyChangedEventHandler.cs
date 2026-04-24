namespace Insurance.InventoryService.AppCore.AppServices.Catalog.ProductVariants.Events.ProductVariantTrackingPolicyChanged;

using Insurance.InventoryService.AppCore.Domain.Catalog.Events;
using OysterFx.AppCore.Shared.Events;

public sealed class ProductVariantTrackingPolicyChangedEventHandler : IDomainEventHandler<ProductVariantTrackingPolicyChangedEvent>
{
    public Task Handle(ProductVariantTrackingPolicyChangedEvent @event)
    {
        return Task.CompletedTask;
    }
}
