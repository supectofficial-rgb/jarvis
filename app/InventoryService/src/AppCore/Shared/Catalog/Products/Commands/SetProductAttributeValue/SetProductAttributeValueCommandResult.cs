namespace Insurance.InventoryService.AppCore.Shared.Catalog.Products.Commands.SetProductAttributeValue;

public class SetProductAttributeValueCommandResult
{
    public Guid ProductBusinessKey { get; set; }
    public Guid AttributeRef { get; set; }
    public string? Value { get; set; }
    public Guid? OptionRef { get; set; }
}
