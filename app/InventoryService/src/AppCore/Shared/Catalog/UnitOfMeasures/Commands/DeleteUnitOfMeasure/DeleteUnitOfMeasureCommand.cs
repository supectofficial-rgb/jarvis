namespace Insurance.InventoryService.AppCore.Shared.Catalog.UnitOfMeasures.Commands.DeleteUnitOfMeasure;

using OysterFx.AppCore.Shared.Commands;

public class DeleteUnitOfMeasureCommand : ICommand<DeleteUnitOfMeasureCommandResult>
{
    public Guid UnitOfMeasureBusinessKey { get; set; }
}
