namespace Insurance.InventoryService.AppCore.AppServices.Catalog.Products.Events.ProductUpdated;

using Insurance.InventoryService.AppCore.Domain.Catalog.Events;
using OysterFx.AppCore.Shared.Events;

public sealed class ProductUpdatedEventHandler : IDomainEventHandler<ProductUpdatedEvent>
{
    public Task Handle(ProductUpdatedEvent @event) => Task.CompletedTask;
}
