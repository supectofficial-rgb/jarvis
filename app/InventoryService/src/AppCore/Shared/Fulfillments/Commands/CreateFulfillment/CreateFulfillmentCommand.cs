namespace Insurance.InventoryService.AppCore.Shared.Fulfillments.Commands.CreateFulfillment;

using Insurance.InventoryService.AppCore.Shared.Fulfillments.Commands;
using OysterFx.AppCore.Shared.Commands;

public class CreateFulfillmentCommand : ICommand<Guid>
{
    public Guid OrderRef { get; set; }
    public Guid SellerRef { get; set; }
    public Guid WarehouseRef { get; set; }
    public Guid ChannelRef { get; set; }
    public List<FulfillmentCommandLineItem> Lines { get; set; } = new();
}
