namespace Insurance.InventoryService.AppCore.Shared.Catalog.ProductVariants.Commands.RemoveVariantComponent;

using OysterFx.AppCore.Shared.Commands;

public class RemoveVariantComponentCommand : ICommand<RemoveVariantComponentCommandResult>
{
    public Guid ProductVariantBusinessKey { get; set; }
    public Guid ComponentVariantRef { get; set; }
}
