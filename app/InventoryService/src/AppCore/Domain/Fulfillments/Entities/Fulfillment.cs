namespace Insurance.InventoryService.AppCore.Domain.Fulfillments.Entities;

using Insurance.InventoryService.AppCore.Domain.Fulfillments.Events;
using OysterFx.AppCore.Domain.Aggregates;

public sealed class Fulfillment : AggregateRoot
{
    private readonly List<FulfillmentLine> _lines = new();
    private readonly List<FulfillmentTransition> _transitions = new();

    public Guid OrderRef { get; private set; }
    public Guid SellerRef { get; private set; }
    public Guid WarehouseRef { get; private set; }
    public Guid ChannelRef { get; private set; }
    public FulfillmentStatus Status { get; private set; }
    public DateTime? PickedAt { get; private set; }
    public DateTime? PackedAt { get; private set; }
    public DateTime? ShippedAt { get; private set; }
    public DateTime? ReturnedAt { get; private set; }
    public DateTime? CancelledAt { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }
    public IReadOnlyCollection<FulfillmentLine> Lines => _lines.AsReadOnly();
    public IReadOnlyCollection<FulfillmentTransition> Transitions => _transitions.AsReadOnly();

    private Fulfillment()
    {
    }

    public static Fulfillment Create(Guid orderRef, Guid sellerRef, Guid warehouseRef, Guid channelRef)
    {
        return new Fulfillment
        {
            OrderRef = orderRef,
            SellerRef = sellerRef,
            WarehouseRef = warehouseRef,
            ChannelRef = channelRef,
            Status = FulfillmentStatus.Pending,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    public FulfillmentLine AddLine(
        Guid variantRef,
        decimal qty,
        Guid uomRef,
        decimal baseQty,
        Guid baseUomRef,
        Guid? sourceLocationRef = null,
        string? lotBatchNo = null,
        Guid? reservationAllocationRef = null)
    {
        if (Status != FulfillmentStatus.Pending)
            throw new InvalidOperationException("Lines can only be added while fulfillment is pending.");

        var line = FulfillmentLine.Create(
            BusinessKey.Value,
            variantRef,
            qty,
            uomRef,
            baseQty,
            baseUomRef,
            sourceLocationRef,
            lotBatchNo,
            reservationAllocationRef);

        _lines.Add(line);
        return line;
    }

    public void MarkPicked(string? reasonCode = null)
    {
        EnsureStatus(FulfillmentStatus.Pending);
        if (_lines.Count == 0)
            throw new InvalidOperationException("Fulfillment must contain at least one line.");

        foreach (var line in _lines)
            line.MarkPicked();

        TransitionTo(FulfillmentStatus.Picked, reasonCode, now => PickedAt = now);
    }

    public void MarkPacked(string? reasonCode = null)
    {
        EnsureStatus(FulfillmentStatus.Picked);
        foreach (var line in _lines)
            line.MarkPacked();

        TransitionTo(FulfillmentStatus.Packed, reasonCode, now => PackedAt = now);
    }

    public void MarkShipped(string? reasonCode = null)
    {
        EnsureStatus(FulfillmentStatus.Packed);
        foreach (var line in _lines)
            line.MarkShipped();

        TransitionTo(FulfillmentStatus.Shipped, reasonCode, now => ShippedAt = now);
    }

    public void MarkReturned(bool partial, string? reasonCode = null)
    {
        EnsureStatus(FulfillmentStatus.Shipped);
        TransitionTo(partial ? FulfillmentStatus.PartiallyReturned : FulfillmentStatus.Returned, reasonCode, now => ReturnedAt = now);
    }

    public void Cancel(string? reasonCode = null)
    {
        if (Status is FulfillmentStatus.Shipped or FulfillmentStatus.Returned or FulfillmentStatus.PartiallyReturned)
            throw new InvalidOperationException("Shipped or returned fulfillment cannot be cancelled.");

        TransitionTo(FulfillmentStatus.Cancelled, reasonCode, now => CancelledAt = now);
    }

    private void EnsureStatus(FulfillmentStatus expected)
    {
        if (Status != expected)
            throw new InvalidOperationException($"Fulfillment must be in {expected} state.");
    }

    private void TransitionTo(FulfillmentStatus next, string? reasonCode, Action<DateTime>? timestampSetter = null)
    {
        var previous = Status;
        Status = next;
        UpdatedAt = DateTime.UtcNow;
        timestampSetter?.Invoke(UpdatedAt);
        _transitions.Add(FulfillmentTransition.Create(BusinessKey.Value, previous, next, reasonCode));
        AddEvent(new FulfillmentStatusChangedEvent(BusinessKey, previous, next, Normalize(reasonCode)));
    }

    private static string? Normalize(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
