namespace Insurance.InventoryService.AppCore.Shared.InventoryTransactions.Commands.CancelInventoryTransaction;

public class CancelInventoryTransactionCommandResult
{
    public Guid TransactionBusinessKey { get; set; }
    public string Status { get; set; } = string.Empty;
}
