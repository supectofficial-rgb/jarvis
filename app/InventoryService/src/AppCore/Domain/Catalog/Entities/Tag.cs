namespace Insurance.InventoryService.AppCore.Domain.Catalog.Entities;

using OysterFx.AppCore.Domain.Aggregates;

public sealed class Tag : AggregateRoot
{
    public string TagName { get; private set; } = string.Empty;
    public string? TagColor { get; private set; }

    private Tag()
    {
    }

    private Tag(string tagName, string? tagColor)
    {
        TagName = NormalizeRequired(tagName, nameof(tagName));
        TagColor = NormalizeOptional(tagColor);
    }

    public static Tag Create(string tagName, string? tagColor = null)
        => new(tagName, tagColor);

    public void Update(string tagName, string? tagColor = null)
    {
        TagName = NormalizeRequired(tagName, nameof(tagName));
        TagColor = NormalizeOptional(tagColor);
    }

    private static string NormalizeRequired(string value, string paramName)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Value is required.", paramName);

        return value.Trim();
    }

    private static string? NormalizeOptional(string? value)
        => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
