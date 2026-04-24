namespace Insurance.InventoryService.AppCore.Shared.Catalog.ProductVariants.Commands.SetVariantAttributeValue;

public class SetVariantAttributeValueCommandResult
{
    public Guid ProductVariantBusinessKey { get; set; }
    public Guid AttributeRef { get; set; }
    public string? Value { get; set; }
    public Guid? OptionRef { get; set; }
}
