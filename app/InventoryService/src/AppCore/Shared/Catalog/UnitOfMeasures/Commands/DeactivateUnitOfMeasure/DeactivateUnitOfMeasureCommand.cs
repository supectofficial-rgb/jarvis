namespace Insurance.InventoryService.AppCore.Shared.Catalog.UnitOfMeasures.Commands.DeactivateUnitOfMeasure;

using OysterFx.AppCore.Shared.Commands;

public class DeactivateUnitOfMeasureCommand : ICommand<DeactivateUnitOfMeasureCommandResult>
{
    public Guid UnitOfMeasureBusinessKey { get; set; }
}
