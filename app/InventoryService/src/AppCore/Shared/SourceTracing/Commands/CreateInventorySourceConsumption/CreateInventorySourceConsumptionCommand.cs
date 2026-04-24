namespace Insurance.InventoryService.AppCore.Shared.SourceTracing.Commands.CreateInventorySourceConsumption;

using OysterFx.AppCore.Shared.Commands;

public class CreateInventorySourceConsumptionCommand : ICommand<Guid>
{
    public Guid SourceBalanceBusinessKey { get; set; }
    public Guid OutboundTransactionRef { get; set; }
    public Guid? OutboundTransactionLineRef { get; set; }
    public Guid? AllocationBusinessKey { get; set; }
    public decimal Quantity { get; set; }
    public string? ReasonCode { get; set; }
}
