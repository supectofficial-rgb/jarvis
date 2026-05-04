namespace Insurance.InventoryService.AppCore.Shared.Catalog.ProductVariants.Commands.RemoveVariantAddOn;

public class RemoveVariantAddOnCommandResult
{
    public Guid ProductVariantBusinessKey { get; set; }
    public Guid AddOnVariantRef { get; set; }
}
