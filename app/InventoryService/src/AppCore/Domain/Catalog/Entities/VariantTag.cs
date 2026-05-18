namespace Insurance.InventoryService.AppCore.Domain.Catalog.Entities;

using OysterFx.AppCore.Domain.Aggregates;

public sealed class VariantTag : Aggregate
{
    public Guid VariantRef { get; private set; }
    public string TagName { get; private set; } = string.Empty;
    public string? TagColor { get; private set; }
    public int DisplayOrder { get; private set; }

    private VariantTag()
    {
    }

    internal static VariantTag Create(
        Guid variantRef,
        Guid variantTagBusinessKey,
        string tagName,
        string? tagColor,
        int displayOrder)
    {
        if (variantTagBusinessKey == Guid.Empty)
            throw new ArgumentException("VariantTagBusinessKey is required.", nameof(variantTagBusinessKey));

        return new VariantTag
        {
            BusinessKey = variantTagBusinessKey,
            VariantRef = variantRef,
            TagName = NormalizeRequired(tagName, nameof(tagName)),
            TagColor = NormalizeOptional(tagColor),
            DisplayOrder = displayOrder
        };
    }

    internal void Update(string tagName, string? tagColor, int displayOrder)
    {
        TagName = NormalizeRequired(tagName, nameof(tagName));
        TagColor = NormalizeOptional(tagColor);
        DisplayOrder = displayOrder;
    }

    private static string NormalizeRequired(string value, string paramName)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Value is required.", paramName);

        return value.Trim();
    }

    private static string? NormalizeOptional(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }
}
