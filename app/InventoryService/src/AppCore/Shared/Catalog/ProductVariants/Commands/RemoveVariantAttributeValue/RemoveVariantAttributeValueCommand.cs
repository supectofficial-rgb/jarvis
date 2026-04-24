namespace Insurance.InventoryService.AppCore.Shared.Catalog.ProductVariants.Commands.RemoveVariantAttributeValue;

using OysterFx.AppCore.Shared.Commands;

public class RemoveVariantAttributeValueCommand : ICommand<RemoveVariantAttributeValueCommandResult>
{
    public Guid ProductVariantBusinessKey { get; set; }
    public Guid AttributeRef { get; set; }
}
