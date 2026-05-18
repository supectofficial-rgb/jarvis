namespace Insurance.InventoryService.AppCore.Shared.Catalog.ProductVariants.Commands.UpsertVariantTag;

public class UpsertVariantTagCommandResult
{
    public Guid ProductVariantBusinessKey { get; set; }
    public Guid VariantTagBusinessKey { get; set; }
    public string TagName { get; set; } = string.Empty;
    public string? TagColor { get; set; }
    public int DisplayOrder { get; set; }
}
