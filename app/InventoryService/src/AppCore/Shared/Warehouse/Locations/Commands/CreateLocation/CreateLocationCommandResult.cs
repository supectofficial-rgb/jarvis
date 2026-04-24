namespace Insurance.InventoryService.AppCore.Shared.Warehouse.Locations.Commands.CreateLocation;

public class CreateLocationCommandResult
{
    public Guid LocationBusinessKey { get; set; }
    public Guid WarehouseRef { get; set; }
    public string LocationCode { get; set; } = string.Empty;
    public string LocationType { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}
