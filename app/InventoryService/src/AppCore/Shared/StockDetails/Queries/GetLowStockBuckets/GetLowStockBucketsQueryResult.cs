namespace Insurance.InventoryService.AppCore.Shared.StockDetails.Queries.GetLowStockBuckets;

using Insurance.InventoryService.AppCore.Shared.StockDetails.Queries.Common;

public class GetLowStockBucketsQueryResult
{
    public List<StockDetailListItem> Items { get; set; } = new();
}
