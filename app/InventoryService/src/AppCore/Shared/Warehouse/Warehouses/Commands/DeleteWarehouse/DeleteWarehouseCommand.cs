namespace Insurance.InventoryService.AppCore.Shared.Warehouse.Warehouses.Commands.DeleteWarehouse;

using OysterFx.AppCore.Shared.Commands;

public class DeleteWarehouseCommand : ICommand<DeleteWarehouseCommandResult>
{
    public Guid WarehouseBusinessKey { get; set; }
}
