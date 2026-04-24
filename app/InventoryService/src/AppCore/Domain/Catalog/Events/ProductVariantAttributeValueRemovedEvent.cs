namespace Insurance.InventoryService.AppCore.Domain.Catalog.Events;

using OysterFx.AppCore.Domain.Events;
using OysterFx.AppCore.Domain.ValueObjects;

public sealed record ProductVariantAttributeValueRemovedEvent : IDomainEvent
{
    public BusinessKey ProductVariantBusinessKey { get; }
    public Guid AttributeRef { get; }
    public DateTime OccurredOn { get; }

    public ProductVariantAttributeValueRemovedEvent(BusinessKey productVariantBusinessKey, Guid attributeRef)
    {
        ProductVariantBusinessKey = productVariantBusinessKey;
        AttributeRef = attributeRef;
        OccurredOn = DateTime.UtcNow;
    }
}
