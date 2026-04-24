namespace Insurance.InventoryService.AppCore.Shared.Warehouse.Locations.Commands.ActivateLocation;

using OysterFx.AppCore.Shared.Commands;

public class ActivateLocationCommand : ICommand<ActivateLocationCommandResult>
{
    public Guid LocationBusinessKey { get; set; }
}
