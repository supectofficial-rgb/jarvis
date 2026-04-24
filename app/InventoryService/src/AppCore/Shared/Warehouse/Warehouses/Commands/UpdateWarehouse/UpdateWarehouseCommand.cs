namespace Insurance.InventoryService.AppCore.Shared.Warehouse.Warehouses.Commands.UpdateWarehouse;

using OysterFx.AppCore.Shared.Commands;

public class UpdateWarehouseCommand : ICommand<UpdateWarehouseCommandResult>
{
    public Guid WarehouseBusinessKey { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
}
