namespace Insurance.InventoryService.AppCore.Shared.InventoryTransactions.Commands.PostInventoryTransaction;

public class PostInventoryTransactionCommandResult
{
    public Guid TransactionBusinessKey { get; set; }
    public string Status { get; set; } = string.Empty;
}
