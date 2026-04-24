namespace Insurance.InventoryService.AppCore.Domain.SourceTracing.Entities;

public enum InventorySourceType
{
    Receipt = 1,
    ReturnRestock = 2,
    AdjustmentIncrease = 3,
    OpeningBalance = 4
}
