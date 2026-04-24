namespace Insurance.InventoryService.AppCore.Domain.SourceTracing.Events;

using Insurance.InventoryService.AppCore.Domain.SourceTracing.Entities;
using OysterFx.AppCore.Domain.Events;
using OysterFx.AppCore.Domain.ValueObjects;

public sealed record InventorySourceBalanceChangedEvent : IDomainEvent
{
    public BusinessKey SourceBalanceBusinessKey { get; }
    public InventorySourceBalanceStatus PreviousStatus { get; }
    public InventorySourceBalanceStatus CurrentStatus { get; }
    public decimal AvailableQty { get; }
    public decimal RemainingQty { get; }
    public string? ReasonCode { get; }
    public DateTime OccurredOn { get; }

    public InventorySourceBalanceChangedEvent(
        BusinessKey sourceBalanceBusinessKey,
        InventorySourceBalanceStatus previousStatus,
        InventorySourceBalanceStatus currentStatus,
        decimal availableQty,
        decimal remainingQty,
        string? reasonCode)
    {
        SourceBalanceBusinessKey = sourceBalanceBusinessKey;
        PreviousStatus = previousStatus;
        CurrentStatus = currentStatus;
        AvailableQty = availableQty;
        RemainingQty = remainingQty;
        ReasonCode = reasonCode;
        OccurredOn = DateTime.UtcNow;
    }
}
