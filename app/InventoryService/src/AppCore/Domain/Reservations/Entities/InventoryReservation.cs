namespace Insurance.InventoryService.AppCore.Domain.Reservations.Entities;

using Insurance.InventoryService.AppCore.Domain.Reservations.Events;
using OysterFx.AppCore.Domain.Aggregates;
using OysterFx.AppCore.Domain.Exceptions;

public sealed class InventoryReservation : AggregateRoot
{
    private readonly List<ReservationAllocation> _allocations = new();
    private readonly List<ReservationTransition> _transitions = new();

    public Guid OrderRef { get; private set; }
    public Guid OrderItemRef { get; private set; }
    public Guid VariantRef { get; private set; }
    public Guid SellerRef { get; private set; }
    public Guid WarehouseRef { get; private set; }
    public Guid ChannelRef { get; private set; }
    public decimal RequestedQuantity { get; private set; }
    public decimal AllocatedQuantity { get; private set; }
    public decimal ConsumedQuantity { get; private set; }
    public decimal ReleasedQuantity { get; private set; }
    public decimal RemainingQuantity => RequestedQuantity - ConsumedQuantity - ReleasedQuantity;
    public decimal AvailableToAllocate => RequestedQuantity - AllocatedQuantity - ConsumedQuantity - ReleasedQuantity;
    public InventoryReservationStatus Status { get; private set; }
    public DateTime? ExpiresAt { get; private set; }
    public DateTime? ConfirmedAt { get; private set; }
    public DateTime? ConsumedAt { get; private set; }
    public DateTime? ReleasedAt { get; private set; }
    public DateTime? RejectedAt { get; private set; }
    public string? CorrelationId { get; private set; }
    public string? IdempotencyKey { get; private set; }
    public string? ReasonCode { get; private set; }
    public IReadOnlyCollection<ReservationAllocation> Allocations => _allocations.AsReadOnly();
    public IReadOnlyCollection<ReservationTransition> Transitions => _transitions.AsReadOnly();

    private InventoryReservation()
    {
    }

    public static InventoryReservation Create(
        Guid orderRef,
        Guid orderItemRef,
        Guid variantRef,
        Guid sellerRef,
        Guid warehouseRef,
        Guid channelRef,
        decimal requestedQuantity,
        DateTime? expiresAt,
        string? correlationId = null,
        string? idempotencyKey = null,
        string? reasonCode = null)
    {
        if (requestedQuantity <= 0)
            throw new ArgumentOutOfRangeException(nameof(requestedQuantity));

        return new InventoryReservation
        {
            OrderRef = orderRef,
            OrderItemRef = orderItemRef,
            VariantRef = variantRef,
            SellerRef = sellerRef,
            WarehouseRef = warehouseRef,
            ChannelRef = channelRef,
            RequestedQuantity = requestedQuantity,
            Status = InventoryReservationStatus.Pending,
            ExpiresAt = expiresAt,
            CorrelationId = Normalize(correlationId),
            IdempotencyKey = Normalize(idempotencyKey),
            ReasonCode = Normalize(reasonCode)
        };
    }

    public void Confirm(decimal allocatedQuantity)
    {
        if (Status != InventoryReservationStatus.Pending)
            throw new AggregateStateExceptions("Only pending reservations can be confirmed.", nameof(Status));

        if (allocatedQuantity <= 0 || allocatedQuantity > RequestedQuantity)
            throw new AggregateStateExceptions("Allocated quantity is invalid.", nameof(allocatedQuantity));

        if (_allocations.Count > 0 && allocatedQuantity != _allocations.Sum(x => x.ActiveAllocatedQty))
            throw new AggregateStateExceptions("Confirmed quantity must match active allocations.", nameof(allocatedQuantity));

        AllocatedQuantity = allocatedQuantity;
        ConfirmedAt = DateTime.UtcNow;
        TransitionTo(InventoryReservationStatus.Confirmed, "confirmed");
    }

    public void Confirm()
    {
        if (_allocations.Count == 0)
            throw new AggregateStateExceptions("Reservation cannot be confirmed without allocations.", nameof(_allocations));

        Confirm(_allocations.Sum(x => x.ActiveAllocatedQty));
    }

    public ReservationAllocation AddAllocation(
        Guid? stockDetailRef,
        Guid warehouseRef,
        Guid locationRef,
        Guid qualityStatusRef,
        decimal allocatedQty,
        string? lotBatchNo = null,
        Guid? serialRef = null)
    {
        if (Status is InventoryReservationStatus.Released or InventoryReservationStatus.Expired or InventoryReservationStatus.Rejected or InventoryReservationStatus.Consumed)
            throw new AggregateStateExceptions("Reservation is not active for allocation.", nameof(Status));

        if (allocatedQty <= 0)
            throw new AggregateStateExceptions("Allocated quantity must be greater than zero.", nameof(allocatedQty));

        if (allocatedQty > AvailableToAllocate)
            throw new AggregateStateExceptions("Total allocated quantity cannot exceed requested quantity.", nameof(AllocatedQuantity));

        if (serialRef.HasValue && _allocations.Any(x => x.SerialRef == serialRef && x.IsActive))
            throw new AggregateStateExceptions("Serial is already allocated in this reservation.", nameof(serialRef));

        var allocation = ReservationAllocation.Create(
            BusinessKey.Value,
            stockDetailRef,
            VariantRef,
            warehouseRef,
            locationRef,
            qualityStatusRef,
            lotBatchNo,
            serialRef,
            allocatedQty);

        _allocations.Add(allocation);
        RecalculateQuantities();
        return allocation;
    }

