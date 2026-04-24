namespace Insurance.InventoryService.AppCore.Shared.Warehouse.Warehouses.Commands.ActivateWarehouse;

using OysterFx.AppCore.Shared.Commands;

public class ActivateWarehouseCommand : ICommand<ActivateWarehouseCommandResult>
{
    public Guid WarehouseBusinessKey { get; set; }
}
