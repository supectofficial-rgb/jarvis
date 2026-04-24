namespace Insurance.InventoryService.AppCore.Shared.Catalog.UnitOfMeasures.Commands.CreateUnitOfMeasure;

using OysterFx.AppCore.Shared.Commands;

public class CreateUnitOfMeasureCommand : ICommand<CreateUnitOfMeasureCommandResult>
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public int Precision { get; set; }
}
