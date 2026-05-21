namespace Insurance.InventoryService.AppCore.Shared.Catalog.ProductVariants.Commands.RemoveVariantImage;

using OysterFx.AppCore.Shared.Commands;

public class RemoveVariantImageCommand : ICommand<RemoveVariantImageCommandResult>
{
    public Guid VariantImageBusinessKey { get; set; }
}
