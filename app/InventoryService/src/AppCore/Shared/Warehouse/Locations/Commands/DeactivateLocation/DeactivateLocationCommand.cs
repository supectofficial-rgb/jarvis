namespace Insurance.InventoryService.AppCore.Shared.Warehouse.Locations.Commands.DeactivateLocation;

using OysterFx.AppCore.Shared.Commands;

public class DeactivateLocationCommand : ICommand<DeactivateLocationCommandResult>
{
    public Guid LocationBusinessKey { get; set; }
}
