namespace Insurance.InventoryService.AppCore.Shared.Catalog.Products.Commands.RemoveProductAttributeValue;

public class RemoveProductAttributeValueCommandResult
{
    public Guid ProductBusinessKey { get; set; }
    public Guid AttributeRef { get; set; }
    public bool Removed { get; set; }
}
