namespace Insurance.InventoryService.AppCore.Domain.Catalog.Events;

using OysterFx.AppCore.Domain.Events;
using OysterFx.AppCore.Domain.ValueObjects;

public sealed record ProductVariantImageUpsertedEvent : IDomainEvent
{
    public BusinessKey ProductVariantBusinessKey { get; }
    public string FileKey { get; }
    public string OriginalFileName { get; }
    public string ContentType { get; }
    public string OriginalUrl { get; }
    public string ThumbnailUrl { get; }
    public int DisplayOrder { get; }
    public bool IsPrimary { get; }
    public DateTime OccurredOn { get; }

    public ProductVariantImageUpsertedEvent(
        BusinessKey productVariantBusinessKey,
        string fileKey,
        string originalFileName,
        string contentType,
        string originalUrl,
        string thumbnailUrl,
        int displayOrder,
        bool isPrimary)
    {
        ProductVariantBusinessKey = productVariantBusinessKey;
        FileKey = fileKey;
        OriginalFileName = originalFileName;
        ContentType = contentType;
        OriginalUrl = originalUrl;
        ThumbnailUrl = thumbnailUrl;
        DisplayOrder = displayOrder;
        IsPrimary = isPrimary;
        OccurredOn = DateTime.UtcNow;
    }
}
