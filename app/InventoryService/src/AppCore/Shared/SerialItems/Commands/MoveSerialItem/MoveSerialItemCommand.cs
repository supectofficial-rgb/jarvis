namespace Insurance.InventoryService.AppCore.Shared.SerialItems.Commands.MoveSerialItem;

using OysterFx.AppCore.Shared.Commands;

public class MoveSerialItemCommand : ICommand<MoveSerialItemCommandResult>
{
    public Guid SerialItemBusinessKey { get; set; }
    public Guid WarehouseRef { get; set; }
    public Guid LocationRef { get; set; }
    public Guid QualityStatusRef { get; set; }
    public string? LotBatchNo { get; set; }
}
