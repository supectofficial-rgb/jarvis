namespace Insurance.InventoryService.AppCore.AppServices.Catalog.ProductVariants.Events.ProductVariantAttributeValueUpserted;

using Insurance.InventoryService.AppCore.Domain.Catalog.Events;
using OysterFx.AppCore.Shared.Events;

public sealed class ProductVariantAttributeValueUpsertedEventHandler : IDomainEventHandler<ProductVariantAttributeValueUpsertedEvent>
{
    public Task Handle(ProductVariantAttributeValueUpsertedEvent @event) => Task.CompletedTask;
}
