namespace Insurance.InventoryService.AppCore.Shared.Catalog.ProductVariants.Commands.RemoveVariantTag;

public class RemoveVariantTagCommandResult
{
    public Guid ProductVariantBusinessKey { get; set; }
    public Guid? VariantTagBusinessKey { get; set; }
    public string? TagName { get; set; }
}
