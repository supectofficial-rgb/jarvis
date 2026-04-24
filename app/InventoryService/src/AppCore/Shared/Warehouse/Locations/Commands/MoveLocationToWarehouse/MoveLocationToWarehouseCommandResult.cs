namespace Insurance.InventoryService.AppCore.Shared.Warehouse.Locations.Commands.MoveLocationToWarehouse;

public class MoveLocationToWarehouseCommandResult
{
    public Guid LocationBusinessKey { get; set; }
    public Guid WarehouseRef { get; set; }
}
