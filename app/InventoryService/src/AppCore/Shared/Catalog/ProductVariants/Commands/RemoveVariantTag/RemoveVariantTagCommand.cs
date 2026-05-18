namespace Insurance.InventoryService.AppCore.Shared.Catalog.ProductVariants.Commands.RemoveVariantTag;

using OysterFx.AppCore.Shared.Commands;

public class RemoveVariantTagCommand : ICommand<RemoveVariantTagCommandResult>
{
    public Guid ProductVariantBusinessKey { get; set; }
    public Guid? VariantTagBusinessKey { get; set; }
    public string? TagName { get; set; }
}
