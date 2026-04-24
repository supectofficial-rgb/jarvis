namespace Insurance.InventoryService.AppCore.Shared.Warehouse.Locations.Commands.DeleteLocation;

using OysterFx.AppCore.Shared.Commands;

public class DeleteLocationCommand : ICommand<DeleteLocationCommandResult>
{
    public Guid LocationBusinessKey { get; set; }
}
