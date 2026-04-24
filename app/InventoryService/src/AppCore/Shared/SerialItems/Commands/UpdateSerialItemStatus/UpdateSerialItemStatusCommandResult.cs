namespace Insurance.InventoryService.AppCore.Shared.SerialItems.Commands.UpdateSerialItemStatus;

public class UpdateSerialItemStatusCommandResult
{
    public Guid SerialItemBusinessKey { get; set; }
    public string Status { get; set; } = string.Empty;
}
