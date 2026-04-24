namespace Insurance.InventoryService.AppCore.AppServices.Catalog.ProductVariants.Events.ProductVariantUomConversionUpserted;

using Insurance.InventoryService.AppCore.Domain.Catalog.Events;
using OysterFx.AppCore.Shared.Events;

public sealed class ProductVariantUomConversionUpsertedEventHandler : IDomainEventHandler<ProductVariantUomConversionUpsertedEvent>
{
    public Task Handle(ProductVariantUomConversionUpsertedEvent @event) => Task.CompletedTask;
}
