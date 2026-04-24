namespace Insurance.InventoryService.AppCore.Shared.Returns.Commands.ApproveReturnRequest;

using OysterFx.AppCore.Shared.Commands;

public class ApproveReturnRequestCommand : ICommand<Guid>
{
    public Guid ReturnRequestBusinessKey { get; set; }
    public string ApprovedBy { get; set; } = string.Empty;
}
