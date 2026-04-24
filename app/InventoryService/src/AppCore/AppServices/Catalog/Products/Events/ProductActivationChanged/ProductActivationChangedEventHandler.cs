namespace Insurance.InventoryService.AppCore.AppServices.Catalog.Products.Events.ProductActivationChanged;

using Insurance.InventoryService.AppCore.Domain.Catalog.Events;
using OysterFx.AppCore.Shared.Events;

public sealed class ProductActivationChangedEventHandler : IDomainEventHandler<ProductActivationChangedEvent>
{
    public Task Handle(ProductActivationChangedEvent @event) => Task.CompletedTask;
}
