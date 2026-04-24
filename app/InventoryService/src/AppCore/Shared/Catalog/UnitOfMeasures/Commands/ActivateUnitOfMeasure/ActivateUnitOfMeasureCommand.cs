namespace Insurance.InventoryService.AppCore.Shared.Catalog.UnitOfMeasures.Commands.ActivateUnitOfMeasure;

using OysterFx.AppCore.Shared.Commands;

public class ActivateUnitOfMeasureCommand : ICommand<ActivateUnitOfMeasureCommandResult>
{
    public Guid UnitOfMeasureBusinessKey { get; set; }
}
