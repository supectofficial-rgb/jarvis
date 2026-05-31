namespace Insurance.InventoryService.AppCore.Domain.Catalog.Entities;

using OysterFx.AppCore.Domain.Aggregates;
using OysterFx.AppCore.Domain.ValueObjects;

public sealed class CategoryVariantNameFormulaPart : Aggregate
{
    public BusinessKey FormulaRef { get; private set; } = null!;
    public Guid AttributeRef { get; private set; }
    public string Separator { get; private set; } = string.Empty;
    public int SortOrder { get; private set; }

    private CategoryVariantNameFormulaPart()
    {
    }

    internal static CategoryVariantNameFormulaPart Create(Guid formulaRef, Guid attributeRef, string? separator, int sortOrder)
    {
        if (formulaRef == Guid.Empty)
            throw new ArgumentException("FormulaRef is required.", nameof(formulaRef));

        if (attributeRef == Guid.Empty)
            throw new ArgumentException("AttributeRef is required.", nameof(attributeRef));

        return new CategoryVariantNameFormulaPart
        {
            FormulaRef = BusinessKey.FromGuid(formulaRef),
            AttributeRef = attributeRef,
            Separator = NormalizeSeparator(separator),
            SortOrder = sortOrder
        };
    }

    private static string NormalizeSeparator(string? value) => value is null ? " " : value;
}
