namespace Insurance.InventoryService.AppCore.Shared.InventoryTransactions.Commands.ReverseInventoryTransaction;

public class ReverseInventoryTransactionCommandResult
{
    public Guid TransactionBusinessKey { get; set; }
    public Guid ReversedByTransactionBusinessKey { get; set; }
    public string Status { get; set; } = string.Empty;
}
