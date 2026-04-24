namespace Insurance.InventoryService.AppCore.AppServices.Catalog.Products.Events.ProductCreated;

using Insurance.InventoryService.AppCore.Domain.Catalog.Events;
using OysterFx.AppCore.Shared.Events;

public sealed class ProductCreatedEventHandler : IDomainEventHandler<ProductCreatedEvent>
{
    public Task Handle(ProductCreatedEvent @event) => Task.CompletedTask;
}
