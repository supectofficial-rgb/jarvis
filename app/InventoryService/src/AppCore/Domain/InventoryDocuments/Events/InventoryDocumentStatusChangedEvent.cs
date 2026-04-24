namespace Insurance.InventoryService.AppCore.Domain.InventoryDocuments.Events;

using Insurance.InventoryService.AppCore.Domain.InventoryDocuments.Entities;
using OysterFx.AppCore.Domain.Events;
using OysterFx.AppCore.Domain.ValueObjects;

public sealed record InventoryDocumentStatusChangedEvent : IDomainEvent
{
    public BusinessKey DocumentBusinessKey { get; }
    public InventoryDocumentStatus PreviousStatus { get; }
    public InventoryDocumentStatus CurrentStatus { get; }
    public string? ReasonCode { get; }
    public DateTime OccurredOn { get; }

    public InventoryDocumentStatusChangedEvent(
        BusinessKey documentBusinessKey,
        InventoryDocumentStatus previousStatus,
        InventoryDocumentStatus currentStatus,
        string? reasonCode)
    {
        DocumentBusinessKey = documentBusinessKey;
        PreviousStatus = previousStatus;
        CurrentStatus = currentStatus;
        ReasonCode = reasonCode;
        OccurredOn = DateTime.UtcNow;
    }
}
