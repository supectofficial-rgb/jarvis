namespace Insurance.InventoryService.AppCore.Domain.Returns.Events;

using Insurance.InventoryService.AppCore.Domain.Returns.Entities;
using OysterFx.AppCore.Domain.Events;
using OysterFx.AppCore.Domain.ValueObjects;

public sealed record ReturnRequestStatusChangedEvent : IDomainEvent
{
    public BusinessKey ReturnRequestBusinessKey { get; }
    public ReturnRequestStatus PreviousStatus { get; }
    public ReturnRequestStatus CurrentStatus { get; }
    public string? ReasonCode { get; }
    public DateTime OccurredOn { get; }

    public ReturnRequestStatusChangedEvent(
        BusinessKey returnRequestBusinessKey,
        ReturnRequestStatus previousStatus,
        ReturnRequestStatus currentStatus,
        string? reasonCode)
    {
        ReturnRequestBusinessKey = returnRequestBusinessKey;
        PreviousStatus = previousStatus;
        CurrentStatus = currentStatus;
        ReasonCode = reasonCode;
        OccurredOn = DateTime.UtcNow;
    }
}
