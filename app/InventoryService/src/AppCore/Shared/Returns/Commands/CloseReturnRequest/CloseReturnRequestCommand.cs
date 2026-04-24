namespace Insurance.InventoryService.AppCore.Shared.Returns.Commands.CloseReturnRequest;

using OysterFx.AppCore.Shared.Commands;

public class CloseReturnRequestCommand : ICommand<Guid>
{
    public Guid ReturnRequestBusinessKey { get; set; }
}
