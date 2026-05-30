namespace Insurance.InventoryService.AppCore.Shared.Catalog.ProductVariants.Commands.UpsertVariantComponent;

public class UpsertVariantComponentCommandResult
{
    public Guid ProductVariantBusinessKey { get; set; }
    public Guid VariantComponentBusinessKey { get; set; }
    public Guid ComponentVariantRef { get; set; }
    public decimal Quantity { get; set; }
}
