namespace Insurance.InventoryService.AppCore.Domain.StockDetails.Events;

using OysterFx.AppCore.Domain.Events;
using OysterFx.AppCore.Domain.ValueObjects;

public sealed record StockDetailQuantityChangedEvent : IDomainEvent
{
    public BusinessKey StockDetailBusinessKey { get; }
    public decimal PreviousQuantityOnHand { get; }
    public decimal CurrentQuantityOnHand { get; }
    public DateTime OccurredAt { get; }

    public StockDetailQuantityChangedEvent(
        BusinessKey stockDetailBusinessKey,
        decimal previousQuantityOnHand,
        decimal currentQuantityOnHand,
        DateTime occurredAt)
    {
        StockDetailBusinessKey = stockDetailBusinessKey;
        PreviousQuantityOnHand = previousQuantityOnHand;
        CurrentQuantityOnHand = currentQuantityOnHand;
        OccurredAt = occurredAt;
    }
}
