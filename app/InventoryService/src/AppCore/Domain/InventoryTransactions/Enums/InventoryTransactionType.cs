namespace Insurance.InventoryService.AppCore.Domain.InventoryTransactions.Entities;

public enum InventoryTransactionType
{
    Receipt = 1,
    Issue = 2,
    Transfer = 3,
    Adjustment = 4,
    Return = 5,
    QualityChange = 6,
    ReserveConsume = 7
}
