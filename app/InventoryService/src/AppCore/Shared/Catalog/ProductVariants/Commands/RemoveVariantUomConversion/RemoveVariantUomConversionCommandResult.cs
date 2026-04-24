namespace Insurance.InventoryService.AppCore.Shared.Catalog.ProductVariants.Commands.RemoveVariantUomConversion;

public class RemoveVariantUomConversionCommandResult
{
    public Guid ProductVariantBusinessKey { get; set; }
    public Guid FromUomRef { get; set; }
    public Guid ToUomRef { get; set; }
    public bool Removed { get; set; }
}
