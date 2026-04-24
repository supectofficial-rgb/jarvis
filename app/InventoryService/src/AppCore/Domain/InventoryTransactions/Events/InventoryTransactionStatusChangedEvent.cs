namespace Insurance.InventoryService.AppCore.Domain.InventoryTransactions.Events;

using Insurance.InventoryService.AppCore.Domain.InventoryTransactions.Entities;
using OysterFx.AppCore.Domain.Events;
using OysterFx.AppCore.Domain.ValueObjects;

public sealed record InventoryTransactionStatusChangedEvent : IDomainEvent
{
    public BusinessKey TransactionBusinessKey { get; }
    public InventoryTransactionStatus PreviousStatus { get; }
    public InventoryTransactionStatus CurrentStatus { get; }
    public Guid? ReversedTransactionRef { get; }
    public string? ReasonCode { get; }
    public DateTime OccurredOn { get; }

    public InventoryTransactionStatusChangedEvent(
        BusinessKey transactionBusinessKey,
        InventoryTransactionStatus previousStatus,
        InventoryTransactionStatus currentStatus,
        Guid? reversedTransactionRef,
        string? reasonCode)
    {
        TransactionBusinessKey = transactionBusinessKey;
        PreviousStatus = previousStatus;
        CurrentStatus = currentStatus;
        ReversedTransactionRef = reversedTransactionRef;
        ReasonCode = reasonCode;
        OccurredOn = DateTime.UtcNow;
    }
}
