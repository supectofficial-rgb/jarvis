namespace Insurance.InventoryService.AppCore.Shared.SourceTracing.Commands.CreateInventorySourceAllocation;

using OysterFx.AppCore.Shared.Commands;

public class CreateInventorySourceAllocationCommand : ICommand<Guid>
{
    public Guid SourceBalanceBusinessKey { get; set; }
    public Guid ReservationRef { get; set; }
    public Guid? ReservationAllocationRef { get; set; }
    public decimal Quantity { get; set; }
}
