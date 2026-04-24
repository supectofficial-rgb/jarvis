namespace Insurance.InventoryService.AppCore.Shared.Reservations.Commands.RejectReservation;

using OysterFx.AppCore.Shared.Commands;

public class RejectReservationCommand : ICommand<Guid>
{
    public Guid ReservationBusinessKey { get; set; }
    public string? ReasonCode { get; set; }
}
