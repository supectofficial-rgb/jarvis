namespace Insurance.InventoryService.AppCore.Domain.Catalog.Entities;

using OysterFx.AppCore.Domain.Aggregates;

public sealed class CategoryVariantNameFormula : AggregateRoot
{
    private readonly List<CategoryVariantNameFormulaPart> _parts = new();

    public Guid CategoryRef { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string Separator { get; private set; } = " ";
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
        int displayOrder,
        IEnumerable<Guid> attributeRefs,
        bool isActive = true)
    {
        if (categoryRef == Guid.Empty)
            throw new ArgumentException("CategoryRef is required.", nameof(categoryRef));

        var formula = new CategoryVariantNameFormula
        {
            CategoryRef = categoryRef,
            Name = NormalizeRequired(name, nameof(name)),
            Separator = NormalizeSeparator(separator),
            DisplayOrder = displayOrder,
            IsActive = isActive
        };
        formula.ReplaceParts(attributeRefs);
        return formula;
    }

    public void Update(string name, string? separator, int displayOrder, IEnumerable<Guid> attributeRefs, bool isActive)
    {
        Name = NormalizeRequired(name, nameof(name));
        Separator = NormalizeSeparator(separator);
        DisplayOrder = displayOrder;
        IsActive = isActive;
        ReplaceParts(attributeRefs);
    }

    private void ReplaceParts(IEnumerable<Guid> attributeRefs)
    {
        var distinctRefs = (attributeRefs ?? Array.Empty<Guid>())
            .Where(x => x != Guid.Empty)
            .Distinct()
            .ToList();

        _parts.Clear();
        for (var i = 0; i < distinctRefs.Count; i++)
            _parts.Add(CategoryVariantNameFormulaPart.Create(BusinessKey.Value, distinctRefs[i], i + 1));
    }

    private static string NormalizeRequired(string value, string paramName)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Value is required.", paramName);

        return value.Trim();
    }

    private static string NormalizeSeparator(string? value) => string.IsNullOrEmpty(value) ? " " : value;
}
