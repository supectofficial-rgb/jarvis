namespace Insurance.InventoryService.AppCore.Domain.Catalog.Entities;

using OysterFx.AppCore.Domain.Aggregates;

public sealed class CategoryVariantNameFormula : AggregateRoot
{
    private readonly List<CategoryVariantNameFormulaPart> _parts = new();

    public Guid CategoryRef { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string Separator { get; private set; } = " ";
    public bool IncludeCategoryName { get; private set; } = true;
    public int DisplayOrder { get; private set; }
    public bool IsActive { get; private set; }
    public IReadOnlyCollection<CategoryVariantNameFormulaPart> Parts => _parts.AsReadOnly();

    private CategoryVariantNameFormula()
    {
    }

    public static CategoryVariantNameFormula Create(
        Guid categoryRef,
        string name,
        string? separator,
        bool includeCategoryName,
        int displayOrder,
        IEnumerable<(Guid AttributeRef, string? Separator, int SortOrder)> parts,
        bool isActive = true)
    {
        if (categoryRef == Guid.Empty)
            throw new ArgumentException("CategoryRef is required.", nameof(categoryRef));

        var formula = new CategoryVariantNameFormula
        {
            CategoryRef = categoryRef,
            Name = NormalizeRequired(name, nameof(name)),
            Separator = NormalizeSeparator(separator),
            IncludeCategoryName = includeCategoryName,
            DisplayOrder = displayOrder,
            IsActive = isActive
        };
        formula.ReplaceParts(parts);
        return formula;
    }

    public static CategoryVariantNameFormula Create(
        Guid categoryRef,
        string name,
        string? separator,
        bool includeCategoryName,
        int displayOrder,
        IEnumerable<Guid> attributeRefs,
        bool isActive = true)
    {
        var parts = (attributeRefs ?? Array.Empty<Guid>())
            .Where(x => x != Guid.Empty)
            .Distinct()
            .Select((attributeRef, index) => (AttributeRef: attributeRef, Separator: (string?)separator, SortOrder: index + 1))
            .ToList();

        return Create(categoryRef, name, separator, includeCategoryName, displayOrder, parts, isActive);
    }

    public void Update(
        string name,
        string? separator,
        bool includeCategoryName,
        int displayOrder,
        IEnumerable<(Guid AttributeRef, string? Separator, int SortOrder)> parts,
        bool isActive)
    {
        Name = NormalizeRequired(name, nameof(name));
        Separator = NormalizeSeparator(separator);
        IncludeCategoryName = includeCategoryName;
        DisplayOrder = displayOrder;
        IsActive = isActive;
        ReplaceParts(parts);
    }

    public void Update(string name, string? separator, bool includeCategoryName, int displayOrder, IEnumerable<Guid> attributeRefs, bool isActive)
    {
        var parts = (attributeRefs ?? Array.Empty<Guid>())
            .Where(x => x != Guid.Empty)
            .Distinct()
            .Select((attributeRef, index) => (AttributeRef: attributeRef, Separator: (string?)separator, SortOrder: index + 1))
            .ToList();

        Update(name, separator, includeCategoryName, displayOrder, parts, isActive);
    }

    private void ReplaceParts(IEnumerable<(Guid AttributeRef, string? Separator, int SortOrder)> parts)
    {
        var normalizedParts = (parts ?? Array.Empty<(Guid AttributeRef, string? Separator, int SortOrder)>())
            .Where(x => x.AttributeRef != Guid.Empty)
            .GroupBy(x => x.AttributeRef)
            .Select(x => x.First())
            .OrderBy(x => x.SortOrder)
            .ToList();

        _parts.Clear();
        for (var i = 0; i < normalizedParts.Count; i++)
            _parts.Add(CategoryVariantNameFormulaPart.Create(BusinessKey.Value, normalizedParts[i].AttributeRef, normalizedParts[i].Separator, i + 1));
    }

    private static string NormalizeRequired(string value, string paramName)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Value is required.", paramName);

        return value.Trim();
    }

    private static string NormalizeSeparator(string? value) => value is null ? " " : value;
}
