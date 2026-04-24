namespace Insurance.InventoryService.AppCore.Shared.SourceTracing.Commands.ReleaseInventorySourceAllocation;

using OysterFx.AppCore.Shared.Commands;

public class ReleaseInventorySourceAllocationCommand : ICommand<Guid>
{
    public Guid SourceBalanceBusinessKey { get; set; }
    public Guid AllocationBusinessKey { get; set; }
    public decimal Quantity { get; set; }
}
