namespace Insurance.InventoryService.AppCore.AppServices.Catalog.ProductVariants.Events.ProductVariantAttributeValueRemoved;

using Insurance.InventoryService.AppCore.Domain.Catalog.Events;
using OysterFx.AppCore.Shared.Events;

public sealed class ProductVariantAttributeValueRemovedEventHandler : IDomainEventHandler<ProductVariantAttributeValueRemovedEvent>
{
    public Task Handle(ProductVariantAttributeValueRemovedEvent @event) => Task.CompletedTask;
}
