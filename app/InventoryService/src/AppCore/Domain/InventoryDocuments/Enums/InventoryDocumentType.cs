namespace Insurance.InventoryService.AppCore.Domain.InventoryDocuments.Entities;

public enum InventoryDocumentType
{
    Receipt = 1,
    Issue = 2,
    Transfer = 3,
    Adjustment = 4,
    ReturnFromSell = 5,
    QualityChange = 6,
    Conversion = 7,
    ReturnFromBuy = 8,
    ReturnFromTransfer = 9,
    [Obsolete("Use ReturnFromSell instead.")]
    Return = ReturnFromSell
}
