namespace Insurance.InventoryService.AppCore.Shared.Catalog.ProductVariants.Commands.UpdateProductVariant;

public class UpdateProductVariantCommandResult
{
    public Guid ProductVariantBusinessKey { get; set; }
    public string VariantSku { get; set; } = string.Empty;
    public string? Barcode { get; set; }
    public string TrackingPolicy { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}
