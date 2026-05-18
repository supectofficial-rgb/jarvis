namespace Insurance.InventoryService.Infra.Persistence.RDB.Queries.Catalog.ProductVariants.Entities;

public class VariantTagReadModel
{
    public long Id { get; set; }
    public Guid BusinessKey { get; set; }
    public Guid VariantRef { get; set; }
    public string TagName { get; set; } = string.Empty;
    public string? TagColor { get; set; }
    public int DisplayOrder { get; set; }
}
