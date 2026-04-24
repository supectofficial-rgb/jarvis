namespace Insurance.InventoryService.AppCore.Shared.SerialItems.Commands.CreateSerialItem;

public class CreateSerialItemCommandResult
{
    public Guid SerialItemBusinessKey { get; set; }
    public string SerialNo { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
}
