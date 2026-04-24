namespace Insurance.InventoryService.AppCore.Shared.Returns.Commands.ReceiveReturnRequest;

using OysterFx.AppCore.Shared.Commands;

public class ReceiveReturnRequestCommand : ICommand<Guid>
{
    public Guid ReturnRequestBusinessKey { get; set; }
    public string ReceivedBy { get; set; } = string.Empty;
}
