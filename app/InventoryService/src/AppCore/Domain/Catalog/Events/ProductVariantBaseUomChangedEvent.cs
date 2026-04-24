namespace Insurance.InventoryService.AppCore.Domain.Catalog.Events;

using OysterFx.AppCore.Domain.Events;
using OysterFx.AppCore.Domain.ValueObjects;

public sealed record ProductVariantBaseUomChangedEvent : IDomainEvent
{
    public BusinessKey ProductVariantBusinessKey { get; }
    public Guid PreviousBaseUomRef { get; }
    public Guid BaseUomRef { get; }
    public DateTime OccurredOn { get; }

    public ProductVariantBaseUomChangedEvent(BusinessKey productVariantBusinessKey, Guid previousBaseUomRef, Guid baseUomRef)
    {
        ProductVariantBusinessKey = productVariantBusinessKey;
        PreviousBaseUomRef = previousBaseUomRef;
        BaseUomRef = baseUomRef;
        OccurredOn = DateTime.UtcNow;
    }
}
