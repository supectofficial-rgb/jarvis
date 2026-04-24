namespace Insurance.InventoryService.AppCore.Shared.Warehouse.Locations.Commands.MoveLocationToWarehouse;

using OysterFx.AppCore.Shared.Commands;

public class MoveLocationToWarehouseCommand : ICommand<MoveLocationToWarehouseCommandResult>
{
    public Guid LocationBusinessKey { get; set; }
    public Guid TargetWarehouseRef { get; set; }
}
