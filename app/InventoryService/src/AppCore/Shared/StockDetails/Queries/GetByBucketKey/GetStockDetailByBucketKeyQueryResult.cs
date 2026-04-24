namespace Insurance.InventoryService.AppCore.Shared.StockDetails.Queries.GetByBucketKey;

using Insurance.InventoryService.AppCore.Shared.StockDetails.Queries.Common;

public class GetStockDetailByBucketKeyQueryResult
{
    public StockDetailListItem Item { get; set; } = new();
}
