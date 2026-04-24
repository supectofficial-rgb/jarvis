namespace Insurance.InventoryService.AppCore.Shared.StockDetails.Queries.GetAvailableStockBuckets;

using Insurance.InventoryService.AppCore.Shared.StockDetails.Queries.Common;

public class GetAvailableStockBucketsQueryResult
{
    public List<StockDetailListItem> Items { get; set; } = new();
}
