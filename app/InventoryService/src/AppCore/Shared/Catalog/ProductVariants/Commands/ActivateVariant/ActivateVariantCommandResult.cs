namespace Insurance.InventoryService.AppCore.Shared.Catalog.ProductVariants.Commands.ActivateVariant;

public class ActivateVariantCommandResult
{
    public Guid ProductVariantBusinessKey { get; set; }
    public bool IsActive { get; set; }
}
