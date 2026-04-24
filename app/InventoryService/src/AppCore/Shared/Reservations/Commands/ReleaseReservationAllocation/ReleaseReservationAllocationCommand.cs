namespace Insurance.InventoryService.AppCore.Shared.Reservations.Commands.ReleaseReservationAllocation;

using OysterFx.AppCore.Shared.Commands;

public class ReleaseReservationAllocationCommand : ICommand<Guid>
{
    public Guid ReservationBusinessKey { get; set; }
    public Guid AllocationBusinessKey { get; set; }
    public decimal Quantity { get; set; }
}
