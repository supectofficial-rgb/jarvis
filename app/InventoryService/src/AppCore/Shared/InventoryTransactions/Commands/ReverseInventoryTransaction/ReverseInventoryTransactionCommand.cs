namespace Insurance.InventoryService.AppCore.Shared.InventoryTransactions.Commands.ReverseInventoryTransaction;

using OysterFx.AppCore.Shared.Commands;

public class ReverseInventoryTransactionCommand : ICommand<ReverseInventoryTransactionCommandResult>
{
    public Guid TransactionBusinessKey { get; set; }
    public Guid ReversedByTransactionBusinessKey { get; set; }
    public string? ReasonCode { get; set; }
}
