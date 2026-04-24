namespace Insurance.InventoryService.AppCore.Shared.StockDetails.Queries.GetSellerStockSummary;

using Insurance.InventoryService.AppCore.Shared.StockDetails.Queries.Common;

public class GetSellerStockSummaryQueryResult
{
    public SellerStockSummaryItem Item { get; set; } = new();
}
