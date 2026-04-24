namespace Insurance.InventoryService.AppCore.Domain.Returns.Entities;

using Insurance.InventoryService.AppCore.Domain.Returns.Events;
using OysterFx.AppCore.Domain.Aggregates;

public sealed class ReturnRequest : AggregateRoot
{
    private readonly List<ReturnLine> _lines = new();
    private readonly List<ReturnRequestTransition> _transitions = new();

    public Guid OrderRef { get; private set; }
    public Guid OrderItemRef { get; private set; }
    public Guid SellerRef { get; private set; }
    public Guid WarehouseRef { get; private set; }
    public ReturnRequestStatus Status { get; private set; }
    public string? ReasonCode { get; private set; }
    public DateTime RequestedAt { get; private set; }
    public DateTime? ApprovedAt { get; private set; }
    public DateTime? RejectedAt { get; private set; }
    public DateTime? ReceivedAt { get; private set; }
    public DateTime? ClosedAt { get; private set; }
    public string? ApprovedBy { get; private set; }
    public string? RejectedBy { get; private set; }
    public string? ReceivedBy { get; private set; }
    public IReadOnlyCollection<ReturnLine> Lines => _lines.AsReadOnly();
    public IReadOnlyCollection<ReturnRequestTransition> Transitions => _transitions.AsReadOnly();

    private ReturnRequest()
    {
    }

    public static ReturnRequest Create(
        Guid orderRef,
        Guid orderItemRef,
        Guid sellerRef,
        Guid warehouseRef,
        string? reasonCode)
    {
        return new ReturnRequest
        {
            OrderRef = orderRef,
            OrderItemRef = orderItemRef,
            SellerRef = sellerRef,
            WarehouseRef = warehouseRef,
            ReasonCode = Normalize(reasonCode),
            Status = ReturnRequestStatus.Requested,
            RequestedAt = DateTime.UtcNow
        };
    }

    public ReturnLine AddLine(
        Guid variantRef,
        decimal qty,
        Guid uomRef,
        decimal baseQty,
        Guid baseUomRef,
        string? lotBatchNo = null,
        string? expectedCondition = null,
        ReturnDisposition disposition = ReturnDisposition.Restock)
    {
        if (Status != ReturnRequestStatus.Requested)
            throw new InvalidOperationException("Return lines can only be added while request is in requested state.");

        var line = ReturnLine.Create(
            BusinessKey.Value,
            variantRef,
            qty,
            uomRef,
            baseQty,
            baseUomRef,
            lotBatchNo,
            expectedCondition,
            disposition);

        _lines.Add(line);
        return line;
    }

    public void Approve(string approvedBy)
    {
        EnsureStatus(ReturnRequestStatus.Requested);
        ApprovedBy = Normalize(approvedBy);
        ApprovedAt = DateTime.UtcNow;
        TransitionTo(ReturnRequestStatus.Approved, "approved");
    }

    public void Reject(string rejectedBy, string? reasonCode = null)
    {
        EnsureStatus(ReturnRequestStatus.Requested);
        RejectedBy = Normalize(rejectedBy);
        RejectedAt = DateTime.UtcNow;
        ReasonCode = Normalize(reasonCode) ?? ReasonCode;
        TransitionTo(ReturnRequestStatus.Rejected, ReasonCode ?? "rejected");
    }

    public void MarkReceived(string receivedBy)
    {
        EnsureStatus(ReturnRequestStatus.Approved);

        if (_lines.Count == 0)
            throw new InvalidOperationException("Return request must contain at least one line.");

        ReceivedBy = Normalize(receivedBy);
        ReceivedAt = DateTime.UtcNow;
        TransitionTo(ReturnRequestStatus.Received, "received");
    }

    public void Close()
    {
        EnsureStatus(ReturnRequestStatus.Received);

        if (_lines.Count == 0)
            throw new InvalidOperationException("Return request must contain at least one line.");

        if (_lines.Any(x => x.Status != ReturnLineStatus.Closed))
            throw new InvalidOperationException("All return lines must be closed before closing the request.");

        ClosedAt = DateTime.UtcNow;
        TransitionTo(ReturnRequestStatus.Closed, "closed");
    }

    public void ReceiveLine(Guid lineBusinessKey, decimal quantity, string? receivedCondition = null)
    {
        EnsureStatus(ReturnRequestStatus.Received);

        var line = FindLine(lineBusinessKey);
        line.Receive(quantity, receivedCondition);
    }

    public void CloseLine(Guid lineBusinessKey)
    {
        EnsureStatus(ReturnRequestStatus.Received);

        var line = FindLine(lineBusinessKey);
        line.Close();
    }

    private void EnsureStatus(ReturnRequestStatus expected)
    {
        if (Status != expected)
            throw new InvalidOperationException($"Return request must be in {expected} state.");
    }

    private ReturnLine FindLine(Guid lineBusinessKey)
    {
        var line = _lines.FirstOrDefault(x => x.BusinessKey.Value == lineBusinessKey);
        if (line is null)
            throw new InvalidOperationException("Return line was not found.");

        return line;
    }

    private void TransitionTo(ReturnRequestStatus nextStatus, string? reasonCode)
    {
        var previousStatus = Status;
        Status = nextStatus;
        AddTransition(previousStatus, nextStatus, reasonCode);
        AddEvent(new ReturnRequestStatusChangedEvent(BusinessKey, previousStatus, nextStatus, Normalize(reasonCode)));
    }

    private void AddTransition(ReturnRequestStatus fromStatus, ReturnRequestStatus toStatus, string? reasonCode)
    {
        _transitions.Add(ReturnRequestTransition.Create(BusinessKey.Value, fromStatus, toStatus, reasonCode));
    }

    private static string? Normalize(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
