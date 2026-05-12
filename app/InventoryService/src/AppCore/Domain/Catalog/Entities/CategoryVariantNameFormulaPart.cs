namespace Insurance.InventoryService.AppCore.Domain.Catalog.Entities;

using OysterFx.AppCore.Domain.Aggregates;
using OysterFx.AppCore.Domain.ValueObjects;

public sealed class CategoryVariantNameFormulaPart : Aggregate
{
    public BusinessKey FormulaRef { get; private set; } = null!;
    public Guid AttributeRef { get; private set; }
    public int SortOrder { get; private set; }

    private CategoryVariantNameFormulaPart()
    {
    }

    internal static CategoryVariantNameFormulaPart Create(Guid formulaRef, Guid attributeRef, int sortOrder)
    {
        if (formulaRef == Guid.Empty)
            throw new ArgumentException("FormulaRef is required.", nameof(formulaRef));

        if (attributeRef == Guid.Empty)
            throw new ArgumentException("AttributeRef is required.", nameof(attributeRef));

        return new CategoryVariantNameFormulaPart
        {
            FormulaRef = BusinessKey.FromGuid(formulaRef),
            AttributeRef = attributeRef,
            SortOrder = sortOrder
        };
    }
}
