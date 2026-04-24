namespace Insurance.InventoryService.AppCore.Domain.Catalog.Events;

using OysterFx.AppCore.Domain.Events;
using OysterFx.AppCore.Domain.ValueObjects;

public sealed record CategoryAttributeRuleUpsertedEvent : IDomainEvent
{
    public BusinessKey CategoryBusinessKey { get; }
    public Guid CategorySchemaVersionRef { get; }
    public Guid AttributeRef { get; }
    public bool IsRequired { get; }
    public bool IsVariant { get; }
    public int DisplayOrder { get; }
    public bool IsOverridden { get; }
    public bool IsActive { get; }
    public DateTime OccurredOn { get; }

    public CategoryAttributeRuleUpsertedEvent(
        BusinessKey categoryBusinessKey,
        Guid categorySchemaVersionRef,
        Guid attributeRef,
        bool isRequired,
        bool isVariant,
        int displayOrder,
        bool isOverridden,
        bool isActive)
    {
        CategoryBusinessKey = categoryBusinessKey;
        CategorySchemaVersionRef = categorySchemaVersionRef;
        AttributeRef = attributeRef;
        IsRequired = isRequired;
        IsVariant = isVariant;
        DisplayOrder = displayOrder;
        IsOverridden = isOverridden;
        IsActive = isActive;
        OccurredOn = DateTime.UtcNow;
    }
}
