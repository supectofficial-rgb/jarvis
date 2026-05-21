namespace Insurance.InventoryService.Infra.Persistence.RDB.Queries.Catalog.ProductVariants.Entities;

public class VariantImageReadModel
{
    public long Id { get; set; }
    public Guid BusinessKey { get; set; }
    public Guid VariantRef { get; set; }
    public string FileKey { get; set; } = string.Empty;
    public string OriginalFileName { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public string OriginalUrl { get; set; } = string.Empty;
    public string ThumbnailUrl { get; set; } = string.Empty;
    public int DisplayOrder { get; set; }
    public bool IsPrimary { get; set; }
}
