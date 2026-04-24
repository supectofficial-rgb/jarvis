namespace Insurance.InventoryService.AppCore.Shared.Warehouse.Warehouses.Commands.DeleteWarehouse;

public class DeleteWarehouseCommandResult
{
    public Guid WarehouseBusinessKey { get; set; }
    public bool Deleted { get; set; }
}
