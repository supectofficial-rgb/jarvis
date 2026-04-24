namespace Insurance.InventoryService.AppCore.Shared.InventoryTransactions.Commands.CancelInventoryTransaction;

using OysterFx.AppCore.Shared.Commands;

public class CancelInventoryTransactionCommand : ICommand<CancelInventoryTransactionCommandResult>
{
    public Guid TransactionBusinessKey { get; set; }
    public string? ReasonCode { get; set; }
}
