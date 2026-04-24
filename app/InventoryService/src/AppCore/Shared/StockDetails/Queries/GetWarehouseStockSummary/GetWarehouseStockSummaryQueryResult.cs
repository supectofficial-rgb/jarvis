namespace Insurance.InventoryService.AppCore.Shared.StockDetails.Queries.GetWarehouseStockSummary;

using Insurance.InventoryService.AppCore.Shared.StockDetails.Queries.Common;

public class GetWarehouseStockSummaryQueryResult
{
    public WarehouseStockSummaryItem Item { get; set; } = new();
}
