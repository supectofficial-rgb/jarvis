namespace Insurance.InventoryService.AppCore.Shared.Catalog.ProductVariants.Commands.UpsertVariantAddOn;

public class UpsertVariantAddOnCommandResult
{
    public Guid ProductVariantBusinessKey { get; set; }
    public Guid AddOnVariantRef { get; set; }
}
