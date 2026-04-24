namespace Insurance.InventoryService.AppCore.Domain.SourceTracing.Entities;

using Insurance.InventoryService.AppCore.Domain.SourceTracing.Events;
using OysterFx.AppCore.Domain.Aggregates;
using OysterFx.AppCore.Domain.Exceptions;

public sealed class InventorySourceBalance : AggregateRoot
{
    private readonly List<InventorySourceAllocation> _allocations = new();
    private readonly List<InventorySourceConsumption> _consumptions = new();

    public InventorySourceType SourceType { get; private set; }
    public Guid? SourceDocumentRef { get; private set; }
    public Guid? SourceDocumentLineRef { get; private set; }
    public Guid? SourceTransactionRef { get; private set; }
    public Guid? SourceTransactionLineRef { get; private set; }
    public Guid VariantRef { get; private set; }
    public Guid SellerRef { get; private set; }
    public Guid WarehouseRef { get; private set; }
    public Guid LocationRef { get; private set; }
    public Guid QualityStatusRef { get; private set; }
    public string? LotBatchNo { get; private set; }
    public Guid? SerialRef { get; private set; }
    public decimal ReceivedQty { get; private set; }
    public decimal AllocatedQty { get; private set; }
    public decimal ConsumedQty { get; private set; }
    public decimal AvailableQty { get; private set; }
    public decimal RemainingQty { get; private set; }
    public Guid BaseUomRef { get; private set; }
    public InventorySourceBalanceStatus Status { get; private set; }
    public DateTime OpenedAt { get; private set; }
    public DateTime? ClosedAt { get; private set; }
    public DateTime? LastConsumedAt { get; private set; }
    public IReadOnlyCollection<InventorySourceAllocation> Allocations => _allocations.AsReadOnly();
    public IReadOnlyCollection<InventorySourceConsumption> Consumptions => _consumptions.AsReadOnly();

    private InventorySourceBalance()
    {
    }

    public static InventorySourceBalance Open(
        InventorySourceType sourceType,
        Guid variantRef,
        Guid sellerRef,
        Guid warehouseRef,
        Guid locationRef,
        Guid qualityStatusRef,
        Guid baseUomRef,
        decimal receivedQty,
        string? lotBatchNo = null,
        Guid? sourceDocumentRef = null,
        Guid? sourceDocumentLineRef = null,
        Guid? sourceTransactionRef = null,
        Guid? sourceTransactionLineRef = null,
        Guid? serialRef = null)
    {
        if (receivedQty <= 0)
            throw new ArgumentOutOfRangeException(nameof(receivedQty));

        return new InventorySourceBalance
        {
            SourceType = sourceType,
            VariantRef = variantRef,
            SellerRef = sellerRef,
            WarehouseRef = warehouseRef,
            LocationRef = locationRef,
            QualityStatusRef = qualityStatusRef,
            BaseUomRef = baseUomRef,
            LotBatchNo = Normalize(lotBatchNo),
            SourceDocumentRef = sourceDocumentRef,
            SourceDocumentLineRef = sourceDocumentLineRef,
            SourceTransactionRef = sourceTransactionRef,
            SourceTransactionLineRef = sourceTransactionLineRef,
            SerialRef = serialRef,
            ReceivedQty = receivedQty,
            AvailableQty = receivedQty,
            RemainingQty = receivedQty,
            Status = InventorySourceBalanceStatus.Open,
            OpenedAt = DateTime.UtcNow
        };
    }

    public InventorySourceAllocation Allocate(
        Guid reservationRef,
        Guid? reservationAllocationRef,
        decimal quantity)
    {
        if (Status != InventorySourceBalanceStatus.Open)
            throw new AggregateStateExceptions("Only open source balances can be allocated.", nameof(Status));

        if (quantity <= 0 || quantity > AvailableQty)
            throw new AggregateStateExceptions("Allocation quantity exceeds available source quantity.", nameof(quantity));

        var previousStatus = Status;

        var allocation = InventorySourceAllocation.Create(
            BusinessKey.Value,
            reservationRef,
            reservationAllocationRef,
            VariantRef,
            quantity,
            BaseUomRef);

        _allocations.Add(allocation);
        RefreshBalances();
        RaiseSnapshot(previousStatus, Status, "allocated");
        return allocation;
    }

    public void ReleaseAllocation(Guid allocationBusinessKey, decimal quantity)
    {
        var allocation = _allocations.FirstOrDefault(x => x.BusinessKey.Value == allocationBusinessKey);
        if (allocation is null)
            throw new InvalidOperationException("Allocation was not found.");

        var previousStatus = Status;

        allocation.Release(quantity);
        RefreshBalances();
        RaiseSnapshot(previousStatus, Status, "allocation_released");
    }

