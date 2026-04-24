namespace Insurance.InventoryService.AppCore.Shared.Warehouse.Warehouses.Commands.ActivateWarehouse;

public class ActivateWarehouseCommandResult
{
    public Guid WarehouseBusinessKey { get; set; }
    public bool IsActive { get; set; }
}
