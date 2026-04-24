namespace Insurance.InventoryService.AppCore.Shared.Catalog.ProductVariants.Commands.ActivateVariant;

using OysterFx.AppCore.Shared.Commands;

public class ActivateVariantCommand : ICommand<ActivateVariantCommandResult>
{
    public Guid ProductVariantBusinessKey { get; set; }
}
