namespace Insurance.InventoryService.AppCore.Shared.Catalog.ProductVariants.Commands.UpsertVariantTag;

using OysterFx.AppCore.Shared.Commands;

public class UpsertVariantTagCommand : ICommand<UpsertVariantTagCommandResult>
{
    public Guid ProductVariantBusinessKey { get; set; }
    public Guid? VariantTagBusinessKey { get; set; }
    public Guid TagBusinessKey { get; set; }
    public int DisplayOrder { get; set; }
}