    public void ReleaseAllocation(Guid allocationBusinessKey, decimal quantity)
    {
        var allocation = FindAllocation(allocationBusinessKey);
        allocation.Release(quantity);
        RecalculateQuantities();
    }

    public void Consume(decimal quantity, string? reasonCode = null)
    {
        if (Status != InventoryReservationStatus.Confirmed)
            throw new AggregateStateExceptions("Only confirmed reservations can be consumed.", nameof(Status));

        if (quantity <= 0 || ConsumedQuantity + quantity > AllocatedQuantity)
            throw new AggregateStateExceptions("Consumed quantity is invalid.", nameof(quantity));

        ConsumedQuantity += quantity;
        ReasonCode = Normalize(reasonCode) ?? ReasonCode;

        if (ConsumedQuantity >= AllocatedQuantity)
        {
            ConsumedAt = DateTime.UtcNow;
            TransitionTo(InventoryReservationStatus.Consumed, ReasonCode ?? "consumed");
        }
    }

    public void ConsumeAllocation(Guid allocationBusinessKey, decimal quantity, string? reasonCode = null)
    {
        if (Status != InventoryReservationStatus.Confirmed)
            throw new AggregateStateExceptions("Only confirmed reservations can be consumed.", nameof(Status));

        var allocation = FindAllocation(allocationBusinessKey);
        allocation.Consume(quantity);
        ConsumedQuantity = _allocations.Sum(x => x.ConsumedQty);
        ReasonCode = Normalize(reasonCode) ?? ReasonCode;

        if (ConsumedQuantity >= AllocatedQuantity || _allocations.All(x => !x.IsActive))
        {
            ConsumedAt = DateTime.UtcNow;
            TransitionTo(InventoryReservationStatus.Consumed, ReasonCode ?? "consumed");
        }
    }

    public void Release(string? reasonCode = null)
    {
        if (Status != InventoryReservationStatus.Confirmed)
            throw new AggregateStateExceptions("Only confirmed reservations can be released.", nameof(Status));

        if (ConsumedQuantity > 0)
            throw new AggregateStateExceptions("Consumed reservation cannot be released.", nameof(ConsumedQuantity));

        foreach (var allocation in _allocations.Where(x => x.IsActive))
            allocation.Release(allocation.AllocatedQty - allocation.ReleasedQty);

        RecalculateQuantities();
        ReleasedAt = DateTime.UtcNow;
        ReasonCode = Normalize(reasonCode) ?? ReasonCode;
        TransitionTo(InventoryReservationStatus.Released, ReasonCode ?? "released");
    }

    public void Reject(string? reasonCode = null)
    {
        if (Status != InventoryReservationStatus.Pending)
            throw new AggregateStateExceptions("Only pending reservations can be rejected.", nameof(Status));

        RejectedAt = DateTime.UtcNow;
        ReasonCode = Normalize(reasonCode) ?? ReasonCode;
        TransitionTo(InventoryReservationStatus.Rejected, ReasonCode ?? "rejected");
    }

    public void Expire(string? reasonCode = null)
    {
        if (Status != InventoryReservationStatus.Confirmed)
            throw new AggregateStateExceptions("Only confirmed reservations can expire.", nameof(Status));

        if (ConsumedQuantity > 0)
            throw new AggregateStateExceptions("Consumed reservation cannot be expired.", nameof(ConsumedQuantity));

        foreach (var allocation in _allocations.Where(x => x.IsActive))
            allocation.Release(allocation.AllocatedQty - allocation.ReleasedQty);

        RecalculateQuantities();
        ReasonCode = Normalize(reasonCode) ?? ReasonCode;
        TransitionTo(InventoryReservationStatus.Expired, ReasonCode ?? "expired");
    }

    private ReservationAllocation FindAllocation(Guid allocationBusinessKey)
    {
        var allocation = _allocations.FirstOrDefault(x => x.BusinessKey.Value == allocationBusinessKey);
        if (allocation is null)
            throw new AggregateStateExceptions("Reservation allocation was not found.", nameof(allocationBusinessKey));

        return allocation;
    }

    private void RecalculateQuantities()
    {
        AllocatedQuantity = _allocations.Sum(x => x.ActiveAllocatedQty);
        ReleasedQuantity = _allocations.Sum(x => x.ReleasedQty);
        ConsumedQuantity = _allocations.Sum(x => x.ConsumedQty);
    }

    private void TransitionTo(InventoryReservationStatus nextStatus, string? reasonCode)
    {
        var previousStatus = Status;
        Status = nextStatus;
        AddTransition(previousStatus, nextStatus, reasonCode);
        AddEvent(new InventoryReservationStatusChangedEvent(BusinessKey, previousStatus, nextStatus, Normalize(reasonCode)));
    }

    private void AddTransition(InventoryReservationStatus fromStatus, InventoryReservationStatus toStatus, string? reasonCode)
    {
        _transitions.Add(ReservationTransition.Create(BusinessKey.Value, fromStatus, toStatus, reasonCode));
    }

    private static string? Normalize(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
