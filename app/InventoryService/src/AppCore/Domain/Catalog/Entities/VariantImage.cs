namespace Insurance.InventoryService.AppCore.Domain.Catalog.Entities;

using OysterFx.AppCore.Domain.Entities;

public sealed class VariantImage : Entity<long>
{
    public Guid VariantRef { get; private set; }
    public string FileKey { get; private set; } = string.Empty;
    public string OriginalFileName { get; private set; } = string.Empty;
    public string ContentType { get; private set; } = string.Empty;
    public string OriginalUrl { get; private set; } = string.Empty;
    public string ThumbnailUrl { get; private set; } = string.Empty;
    public int DisplayOrder { get; private set; }
    public bool IsPrimary { get; private set; }

    private VariantImage()
    {
    }

    public static VariantImage Create(
        Guid variantRef,
        string fileKey,
        string originalFileName,
        string contentType,
        string originalUrl,
        string thumbnailUrl,
        int displayOrder,
        bool isPrimary)
    {
        return new VariantImage
        {
            VariantRef = variantRef,
            FileKey = NormalizeRequired(fileKey, nameof(fileKey)),
            OriginalFileName = NormalizeRequired(originalFileName, nameof(originalFileName)),
            ContentType = NormalizeRequired(contentType, nameof(contentType)),
            OriginalUrl = NormalizeRequired(originalUrl, nameof(originalUrl)),
            ThumbnailUrl = NormalizeRequired(thumbnailUrl, nameof(thumbnailUrl)),
            DisplayOrder = displayOrder,
            IsPrimary = isPrimary
        };
    }

    public void Update(
        string originalFileName,
        string contentType,
        string originalUrl,
        string thumbnailUrl,
        int displayOrder,
        bool isPrimary)
    {
        OriginalFileName = NormalizeRequired(originalFileName, nameof(originalFileName));
        ContentType = NormalizeRequired(contentType, nameof(contentType));
        OriginalUrl = NormalizeRequired(originalUrl, nameof(originalUrl));
        ThumbnailUrl = NormalizeRequired(thumbnailUrl, nameof(thumbnailUrl));
        DisplayOrder = displayOrder;
        IsPrimary = isPrimary;
    }

    public void SetPrimary(bool isPrimary)
    {
        IsPrimary = isPrimary;
    }

    private static string NormalizeRequired(string value, string paramName)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Value is required.", paramName);

        return value.Trim();
    }
}
