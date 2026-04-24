namespace Insurance.InventoryService.AppCore.Shared.Fulfillments.Commands.MarkFulfillmentPacked;

using OysterFx.AppCore.Shared.Commands;

public class MarkFulfillmentPackedCommand : ICommand<Guid>
{
    public Guid FulfillmentBusinessKey { get; set; }
    public string? ReasonCode { get; set; }
}
