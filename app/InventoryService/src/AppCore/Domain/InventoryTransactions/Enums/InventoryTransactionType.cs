namespace Insurance.InventoryService.AppCore.Domain.InventoryTransactions.Entities;

public enum InventoryTransactionType
{
    Receipt = 1,
    Issue = 2,
    Transfer = 3,
    Adjustment = 4,
    ReturnFromSell = 5,
    QualityChange = 6,
    ReserveConsume = 7,
    Conversion = 8,
    ReturnFromBuy = 9,
    ReturnFromTransfer = 10,
    [Obsolete("Use ReturnFromSell instead.")]
    Return = ReturnFromSell
}
