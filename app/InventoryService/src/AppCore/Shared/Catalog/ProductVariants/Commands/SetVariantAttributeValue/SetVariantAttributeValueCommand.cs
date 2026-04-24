namespace Insurance.InventoryService.AppCore.Shared.Catalog.ProductVariants.Commands.SetVariantAttributeValue;

using OysterFx.AppCore.Shared.Commands;

public class SetVariantAttributeValueCommand : ICommand<SetVariantAttributeValueCommandResult>
{
    public Guid ProductVariantBusinessKey { get; set; }
    public Guid AttributeRef { get; set; }
    public string? Value { get; set; }
    public Guid? OptionRef { get; set; }
}
