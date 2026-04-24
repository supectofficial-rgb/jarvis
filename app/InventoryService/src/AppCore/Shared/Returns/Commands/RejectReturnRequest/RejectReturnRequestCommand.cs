namespace Insurance.InventoryService.AppCore.Shared.Returns.Commands.RejectReturnRequest;

using OysterFx.AppCore.Shared.Commands;

public class RejectReturnRequestCommand : ICommand<Guid>
{
    public Guid ReturnRequestBusinessKey { get; set; }
    public string RejectedBy { get; set; } = string.Empty;
    public string? ReasonCode { get; set; }
}
