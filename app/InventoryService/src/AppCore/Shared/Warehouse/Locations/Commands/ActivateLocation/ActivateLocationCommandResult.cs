namespace Insurance.InventoryService.AppCore.Shared.Warehouse.Locations.Commands.ActivateLocation;

public class ActivateLocationCommandResult
{
    public Guid LocationBusinessKey { get; set; }
    public bool IsActive { get; set; }
}
