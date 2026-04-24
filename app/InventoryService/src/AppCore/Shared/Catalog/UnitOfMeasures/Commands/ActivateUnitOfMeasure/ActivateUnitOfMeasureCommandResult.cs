namespace Insurance.InventoryService.AppCore.Shared.Catalog.UnitOfMeasures.Commands.ActivateUnitOfMeasure;

public class ActivateUnitOfMeasureCommandResult
{
    public Guid UnitOfMeasureBusinessKey { get; set; }
    public bool IsActive { get; set; }
}
