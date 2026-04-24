namespace Insurance.InventoryService.AppCore.AppServices.Catalog.ProductVariants.Events.ProductVariantBaseUomChanged;

using Insurance.InventoryService.AppCore.Domain.Catalog.Events;
using OysterFx.AppCore.Shared.Events;

public sealed class ProductVariantBaseUomChangedEventHandler : IDomainEventHandler<ProductVariantBaseUomChangedEvent>
{
    public Task Handle(ProductVariantBaseUomChangedEvent @event)
    {
        return Task.CompletedTask;
    }
}
