namespace Insurance.InventoryService.AppCore.Shared.Warehouse.Warehouses.Commands.CreateWarehouse;

using OysterFx.AppCore.Shared.Commands;

public class CreateWarehouseCommand : ICommand<CreateWarehouseCommandResult>
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
}
