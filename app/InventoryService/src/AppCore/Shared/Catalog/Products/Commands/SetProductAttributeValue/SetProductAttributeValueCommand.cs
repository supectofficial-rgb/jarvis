namespace Insurance.InventoryService.AppCore.Shared.Catalog.Products.Commands.SetProductAttributeValue;

using OysterFx.AppCore.Shared.Commands;

public class SetProductAttributeValueCommand : ICommand<SetProductAttributeValueCommandResult>
{
    public Guid ProductBusinessKey { get; set; }
    public Guid AttributeRef { get; set; }
    public string? Value { get; set; }
    public Guid? OptionRef { get; set; }
}
