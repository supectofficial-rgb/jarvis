namespace Insurance.InventoryService.AppCore.Domain.InventoryDocuments.Entities;

public enum InventoryDocumentType
{
    Receipt = 1,
    Issue = 2,
    Transfer = 3,
    Adjustment = 4,
    Return = 5,
    QualityChange = 6
}
