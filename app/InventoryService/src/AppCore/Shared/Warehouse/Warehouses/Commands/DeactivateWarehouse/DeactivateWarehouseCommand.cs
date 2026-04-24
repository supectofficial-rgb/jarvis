namespace Insurance.InventoryService.AppCore.Shared.Warehouse.Warehouses.Commands.DeactivateWarehouse;

using OysterFx.AppCore.Shared.Commands;

public class DeactivateWarehouseCommand : ICommand<DeactivateWarehouseCommandResult>
{
    public Guid WarehouseBusinessKey { get; set; }
}
