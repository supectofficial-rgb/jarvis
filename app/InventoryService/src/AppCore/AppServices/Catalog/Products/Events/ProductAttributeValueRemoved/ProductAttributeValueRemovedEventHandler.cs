namespace Insurance.InventoryService.AppCore.AppServices.Catalog.Products.Events.ProductAttributeValueRemoved;

using Insurance.InventoryService.AppCore.Domain.Catalog.Events;
using OysterFx.AppCore.Shared.Events;

public sealed class ProductAttributeValueRemovedEventHandler : IDomainEventHandler<ProductAttributeValueRemovedEvent>
{
    public Task Handle(ProductAttributeValueRemovedEvent @event) => Task.CompletedTask;
}
