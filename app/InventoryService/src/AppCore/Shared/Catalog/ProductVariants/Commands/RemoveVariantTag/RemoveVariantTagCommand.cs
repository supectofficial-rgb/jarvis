namespace Insurance.InventoryService.AppCore.Shared.Catalog.ProductVariants.Commands.RemoveVariantTag;

using OysterFx.AppCore.Shared.Commands;

public class RemoveVariantTagCommand : ICommand<RemoveVariantTagCommandResult>
{
    public Guid VariantTagBusinessKey { get; set; }
}
