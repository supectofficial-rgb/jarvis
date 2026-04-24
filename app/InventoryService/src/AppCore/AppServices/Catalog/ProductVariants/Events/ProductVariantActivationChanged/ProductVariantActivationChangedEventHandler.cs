namespace Insurance.InventoryService.AppCore.AppServices.Catalog.ProductVariants.Events.ProductVariantActivationChanged;

using Insurance.InventoryService.AppCore.Domain.Catalog.Events;
using OysterFx.AppCore.Shared.Events;

public sealed class ProductVariantActivationChangedEventHandler : IDomainEventHandler<ProductVariantActivationChangedEvent>
{
    public Task Handle(ProductVariantActivationChangedEvent @event) => Task.CompletedTask;
}
