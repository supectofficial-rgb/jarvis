namespace Insurance.InventoryService.AppCore.Domain.Reservations.Entities;

using OysterFx.AppCore.Domain.Aggregates;

public sealed class ReservationTransition : Aggregate
{
    public Guid ReservationRef { get; private set; }
    public InventoryReservationStatus FromStatus { get; private set; }
    public InventoryReservationStatus ToStatus { get; private set; }
    public string? ReasonCode { get; private set; }
    public DateTime ChangedAt { get; private set; }

    private ReservationTransition()
    {
    }

    internal static ReservationTransition Create(
        Guid reservationRef,
        InventoryReservationStatus fromStatus,
        InventoryReservationStatus toStatus,
        string? reasonCode)
    {
        return new ReservationTransition
        {
            ReservationRef = reservationRef,
            FromStatus = fromStatus,
            ToStatus = toStatus,
            ReasonCode = reasonCode,
            ChangedAt = DateTime.UtcNow
        };
    }
}
