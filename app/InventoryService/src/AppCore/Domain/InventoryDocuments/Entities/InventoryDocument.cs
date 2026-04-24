namespace Insurance.InventoryService.AppCore.Domain.InventoryDocuments.Entities;

using Insurance.InventoryService.AppCore.Domain.InventoryDocuments.Events;
using OysterFx.AppCore.Domain.Aggregates;
using OysterFx.AppCore.Domain.Exceptions;
using OysterFx.AppCore.Domain.ValueObjects;

public sealed class InventoryDocument : AggregateRoot
{
    public string DocumentNo { get; private set; } = string.Empty;
    public InventoryDocumentType DocumentType { get; private set; }
    public InventoryDocumentStatus Status { get; private set; }
    public string? ReferenceType { get; private set; }
    public Guid? ReferenceBusinessId { get; private set; }
    public Guid WarehouseRef { get; private set; }
    public Guid SellerRef { get; private set; }
    public DateTime OccurredAt { get; private set; }
    public DateTime? ApprovedAt { get; private set; }
    public string? ApprovedBy { get; private set; }
    public DateTime? PostedAt { get; private set; }
    public string? PostedBy { get; private set; }
    public Guid? PostedTransactionRef { get; private set; }
    public DateTime? CancelledAt { get; private set; }
    public DateTime? RejectedAt { get; private set; }
    public string? CorrelationId { get; private set; }
    public string? IdempotencyKey { get; private set; }
    public string? ReasonCode { get; private set; }
    public List<InventoryDocumentLine> Lines { get; private set; } = new();

    private InventoryDocument()
    {
    }

    private InventoryDocument(
        string documentNo,
        InventoryDocumentType documentType,
        Guid warehouseRef,
        Guid sellerRef,
        DateTime occurredAt,
        string? referenceType,
        Guid? referenceBusinessId,
        string? correlationId,
        string? idempotencyKey,
        string? reasonCode)
    {
        if (string.IsNullOrWhiteSpace(documentNo))
            throw new ArgumentException("Document number is required.", nameof(documentNo));

        DocumentNo = documentNo.Trim();
        DocumentType = documentType;
        WarehouseRef = warehouseRef;
        SellerRef = sellerRef;
        OccurredAt = occurredAt;
        ReferenceType = Normalize(referenceType);
        ReferenceBusinessId = referenceBusinessId;
        CorrelationId = Normalize(correlationId);
        IdempotencyKey = Normalize(idempotencyKey);
        ReasonCode = Normalize(reasonCode);
        Status = InventoryDocumentStatus.Draft;
    }

    public static InventoryDocument Create(
        string documentNo,
        InventoryDocumentType documentType,
        Guid warehouseRef,
        Guid sellerRef,
        DateTime occurredAt,
        string? referenceType = null,
        Guid? referenceBusinessId = null,
        string? correlationId = null,
        string? idempotencyKey = null,
        string? reasonCode = null)
    {
        return new InventoryDocument(
            documentNo,
            documentType,
            warehouseRef,
            sellerRef,
            occurredAt,
            referenceType,
            referenceBusinessId,
            correlationId,
            idempotencyKey,
            reasonCode);
    }

    public void AddLine(InventoryDocumentLine line)
    {
        ArgumentNullException.ThrowIfNull(line);

        if (Status != InventoryDocumentStatus.Draft)
            throw new AggregateStateExceptions("Only draft documents can be edited.", nameof(Status));

        ValidateLineByType(line);
        Lines.Add(line);
    }

    public void Approve(string approvedBy)
    {
        if (Status != InventoryDocumentStatus.Draft)
            throw new AggregateStateExceptions("Only draft documents can be approved.", nameof(Status));

        EnsureHasLines();
        ApprovedAt = DateTime.UtcNow;
        ApprovedBy = Normalize(approvedBy);
        ChangeStatus(InventoryDocumentStatus.Approved, ReasonCode);
    }

