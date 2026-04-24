namespace Insurance.InventoryService.AppCore.Shared.Fulfillments.Commands.MarkFulfillmentPicked;

using OysterFx.AppCore.Shared.Commands;

public class MarkFulfillmentPickedCommand : ICommand<Guid>
{
    public Guid FulfillmentBusinessKey { get; set; }
    public string? ReasonCode { get; set; }
}
