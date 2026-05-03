namespace Insurance.InventoryService.AppCore.Domain.Catalog.Entities;

using OysterFx.AppCore.Domain.Aggregates;

public sealed class CategoryVariantNameFormulaPart : Aggregate
{
    public Guid FormulaRef { get; private set; }
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
            FormulaRef = formulaRef,
            AttributeRef = attributeRef,
            SortOrder = sortOrder
        };
    }
}
