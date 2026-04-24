namespace Insurance.InventoryService.AppCore.AppServices.Catalog.ProductVariants.Events.ProductVariantUomConversionRemoved;

using Insurance.InventoryService.AppCore.Domain.Catalog.Events;
using OysterFx.AppCore.Shared.Events;

public sealed class ProductVariantUomConversionRemovedEventHandler : IDomainEventHandler<ProductVariantUomConversionRemovedEvent>
{
    public Task Handle(ProductVariantUomConversionRemovedEvent @event) => Task.CompletedTask;
}
