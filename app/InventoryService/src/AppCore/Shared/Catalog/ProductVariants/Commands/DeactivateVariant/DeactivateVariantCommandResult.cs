namespace Insurance.InventoryService.AppCore.Shared.Catalog.ProductVariants.Commands.DeactivateVariant;

public class DeactivateVariantCommandResult
{
    public Guid ProductVariantBusinessKey { get; set; }
    public bool IsActive { get; set; }
}
