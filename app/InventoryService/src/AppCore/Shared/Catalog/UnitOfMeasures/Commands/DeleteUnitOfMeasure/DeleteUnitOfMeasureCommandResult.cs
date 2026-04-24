namespace Insurance.InventoryService.AppCore.Shared.Catalog.UnitOfMeasures.Commands.DeleteUnitOfMeasure;

public class DeleteUnitOfMeasureCommandResult
{
    public Guid UnitOfMeasureBusinessKey { get; set; }
    public bool Deleted { get; set; }
}
