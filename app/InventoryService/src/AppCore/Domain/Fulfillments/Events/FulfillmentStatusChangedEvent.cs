namespace Insurance.InventoryService.AppCore.Domain.Fulfillments.Events;

using Insurance.InventoryService.AppCore.Domain.Fulfillments.Entities;
using OysterFx.AppCore.Domain.Events;
using OysterFx.AppCore.Domain.ValueObjects;

public sealed record FulfillmentStatusChangedEvent : IDomainEvent
{
    public BusinessKey FulfillmentBusinessKey { get; }
    public FulfillmentStatus PreviousStatus { get; }
    public FulfillmentStatus CurrentStatus { get; }
    public string? ReasonCode { get; }
    public DateTime OccurredOn { get; }

    public FulfillmentStatusChangedEvent(
        BusinessKey fulfillmentBusinessKey,
        FulfillmentStatus previousStatus,
        FulfillmentStatus currentStatus,
        string? reasonCode)
    {
        FulfillmentBusinessKey = fulfillmentBusinessKey;
        PreviousStatus = previousStatus;
        CurrentStatus = currentStatus;
        ReasonCode = reasonCode;
        OccurredOn = DateTime.UtcNow;
    }
}
