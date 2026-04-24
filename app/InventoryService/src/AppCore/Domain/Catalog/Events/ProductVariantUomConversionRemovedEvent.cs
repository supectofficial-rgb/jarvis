namespace Insurance.InventoryService.AppCore.Domain.Catalog.Events;

using OysterFx.AppCore.Domain.Events;
using OysterFx.AppCore.Domain.ValueObjects;

public sealed record ProductVariantUomConversionRemovedEvent : IDomainEvent
{
    public BusinessKey ProductVariantBusinessKey { get; }
    public Guid FromUomRef { get; }
    public Guid ToUomRef { get; }
    public DateTime OccurredOn { get; }

    public ProductVariantUomConversionRemovedEvent(BusinessKey productVariantBusinessKey, Guid fromUomRef, Guid toUomRef)
    {
        ProductVariantBusinessKey = productVariantBusinessKey;
        FromUomRef = fromUomRef;
        ToUomRef = toUomRef;
        OccurredOn = DateTime.UtcNow;
    }
}
