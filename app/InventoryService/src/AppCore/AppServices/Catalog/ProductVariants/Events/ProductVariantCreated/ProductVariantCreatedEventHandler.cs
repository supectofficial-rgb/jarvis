namespace Insurance.InventoryService.AppCore.AppServices.Catalog.ProductVariants.Events.ProductVariantCreated;

using Insurance.InventoryService.AppCore.Domain.Catalog.Events;
using OysterFx.AppCore.Shared.Events;

public sealed class ProductVariantCreatedEventHandler : IDomainEventHandler<ProductVariantCreatedEvent>
{
    public Task Handle(ProductVariantCreatedEvent @event) => Task.CompletedTask;
}
