namespace Insurance.InventoryService.AppCore.Shared.Warehouse.Warehouses.Commands.DeactivateWarehouse;

public class DeactivateWarehouseCommandResult
{
    public Guid WarehouseBusinessKey { get; set; }
    public bool IsActive { get; set; }
}
