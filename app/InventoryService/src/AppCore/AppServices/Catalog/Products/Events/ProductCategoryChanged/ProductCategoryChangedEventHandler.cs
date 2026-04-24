namespace Insurance.InventoryService.AppCore.AppServices.Catalog.Products.Events.ProductCategoryChanged;

using Insurance.InventoryService.AppCore.Domain.Catalog.Events;
using OysterFx.AppCore.Shared.Events;

public sealed class ProductCategoryChangedEventHandler : IDomainEventHandler<ProductCategoryChangedEvent>
{
    public Task Handle(ProductCategoryChangedEvent @event)
    {
        return Task.CompletedTask;
    }
}
