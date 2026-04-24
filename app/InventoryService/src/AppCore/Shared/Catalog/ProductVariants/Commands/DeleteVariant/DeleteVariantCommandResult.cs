namespace Insurance.InventoryService.AppCore.Shared.Catalog.ProductVariants.Commands.DeleteVariant;

public class DeleteVariantCommandResult
{
    public Guid ProductVariantBusinessKey { get; set; }
    public bool Deleted { get; set; }
}
