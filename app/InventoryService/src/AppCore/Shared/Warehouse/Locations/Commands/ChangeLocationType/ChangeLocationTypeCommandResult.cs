namespace Insurance.InventoryService.AppCore.Shared.Warehouse.Locations.Commands.ChangeLocationType;

public class ChangeLocationTypeCommandResult
{
    public Guid LocationBusinessKey { get; set; }
    public string LocationType { get; set; } = string.Empty;
}
