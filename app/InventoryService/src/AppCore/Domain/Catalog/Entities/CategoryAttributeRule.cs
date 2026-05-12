namespace Insurance.InventoryService.AppCore.Domain.Catalog.Entities;

using OysterFx.AppCore.Domain.Aggregates;
using OysterFx.AppCore.Domain.ValueObjects;

public sealed class CategoryAttributeRule : Aggregate
{
    public BusinessKey CategorySchemaVersionRef { get; private set; } = null!;
    public Guid AttributeRef { get; private set; }
    public bool IsRequired { get; private set; }
    public bool IsVariant { get; private set; }
    public bool IsVariantCodeCovered { get; private set; }
    public int DisplayOrder { get; private set; }
    public bool IsOverridden { get; private set; }
    public bool IsActive { get; private set; }

    private CategoryAttributeRule()
    {
    }

    internal static CategoryAttributeRule Create(
        Guid categorySchemaVersionRef,
        Guid attributeRef,
        bool isRequired,
        bool isVariant,
        bool isVariantCodeCovered,
        int displayOrder,
        bool isOverridden,
        bool isActive)
    {
        if (categorySchemaVersionRef == Guid.Empty)
            throw new ArgumentException("CategorySchemaVersionRef is required.", nameof(categorySchemaVersionRef));

        if (attributeRef == Guid.Empty)
            throw new ArgumentException("AttributeRef is required.", nameof(attributeRef));

        return new CategoryAttributeRule
        {
            CategorySchemaVersionRef = BusinessKey.FromGuid(categorySchemaVersionRef),
            AttributeRef = attributeRef,
            IsRequired = isRequired,
            IsVariant = isVariant,
            IsVariantCodeCovered = isVariantCodeCovered,
            DisplayOrder = displayOrder,
            IsOverridden = isOverridden,
            IsActive = isActive
        };
    }

    internal CategoryAttributeRule CloneForSchemaVersion(Guid categorySchemaVersionRef)
    {
        return Create(
            categorySchemaVersionRef,
            AttributeRef,
            IsRequired,
            IsVariant,
            IsVariantCodeCovered,
            DisplayOrder,
            IsOverridden,
            IsActive);
    }

    internal void Update(bool isRequired, bool isVariant, bool isVariantCodeCovered, int displayOrder, bool isOverridden, bool isActive)
    {
        IsRequired = isRequired;
        IsVariant = isVariant;
        IsVariantCodeCovered = isVariantCodeCovered;
        DisplayOrder = displayOrder;
        IsOverridden = isOverridden;
        IsActive = isActive;
    }
}
