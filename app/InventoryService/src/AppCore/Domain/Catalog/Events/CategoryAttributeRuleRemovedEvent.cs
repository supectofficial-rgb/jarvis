namespace Insurance.InventoryService.AppCore.Domain.Catalog.Events;

using OysterFx.AppCore.Domain.Events;
using OysterFx.AppCore.Domain.ValueObjects;

public sealed record CategoryAttributeRuleRemovedEvent : IDomainEvent
{
    public BusinessKey CategoryBusinessKey { get; }
    public Guid CategorySchemaVersionRef { get; }
    public Guid AttributeRef { get; }
    public DateTime OccurredOn { get; }

    public CategoryAttributeRuleRemovedEvent(BusinessKey categoryBusinessKey, Guid categorySchemaVersionRef, Guid attributeRef)
    {
        CategoryBusinessKey = categoryBusinessKey;
        CategorySchemaVersionRef = categorySchemaVersionRef;
        AttributeRef = attributeRef;
        OccurredOn = DateTime.UtcNow;
    }
}
