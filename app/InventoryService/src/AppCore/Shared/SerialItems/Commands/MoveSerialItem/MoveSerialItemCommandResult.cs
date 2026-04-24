namespace Insurance.InventoryService.AppCore.Shared.SerialItems.Commands.MoveSerialItem;

public class MoveSerialItemCommandResult
{
    public Guid SerialItemBusinessKey { get; set; }
    public Guid WarehouseRef { get; set; }
    public Guid LocationRef { get; set; }
    public Guid QualityStatusRef { get; set; }
}
