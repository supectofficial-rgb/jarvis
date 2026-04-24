namespace Insurance.InventoryService.AppCore.Shared.Returns.Commands.CreateReturnRequest;

using Insurance.InventoryService.AppCore.Shared.Returns.Commands;
using OysterFx.AppCore.Shared.Commands;

public class CreateReturnRequestCommand : ICommand<Guid>
{
    public Guid OrderRef { get; set; }
    public Guid OrderItemRef { get; set; }
    public Guid SellerRef { get; set; }
    public Guid WarehouseRef { get; set; }
    public string? ReasonCode { get; set; }
    public List<ReturnRequestCommandLineItem> Lines { get; set; } = new();
}
