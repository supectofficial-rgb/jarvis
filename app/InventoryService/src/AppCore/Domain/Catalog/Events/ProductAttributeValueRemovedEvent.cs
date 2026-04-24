namespace Insurance.InventoryService.AppCore.Domain.Catalog.Events;

using OysterFx.AppCore.Domain.Events;
using OysterFx.AppCore.Domain.ValueObjects;

public sealed record ProductAttributeValueRemovedEvent : IDomainEvent
{
    public BusinessKey ProductBusinessKey { get; }
    public Guid AttributeRef { get; }
    public DateTime OccurredOn { get; }

    public ProductAttributeValueRemovedEvent(BusinessKey productBusinessKey, Guid attributeRef)
    {
        ProductBusinessKey = productBusinessKey;
        AttributeRef = attributeRef;
        OccurredOn = DateTime.UtcNow;
    }
}
