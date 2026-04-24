namespace Insurance.InventoryService.AppCore.Shared.Catalog.ProductVariants.Commands.ChangeVariantBaseUom;

using OysterFx.AppCore.Shared.Commands;

public class ChangeVariantBaseUomCommand : ICommand<ChangeVariantBaseUomCommandResult>
{
    public Guid ProductVariantBusinessKey { get; set; }
    public Guid BaseUomRef { get; set; }
}
