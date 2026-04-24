namespace Insurance.InventoryService.AppCore.Shared.StockDetails.Queries.GetByQualityStatus;

using Insurance.InventoryService.AppCore.Shared.StockDetails.Queries.Common;

public class GetStockDetailsByQualityStatusQueryResult
{
    public List<StockDetailListItem> Items { get; set; } = new();
}
