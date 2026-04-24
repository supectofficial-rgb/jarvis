namespace Insurance.InventoryService.AppCore.Shared.Warehouse.Locations.Commands.UpdateLocation;

public class UpdateLocationCommandResult
{
    public Guid LocationBusinessKey { get; set; }
    public string LocationCode { get; set; } = string.Empty;
    public string LocationType { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}
