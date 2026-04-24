namespace Insurance.InventoryService.AppCore.Shared.StockDetails.Queries.GetEmptyStockBuckets;

using Insurance.InventoryService.AppCore.Shared.StockDetails.Queries.Common;

public class GetEmptyStockBucketsQueryResult
{
    public List<StockDetailListItem> Items { get; set; } = new();
}
