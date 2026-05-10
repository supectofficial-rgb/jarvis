namespace Insurance.InventoryService.AppCore.Shared.Catalog.ProductVariants.Commands.RemoveVariantImage;

public class RemoveVariantImageCommandResult
{
    public Guid ProductVariantBusinessKey { get; set; }
    public string FileKey { get; set; } = string.Empty;
}
