namespace Insurance.InventoryService.AppCore.Shared.SourceTracing.Commands.ConsumeInventorySourceBalance;

using OysterFx.AppCore.Shared.Commands;

public class ConsumeInventorySourceBalanceCommand : ICommand<Guid>
{
    public Guid SourceBalanceBusinessKey { get; set; }
    public decimal Quantity { get; set; }
    public Guid OutboundTransactionRef { get; set; }
    public Guid? OutboundTransactionLineRef { get; set; }
    public string? ReasonCode { get; set; }
}
