namespace Insurance.InventoryService.AppCore.Domain.InventoryTransactions.Entities;

public enum InventoryTransactionStatus
{
    Draft = 1,
    Posted = 2,
    Reversed = 3,
    Cancelled = 4
}
