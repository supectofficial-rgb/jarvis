namespace Insurance.InventoryService.AppCore.Shared.Catalog.ProductVariants.Commands.LockVariantInventoryMovement;

public class LockVariantInventoryMovementCommandResult
{
    public Guid ProductVariantBusinessKey { get; set; }
    public bool InventoryMovementLocked { get; set; }
}
