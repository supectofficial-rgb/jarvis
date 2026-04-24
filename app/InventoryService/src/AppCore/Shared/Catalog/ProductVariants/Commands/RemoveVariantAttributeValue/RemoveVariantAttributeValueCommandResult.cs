namespace Insurance.InventoryService.AppCore.Shared.Catalog.ProductVariants.Commands.RemoveVariantAttributeValue;

public class RemoveVariantAttributeValueCommandResult
{
    public Guid ProductVariantBusinessKey { get; set; }
    public Guid AttributeRef { get; set; }
    public bool Removed { get; set; }
}
