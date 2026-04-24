namespace Insurance.InventoryService.AppCore.Shared.SerialItems.Commands.UpdateSerialItemStatus;

using OysterFx.AppCore.Shared.Commands;

public class UpdateSerialItemStatusCommand : ICommand<UpdateSerialItemStatusCommandResult>
{
    public Guid SerialItemBusinessKey { get; set; }
    public string Status { get; set; } = string.Empty;
    public Guid? LastTransactionRef { get; set; }
    public Guid? QualityStatusRef { get; set; }
    public Guid? WarehouseRef { get; set; }
    public Guid? LocationRef { get; set; }
    public string? LotBatchNo { get; set; }
}