    public void ConsumeAllocated(Guid allocationBusinessKey, decimal quantity, Guid outboundTransactionRef, Guid? outboundTransactionLineRef, string? reasonCode = null)
    {
        var allocation = _allocations.FirstOrDefault(x => x.BusinessKey.Value == allocationBusinessKey);
        if (allocation is null)
            throw new InvalidOperationException("Allocation was not found.");

        allocation.Consume(quantity);
        AddConsumption(outboundTransactionRef, outboundTransactionLineRef, quantity, reasonCode, allocation.ReservationRef);
    }

    public void Consume(decimal quantity, Guid outboundTransactionRef, Guid? outboundTransactionLineRef = null, string? reasonCode = null)
    {
        AddConsumption(outboundTransactionRef, outboundTransactionLineRef, quantity, reasonCode, null);
    }

    private void ApplyConsumption(decimal quantity)
    {
        if (quantity <= 0 || quantity > RemainingQty)
            throw new AggregateStateExceptions("Invalid source consumption quantity.", nameof(quantity));

        var previousStatus = Status;

        ConsumedQty += quantity;
        RemainingQty = ReceivedQty - ConsumedQty;
        LastConsumedAt = DateTime.UtcNow;
        RefreshBalances();

        if (RemainingQty == 0)
        {
            Status = InventorySourceBalanceStatus.Closed;
            ClosedAt = DateTime.UtcNow;
        }
        else
        {
            Status = InventorySourceBalanceStatus.Open;
            ClosedAt = null;
        }

        RaiseSnapshot(previousStatus, Status, "consumed");
    }

    public void Cancel()
    {
        if (ConsumedQty > 0)
            throw new AggregateStateExceptions("Consumed source balance cannot be cancelled.", nameof(ConsumedQty));

        var previousStatus = Status;

        foreach (var allocation in _allocations.Where(x => x.ActiveAllocatedQty > 0))
            allocation.Release(allocation.ActiveAllocatedQty);

        RefreshBalances();
        Status = InventorySourceBalanceStatus.Cancelled;
        ClosedAt = DateTime.UtcNow;
        RaiseSnapshot(previousStatus, Status, "cancelled");
    }

    private InventorySourceConsumption AddConsumption(
        Guid outboundTransactionRef,
        Guid? outboundTransactionLineRef,
        decimal quantity,
        string? reasonCode,
        Guid? reservationRef)
    {
        ApplyConsumption(quantity);

        var consumption = InventorySourceConsumption.Create(
            outboundTransactionRef,
            outboundTransactionLineRef,
            BusinessKey.Value,
            SourceDocumentRef,
            SourceDocumentLineRef,
            SourceTransactionRef,
            SourceTransactionLineRef,
            VariantRef,
            SellerRef,
            WarehouseRef,
            LocationRef,
            QualityStatusRef,
            LotBatchNo,
            SerialRef,
            quantity,
            BaseUomRef,
            reservationRef,
            reasonCode);

        _consumptions.Add(consumption);
        return consumption;
    }

    private void RefreshBalances()
    {
        AllocatedQty = _allocations.Sum(x => x.ActiveAllocatedQty);
        AvailableQty = RemainingQty - AllocatedQty;

        if (AllocatedQty < 0)
            throw new AggregateStateExceptions("Allocated quantity cannot be negative.", nameof(AllocatedQty));

        if (ConsumedQty < 0)
            throw new AggregateStateExceptions("Consumed quantity cannot be negative.", nameof(ConsumedQty));

        if (AvailableQty < 0)
            throw new AggregateStateExceptions("Available quantity cannot be negative.", nameof(AvailableQty));

        if (RemainingQty < 0)
            throw new AggregateStateExceptions("Remaining quantity cannot be negative.", nameof(RemainingQty));

        if (AllocatedQty > ReceivedQty)
            throw new AggregateStateExceptions("Allocated quantity cannot exceed received quantity.", nameof(AllocatedQty));

        if (ConsumedQty > ReceivedQty)
            throw new AggregateStateExceptions("Consumed quantity cannot exceed received quantity.", nameof(ConsumedQty));
    }

    private void RaiseSnapshot(InventorySourceBalanceStatus previousStatus, InventorySourceBalanceStatus currentStatus, string? reasonCode)
    {
        AddEvent(new InventorySourceBalanceChangedEvent(
            BusinessKey,
            previousStatus,
            currentStatus,
            AvailableQty,
            RemainingQty,
            Normalize(reasonCode)));
    }

    private static string? Normalize(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
