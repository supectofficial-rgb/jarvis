namespace Insurance.InventoryService.AppCore.Domain.SourceTracing.Entities;

using OysterFx.AppCore.Domain.Aggregates;
using OysterFx.AppCore.Domain.Exceptions;

public sealed class InventorySourceAllocation : Aggregate
{
    public Guid SourceBalanceRef { get; private set; }
    public Guid ReservationRef { get; private set; }
    public Guid? ReservationAllocationRef { get; private set; }
    public Guid VariantRef { get; private set; }
    public decimal AllocatedQty { get; private set; }
    public Guid BaseUomRef { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public decimal ReleasedQty { get; private set; }
    public decimal ConsumedQty { get; private set; }
    public decimal ActiveAllocatedQty => AllocatedQty - ReleasedQty - ConsumedQty;

    private InventorySourceAllocation()
    {
    }

    internal static InventorySourceAllocation Create(
        Guid sourceBalanceRef,
        Guid reservationRef,
        Guid? reservationAllocationRef,
        Guid variantRef,
        decimal allocatedQty,
        Guid baseUomRef)
    {
        if (allocatedQty <= 0)
            throw new AggregateStateExceptions("Allocated quantity must be greater than zero.", nameof(allocatedQty));

        return new InventorySourceAllocation
        {
            SourceBalanceRef = sourceBalanceRef,
            ReservationRef = reservationRef,
            ReservationAllocationRef = reservationAllocationRef,
            VariantRef = variantRef,
            AllocatedQty = allocatedQty,
            BaseUomRef = baseUomRef,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void Release(decimal quantity)
    {
        if (quantity <= 0 || ReleasedQty + ConsumedQty + quantity > AllocatedQty)
            throw new AggregateStateExceptions("Release quantity is invalid.", nameof(quantity));

        ReleasedQty += quantity;
    }

    public void Consume(decimal quantity)
    {
        if (quantity <= 0 || ReleasedQty + ConsumedQty + quantity > AllocatedQty)
            throw new AggregateStateExceptions("Consume quantity is invalid.", nameof(quantity));

        ConsumedQty += quantity;
    }
}
