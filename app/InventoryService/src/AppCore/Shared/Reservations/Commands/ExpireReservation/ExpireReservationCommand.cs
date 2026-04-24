namespace Insurance.InventoryService.AppCore.Shared.Reservations.Commands.ExpireReservation;

using OysterFx.AppCore.Shared.Commands;

public class ExpireReservationCommand : ICommand<Guid>
{
    public Guid ReservationBusinessKey { get; set; }
    public string? ReasonCode { get; set; }
}
