namespace Insurance.InventoryService.AppCore.Shared.Catalog.ProductVariants.Commands.RemoveVariantComponent;

public class RemoveVariantComponentCommandResult
{
    public Guid ProductVariantBusinessKey { get; set; }
    public Guid ComponentVariantRef { get; set; }
}
