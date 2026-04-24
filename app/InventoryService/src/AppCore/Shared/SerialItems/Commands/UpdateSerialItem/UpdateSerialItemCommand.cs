namespace Insurance.InventoryService.AppCore.Shared.SerialItems.Commands.UpdateSerialItem;

using OysterFx.AppCore.Shared.Commands;

public class UpdateSerialItemCommand : ICommand<UpdateSerialItemCommandResult>
{
    public Guid SerialItemBusinessKey { get; set; }
    public Guid WarehouseRef { get; set; }
    public Guid LocationRef { get; set; }
    public Guid QualityStatusRef { get; set; }
    public string? LotBatchNo { get; set; }
    public string Status { get; set; } = string.Empty;
}
