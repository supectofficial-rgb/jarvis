namespace Insurance.InventoryService.AppCore.Domain.Returns.Entities;

using OysterFx.AppCore.Domain.Aggregates;

public sealed class ReturnRequestTransition : Aggregate
{
    public Guid ReturnRequestRef { get; private set; }
    public ReturnRequestStatus FromStatus { get; private set; }
    public ReturnRequestStatus ToStatus { get; private set; }
    public string? ReasonCode { get; private set; }
    public DateTime ChangedAt { get; private set; }

    private ReturnRequestTransition()
    {
    }

    internal static ReturnRequestTransition Create(
        Guid returnRequestRef,
        ReturnRequestStatus fromStatus,
        ReturnRequestStatus toStatus,
        string? reasonCode)
    {
        return new ReturnRequestTransition
        {
            ReturnRequestRef = returnRequestRef,
            FromStatus = fromStatus,
            ToStatus = toStatus,
            ReasonCode = reasonCode,
            ChangedAt = DateTime.UtcNow
        };
    }
}
