namespace Insurance.InventoryService.AppCore.Domain.Fulfillments.Entities;

using OysterFx.AppCore.Domain.Aggregates;

public sealed class FulfillmentTransition : Aggregate
{
    public Guid FulfillmentRef { get; private set; }
    public FulfillmentStatus FromStatus { get; private set; }
    public FulfillmentStatus ToStatus { get; private set; }
    public string? ReasonCode { get; private set; }
    public DateTime ChangedAt { get; private set; }

    private FulfillmentTransition()
    {
    }

    internal static FulfillmentTransition Create(
        Guid fulfillmentRef,
        FulfillmentStatus fromStatus,
        FulfillmentStatus toStatus,
        string? reasonCode)
    {
        return new FulfillmentTransition
        {
            FulfillmentRef = fulfillmentRef,
            FromStatus = fromStatus,
            ToStatus = toStatus,
            ReasonCode = reasonCode,
            ChangedAt = DateTime.UtcNow
        };
    }
}
