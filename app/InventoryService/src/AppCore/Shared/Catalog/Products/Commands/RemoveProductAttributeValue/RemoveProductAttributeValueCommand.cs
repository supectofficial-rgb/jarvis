namespace Insurance.InventoryService.AppCore.Shared.Catalog.Products.Commands.RemoveProductAttributeValue;

using OysterFx.AppCore.Shared.Commands;

public class RemoveProductAttributeValueCommand : ICommand<RemoveProductAttributeValueCommandResult>
{
    public Guid ProductBusinessKey { get; set; }
    public Guid AttributeRef { get; set; }
}
