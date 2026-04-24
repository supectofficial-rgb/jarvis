namespace Insurance.InventoryService.AppCore.Domain.Reservations.Entities;

using OysterFx.AppCore.Domain.Aggregates;
using OysterFx.AppCore.Domain.Exceptions;

public sealed class ReservationAllocation : Aggregate
{
    public Guid ReservationRef { get; private set; }
    public Guid? StockDetailRef { get; private set; }
    public Guid VariantRef { get; private set; }
    public Guid WarehouseRef { get; private set; }
    public Guid LocationRef { get; private set; }
    public Guid QualityStatusRef { get; private set; }
    public string? LotBatchNo { get; private set; }
    public Guid? SerialRef { get; private set; }
    public decimal AllocatedQty { get; private set; }
    public DateTime AllocatedAt { get; private set; }
    public decimal ReleasedQty { get; private set; }
    public decimal ConsumedQty { get; private set; }
    public decimal ActiveAllocatedQty => AllocatedQty - ReleasedQty - ConsumedQty;
    public bool IsActive => ReleasedQty < AllocatedQty && ConsumedQty < AllocatedQty;

    private ReservationAllocation()
    {
    }

    internal static ReservationAllocation Create(
        Guid reservationRef,
        Guid? stockDetailRef,
        Guid variantRef,
        Guid warehouseRef,
        Guid locationRef,
        Guid qualityStatusRef,
        string? lotBatchNo,
        Guid? serialRef,
        decimal allocatedQty)
    {
        return new ReservationAllocation
        {
            ReservationRef = reservationRef,
            StockDetailRef = stockDetailRef,
            VariantRef = variantRef,
            WarehouseRef = warehouseRef,
            LocationRef = locationRef,
            QualityStatusRef = qualityStatusRef,
            LotBatchNo = lotBatchNo,
            SerialRef = serialRef,
            AllocatedQty = allocatedQty,
            AllocatedAt = DateTime.UtcNow
        };
    }

    public void Release(decimal quantity)
    {
        if (quantity <= 0 || ReleasedQty + quantity > AllocatedQty)
            throw new AggregateStateExceptions("Release quantity is invalid.", nameof(quantity));

        ReleasedQty += quantity;
    }

    public void Consume(decimal quantity)
    {
        if (quantity <= 0 || ConsumedQty + ReleasedQty + quantity > AllocatedQty)
            throw new AggregateStateExceptions("Consume quantity is invalid.", nameof(quantity));

        ConsumedQty += quantity;
    }
}
