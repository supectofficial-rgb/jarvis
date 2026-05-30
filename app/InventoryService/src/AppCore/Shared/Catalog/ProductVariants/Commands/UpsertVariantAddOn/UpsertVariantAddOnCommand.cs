namespace Insurance.InventoryService.AppCore.Shared.Catalog.ProductVariants.Commands.UpsertVariantAddOn;

using OysterFx.AppCore.Shared.Commands;

public class UpsertVariantAddOnCommand : ICommand<UpsertVariantAddOnCommandResult>
{
    public Guid ProductVariantBusinessKey { get; set; }
    public Guid? AddOnVariantRef { get; set; }
    public Guid? TagId { get; set; }
    public bool IsRequired { get; set; }
}
