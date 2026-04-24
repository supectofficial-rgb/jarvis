namespace Insurance.InventoryService.AppCore.Shared.Reservations.Commands.ReleaseReservation;

using OysterFx.AppCore.Shared.Commands;

public class ReleaseReservationCommand : ICommand<Guid>
{
    public Guid ReservationBusinessKey { get; set; }
    public string? ReasonCode { get; set; }
}
