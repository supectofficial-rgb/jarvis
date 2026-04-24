namespace Insurance.InventoryService.AppCore.Domain.InventoryTransactions.Entities;

using Insurance.InventoryService.AppCore.Domain.InventoryTransactions.Events;
using OysterFx.AppCore.Domain.Aggregates;
using OysterFx.AppCore.Domain.Exceptions;

public sealed class InventoryTransaction : AggregateRoot
{
    public string TransactionNo { get; private set; } = string.Empty;
    public InventoryTransactionType TransactionType { get; private set; }
    public string? ReferenceType { get; private set; }
    public Guid? ReferenceBusinessId { get; private set; }
    public Guid WarehouseRef { get; private set; }
    public Guid SellerRef { get; private set; }
    public DateTime OccurredAt { get; private set; }
    public DateTime? PostedAt { get; private set; }
    public InventoryTransactionStatus Status { get; private set; }
    public string? CorrelationId { get; private set; }
    public string? IdempotencyKey { get; private set; }
    public string? ReasonCode { get; private set; }
    public Guid? ReversedTransactionRef { get; private set; }
    public List<InventoryTransactionLine> Lines { get; private set; } = new();

    private InventoryTransaction()
    {
    }

    private InventoryTransaction(
        string transactionNo,
        InventoryTransactionType transactionType,
        Guid warehouseRef,
        Guid sellerRef,
        DateTime occurredAt,
        string? referenceType,
        Guid? referenceBusinessId,
        string? correlationId,
        string? idempotencyKey,
        string? reasonCode)
    {
        if (string.IsNullOrWhiteSpace(transactionNo))
            throw new ArgumentException("Transaction number is required.", nameof(transactionNo));

        TransactionNo = transactionNo.Trim();
        TransactionType = transactionType;
        WarehouseRef = warehouseRef;
        SellerRef = sellerRef;
        OccurredAt = occurredAt;
        ReferenceType = Normalize(referenceType);
        ReferenceBusinessId = referenceBusinessId;
        CorrelationId = Normalize(correlationId);
        IdempotencyKey = Normalize(idempotencyKey);
        ReasonCode = Normalize(reasonCode);
        Status = InventoryTransactionStatus.Draft;
    }

    public static InventoryTransaction Create(
        string transactionNo,
        InventoryTransactionType transactionType,
        Guid warehouseRef,
        Guid sellerRef,
        DateTime occurredAt,
        string? referenceType = null,
        Guid? referenceBusinessId = null,
        string? correlationId = null,
        string? idempotencyKey = null,
        string? reasonCode = null)
    {
        return new InventoryTransaction(
            transactionNo,
            transactionType,
            warehouseRef,
            sellerRef,
            occurredAt,
            referenceType,
            referenceBusinessId,
            correlationId,
            idempotencyKey,
            reasonCode);
    }

    public void AddLine(InventoryTransactionLine line)
    {
        ArgumentNullException.ThrowIfNull(line);

        if (Status != InventoryTransactionStatus.Draft)
            throw new AggregateStateExceptions("Only draft transactions can be edited.", nameof(Status));

        Lines.Add(line);
    }

    public void MarkPosted()
    {
        if (Status != InventoryTransactionStatus.Draft)
            throw new AggregateStateExceptions("Only draft transactions can be posted.", nameof(Status));

        if (Lines.Count == 0)
            throw new AggregateStateExceptions("Transaction must contain at least one line.", nameof(Lines));

        PostedAt = DateTime.UtcNow;
        ChangeStatus(InventoryTransactionStatus.Posted, ReasonCode, null);
    }

    public void CancelDraft(string? reasonCode = null)
    {
        if (Status != InventoryTransactionStatus.Draft)
            throw new AggregateStateExceptions("Only draft transactions can be cancelled.", nameof(Status));

        ReasonCode = Normalize(reasonCode) ?? ReasonCode;
        ChangeStatus(InventoryTransactionStatus.Cancelled, ReasonCode, null);
    }

    public void MarkReversed(Guid reversedTransactionRef, string? reasonCode = null)
    {
        if (Status != InventoryTransactionStatus.Posted)
            throw new AggregateStateExceptions("Only posted transactions can be reversed.", nameof(Status));

        ReversedTransactionRef = reversedTransactionRef;
        ReasonCode = Normalize(reasonCode) ?? ReasonCode;
        ChangeStatus(InventoryTransactionStatus.Reversed, ReasonCode, reversedTransactionRef);
    }

    private void ChangeStatus(InventoryTransactionStatus next, string? reasonCode, Guid? reversedTransactionRef)
    {
        var previous = Status;
        Status = next;
        AddEvent(new InventoryTransactionStatusChangedEvent(BusinessKey, previous, next, reversedTransactionRef, Normalize(reasonCode)));
    }

    private static string? Normalize(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
