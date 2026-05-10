namespace Insurance.InventoryService.AppCore.Shared.Catalog.ProductVariants.Commands.UpsertVariantImage;

public class UpsertVariantImageCommandResult
{
    public Guid ProductVariantBusinessKey { get; set; }
    public string FileKey { get; set; } = string.Empty;
}
