namespace Insurance.InventoryService.AppCore.Domain.Catalog.Events;

using OysterFx.AppCore.Domain.Events;
using OysterFx.AppCore.Domain.ValueObjects;

public sealed record ProductAttributeValueSnapshot(Guid AttributeRef, string? Value, Guid? OptionRef);

public sealed record ProductCreatedEvent : IDomainEvent
{
    public BusinessKey ProductBusinessKey { get; }
    public Guid CategoryRef { get; }
    public Guid CategorySchemaVersionRef { get; }
    public string BaseSku { get; }
    public string Name { get; }
    public Guid DefaultUomRef { get; }
    public Guid? TaxCategoryRef { get; }
    public bool IsActive { get; }
    public IReadOnlyCollection<ProductAttributeValueSnapshot> AttributeValues { get; }
    public DateTime OccurredOn { get; }

    public ProductCreatedEvent(
        BusinessKey productBusinessKey,
        Guid categoryRef,
        Guid categorySchemaVersionRef,
        string baseSku,
        string name,
        Guid defaultUomRef,
        Guid? taxCategoryRef,
        bool isActive,
        IReadOnlyCollection<ProductAttributeValueSnapshot> attributeValues)
    {
        ProductBusinessKey = productBusinessKey;
        CategoryRef = categoryRef;
        CategorySchemaVersionRef = categorySchemaVersionRef;
        BaseSku = baseSku;
        Name = name;
        DefaultUomRef = defaultUomRef;
        TaxCategoryRef = taxCategoryRef;
        IsActive = isActive;
        AttributeValues = attributeValues;
        OccurredOn = DateTime.UtcNow;
    }
}
