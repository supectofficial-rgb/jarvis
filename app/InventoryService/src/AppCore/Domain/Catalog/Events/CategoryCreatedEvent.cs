namespace Insurance.InventoryService.AppCore.Domain.Catalog.Events;

using OysterFx.AppCore.Domain.Events;
using OysterFx.AppCore.Domain.ValueObjects;

public sealed record CategoryAttributeRuleSnapshot(
    Guid CategorySchemaVersionRef,
    Guid AttributeRef,
    bool IsRequired,
    bool IsVariant,
    int DisplayOrder,
    bool IsOverridden,
    bool IsActive);

public sealed record CategorySchemaVersionSnapshot(
    Guid SchemaVersionRef,
    Guid CategoryRef,
    int VersionNo,
    bool IsCurrent,
    DateTime CreatedAt,
    string? ChangeSummary,
    IReadOnlyCollection<CategoryAttributeRuleSnapshot> Rules);

public sealed record CategoryCreatedEvent : IDomainEvent
{
    public BusinessKey CategoryBusinessKey { get; }
    public string Code { get; }
    public string Name { get; }
    public int DisplayOrder { get; }
    public Guid? ParentCategoryRef { get; }
    public bool IsActive { get; }
    public IReadOnlyCollection<CategorySchemaVersionSnapshot> SchemaVersions { get; }
    public IReadOnlyCollection<CategoryAttributeRuleSnapshot> AttributeRules =>
        SchemaVersions.FirstOrDefault(x => x.IsCurrent)?.Rules
        ?? Array.Empty<CategoryAttributeRuleSnapshot>();
    public DateTime OccurredOn { get; }

    public CategoryCreatedEvent(
        BusinessKey categoryBusinessKey,
        string code,
        string name,
        int displayOrder,
        Guid? parentCategoryRef,
        bool isActive,
        IReadOnlyCollection<CategorySchemaVersionSnapshot> schemaVersions)
    {
        CategoryBusinessKey = categoryBusinessKey;
        Code = code;
        Name = name;
        DisplayOrder = displayOrder;
        ParentCategoryRef = parentCategoryRef;
        IsActive = isActive;
        SchemaVersions = schemaVersions;
        OccurredOn = DateTime.UtcNow;
    }
}
