namespace Insurance.InventoryService.AppCore.Shared.Fulfillments.Commands.MarkFulfillmentShipped;

using OysterFx.AppCore.Shared.Commands;

public class MarkFulfillmentShippedCommand : ICommand<Guid>
{
    public Guid FulfillmentBusinessKey { get; set; }
    public string? ReasonCode { get; set; }
}
