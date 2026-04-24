namespace Insurance.InventoryService.AppCore.Shared.Reservations.Commands.ConsumeReservation;

using OysterFx.AppCore.Shared.Commands;

public class ConsumeReservationCommand : ICommand<Guid>
{
    public Guid ReservationBusinessKey { get; set; }
    public decimal Quantity { get; set; }
    public string? ReasonCode { get; set; }
}
