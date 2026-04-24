namespace Insurance.InventoryService.AppCore.Shared.Warehouse.Warehouses.Commands.UpdateWarehouse;

public class UpdateWarehouseCommandResult
{
    public Guid WarehouseBusinessKey { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}