    public void Reject(string rejectedBy, string? reasonCode = null)
    {
        if (Status is InventoryDocumentStatus.Posted or InventoryDocumentStatus.Cancelled)
            throw new AggregateStateExceptions("Posted or cancelled documents cannot be rejected.", nameof(Status));

        RejectedAt = DateTime.UtcNow;
        ReasonCode = Normalize(reasonCode) ?? ReasonCode;
        ChangeStatus(InventoryDocumentStatus.Rejected, ReasonCode ?? Normalize(rejectedBy));
    }

    public void Cancel(string cancelledBy, string? reasonCode = null)
    {
        if (Status is InventoryDocumentStatus.Posted or InventoryDocumentStatus.Rejected)
            throw new AggregateStateExceptions("Posted or rejected documents cannot be cancelled.", nameof(Status));

        CancelledAt = DateTime.UtcNow;
        ReasonCode = Normalize(reasonCode) ?? ReasonCode;
        ChangeStatus(InventoryDocumentStatus.Cancelled, ReasonCode ?? Normalize(cancelledBy));
    }

    public void MarkPosted(BusinessKey transactionBusinessKey, string? postedBy)
    {
        if (Status == InventoryDocumentStatus.Posted)
            throw new AggregateStateExceptions("Document is already posted.", nameof(Status));

        if (Status is InventoryDocumentStatus.Cancelled or InventoryDocumentStatus.Rejected)
            throw new AggregateStateExceptions("Cancelled or rejected documents cannot be posted.", nameof(Status));


        EnsureHasLines();

        PostedAt = DateTime.UtcNow;
        PostedBy = Normalize(postedBy);
        PostedTransactionRef = transactionBusinessKey.Value;
        ChangeStatus(InventoryDocumentStatus.Posted, ReasonCode);
    }

    private void ChangeStatus(InventoryDocumentStatus next, string? reasonCode)
    {
        var previous = Status;
        Status = next;
        AddEvent(new InventoryDocumentStatusChangedEvent(BusinessKey, previous, next, Normalize(reasonCode)));
    }

    private void ValidateLineByType(InventoryDocumentLine line)
    {
        switch (DocumentType)
        {
            case InventoryDocumentType.Receipt:
            case InventoryDocumentType.Return:
                if (!line.DestinationLocationRef.HasValue)
                    throw new AggregateStateExceptions("Destination location is required for receipt/return.", nameof(line.DestinationLocationRef));
                break;

            case InventoryDocumentType.Issue:
                if (!line.SourceLocationRef.HasValue)
                    throw new AggregateStateExceptions("Source location is required for issue.", nameof(line.SourceLocationRef));
                break;

            case InventoryDocumentType.Transfer:
                if (!line.SourceLocationRef.HasValue || !line.DestinationLocationRef.HasValue)
                    throw new AggregateStateExceptions("Transfer requires source and destination locations.", nameof(line.SourceLocationRef));
                if (line.SourceLocationRef == line.DestinationLocationRef)
                    throw new AggregateStateExceptions("Transfer source and destination locations must differ.", nameof(line.DestinationLocationRef));
                break;

            case InventoryDocumentType.Adjustment:
                if (!line.AdjustmentDirection.HasValue)
                    throw new AggregateStateExceptions("Adjustment direction is required for adjustment document.", nameof(line.AdjustmentDirection));
                if (string.IsNullOrWhiteSpace(line.ReasonCode))
                    throw new AggregateStateExceptions("Reason code is required for adjustment document.", nameof(line.ReasonCode));
                break;

            case InventoryDocumentType.QualityChange:
                if (!line.FromQualityStatusRef.HasValue || !line.ToQualityStatusRef.HasValue)
                    throw new AggregateStateExceptions("From/To quality status are required for quality change.", nameof(line.FromQualityStatusRef));
                if (line.FromQualityStatusRef == line.ToQualityStatusRef)
                    throw new AggregateStateExceptions("Quality statuses must differ in quality change document.", nameof(line.ToQualityStatusRef));
                break;
        }
    }

    private void EnsureHasLines()
    {
        if (Lines.Count == 0)
            throw new AggregateStateExceptions("Document must contain at least one line.", nameof(Lines));
    }

    private static string? Normalize(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}

