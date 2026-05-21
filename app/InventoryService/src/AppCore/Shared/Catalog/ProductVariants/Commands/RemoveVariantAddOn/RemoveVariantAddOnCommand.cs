namespace Insurance.InventoryService.AppCore.Shared.Catalog.ProductVariants.Commands.RemoveVariantAddOn;

using OysterFx.AppCore.Shared.Commands;

public class RemoveVariantAddOnCommand : ICommand<RemoveVariantAddOnCommandResult>
{
    public Guid VariantAddOnBusinessKey { get; set; }
}
