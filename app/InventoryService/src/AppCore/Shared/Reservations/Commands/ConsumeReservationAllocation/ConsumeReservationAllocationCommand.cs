namespace Insurance.InventoryService.AppCore.Shared.Reservations.Commands.ConsumeReservationAllocation;

using OysterFx.AppCore.Shared.Commands;

public class ConsumeReservationAllocationCommand : ICommand<Guid>
{
    public Guid ReservationBusinessKey { get; set; }
    public Guid AllocationBusinessKey { get; set; }
    public decimal Quantity { get; set; }
    public string? ReasonCode { get; set; }
}
