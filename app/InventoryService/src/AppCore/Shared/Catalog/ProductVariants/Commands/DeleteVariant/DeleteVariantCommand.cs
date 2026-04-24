namespace Insurance.InventoryService.AppCore.Shared.Catalog.ProductVariants.Commands.DeleteVariant;

using OysterFx.AppCore.Shared.Commands;

public class DeleteVariantCommand : ICommand<DeleteVariantCommandResult>
{
    public Guid ProductVariantBusinessKey { get; set; }
}
