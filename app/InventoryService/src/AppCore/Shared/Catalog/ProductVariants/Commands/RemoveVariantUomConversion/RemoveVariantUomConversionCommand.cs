namespace Insurance.InventoryService.AppCore.Shared.Catalog.ProductVariants.Commands.RemoveVariantUomConversion;

using OysterFx.AppCore.Shared.Commands;

public class RemoveVariantUomConversionCommand : ICommand<RemoveVariantUomConversionCommandResult>
{
    public Guid ProductVariantBusinessKey { get; set; }
    public Guid FromUomRef { get; set; }
    public Guid ToUomRef { get; set; }
}
