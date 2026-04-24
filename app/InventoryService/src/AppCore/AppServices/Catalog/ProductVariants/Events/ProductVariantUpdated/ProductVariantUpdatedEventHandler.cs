namespace Insurance.InventoryService.AppCore.AppServices.Catalog.ProductVariants.Events.ProductVariantUpdated;

using Insurance.InventoryService.AppCore.Domain.Catalog.Events;
using OysterFx.AppCore.Shared.Events;

public sealed class ProductVariantUpdatedEventHandler : IDomainEventHandler<ProductVariantUpdatedEvent>
{
    public Task Handle(ProductVariantUpdatedEvent @event) => Task.CompletedTask;
}
