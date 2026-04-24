namespace Insurance.InventoryService.AppCore.Shared.Warehouse.Locations.Commands.DeactivateLocation;

public class DeactivateLocationCommandResult
{
    public Guid LocationBusinessKey { get; set; }
    public bool IsActive { get; set; }
}
