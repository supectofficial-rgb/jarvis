namespace Insurance.InventoryService.AppCore.Shared.Catalog.ProductVariants.Commands.RemoveVariantAddOn;

using OysterFx.AppCore.Shared.Commands;

public class RemoveVariantAddOnCommand : ICommand<RemoveVariantAddOnCommandResult>
{
    public Guid ProductVariantBusinessKey { get; set; }
    public Guid AddOnVariantRef { get; set; }
}
