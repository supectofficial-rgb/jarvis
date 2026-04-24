namespace Insurance.InventoryService.AppCore.AppServices.Catalog.Products.Events.ProductAttributeValueUpserted;

using Insurance.InventoryService.AppCore.Domain.Catalog.Events;
using OysterFx.AppCore.Shared.Events;

public sealed class ProductAttributeValueUpsertedEventHandler : IDomainEventHandler<ProductAttributeValueUpsertedEvent>
{
    public Task Handle(ProductAttributeValueUpsertedEvent @event) => Task.CompletedTask;
}
