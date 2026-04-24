namespace Insurance.InventoryService.AppCore.Shared.Catalog.ProductVariants.Commands.LockVariantInventoryMovement;

using OysterFx.AppCore.Shared.Commands;

public class LockVariantInventoryMovementCommand : ICommand<LockVariantInventoryMovementCommandResult>
{
    public Guid ProductVariantBusinessKey { get; set; }
}
