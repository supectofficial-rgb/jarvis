namespace Insurance.InventoryService.AppCore.Shared.Catalog.UnitOfMeasures.Commands.DeactivateUnitOfMeasure;

public class DeactivateUnitOfMeasureCommandResult
{
    public Guid UnitOfMeasureBusinessKey { get; set; }
    public bool IsActive { get; set; }
}
