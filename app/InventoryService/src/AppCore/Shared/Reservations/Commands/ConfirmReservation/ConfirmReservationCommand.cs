namespace Insurance.InventoryService.AppCore.Shared.Reservations.Commands.ConfirmReservation;

using OysterFx.AppCore.Shared.Commands;

public class ConfirmReservationCommand : ICommand<Guid>
{
    public Guid ReservationBusinessKey { get; set; }
    public decimal? AllocatedQuantity { get; set; }
}
