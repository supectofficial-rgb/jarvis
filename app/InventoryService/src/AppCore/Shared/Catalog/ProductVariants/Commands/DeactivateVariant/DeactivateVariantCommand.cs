namespace Insurance.InventoryService.AppCore.Shared.Catalog.ProductVariants.Commands.DeactivateVariant;

using OysterFx.AppCore.Shared.Commands;

public class DeactivateVariantCommand : ICommand<DeactivateVariantCommandResult>
{
    public Guid ProductVariantBusinessKey { get; set; }
}
