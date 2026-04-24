namespace Insurance.InventoryService.AppCore.Shared.Fulfillments.Commands.MarkFulfillmentReturned;

using OysterFx.AppCore.Shared.Commands;

public class MarkFulfillmentReturnedCommand : ICommand<Guid>
{
    public Guid FulfillmentBusinessKey { get; set; }
    public bool Partial { get; set; }
    public string? ReasonCode { get; set; }
}
