namespace Insurance.InventoryService.AppCore.Shared.Catalog.ProductVariants.Commands.ChangeVariantBaseUom;

public class ChangeVariantBaseUomCommandResult
{
    public Guid ProductVariantBusinessKey { get; set; }
    public Guid BaseUomRef { get; set; }
}
