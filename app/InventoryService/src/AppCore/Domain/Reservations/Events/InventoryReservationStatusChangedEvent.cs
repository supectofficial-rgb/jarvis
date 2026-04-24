namespace Insurance.InventoryService.AppCore.Domain.Reservations.Events;

using Insurance.InventoryService.AppCore.Domain.Reservations.Entities;
using OysterFx.AppCore.Domain.Events;
using OysterFx.AppCore.Domain.ValueObjects;

public sealed record InventoryReservationStatusChangedEvent : IDomainEvent
{
    public BusinessKey ReservationBusinessKey { get; }
    public InventoryReservationStatus PreviousStatus { get; }
    public InventoryReservationStatus CurrentStatus { get; }
    public string? ReasonCode { get; }
    public DateTime OccurredOn { get; }

    public InventoryReservationStatusChangedEvent(
        BusinessKey reservationBusinessKey,
        InventoryReservationStatus previousStatus,
        InventoryReservationStatus currentStatus,
        string? reasonCode)
    {
        ReservationBusinessKey = reservationBusinessKey;
        PreviousStatus = previousStatus;
        CurrentStatus = currentStatus;
        ReasonCode = reasonCode;
        OccurredOn = DateTime.UtcNow;
    }
}
