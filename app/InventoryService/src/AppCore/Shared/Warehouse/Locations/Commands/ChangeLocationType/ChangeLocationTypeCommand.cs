namespace Insurance.InventoryService.AppCore.Shared.Warehouse.Locations.Commands.ChangeLocationType;

using OysterFx.AppCore.Shared.Commands;

public class ChangeLocationTypeCommand : ICommand<ChangeLocationTypeCommandResult>
{
    public Guid LocationBusinessKey { get; set; }
    public string LocationType { get; set; } = string.Empty;
}
