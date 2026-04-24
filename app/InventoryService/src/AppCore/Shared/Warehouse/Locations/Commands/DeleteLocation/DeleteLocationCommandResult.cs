namespace Insurance.InventoryService.AppCore.Shared.Warehouse.Locations.Commands.DeleteLocation;

public class DeleteLocationCommandResult
{
    public Guid LocationBusinessKey { get; set; }
    public bool Deleted { get; set; }
}
