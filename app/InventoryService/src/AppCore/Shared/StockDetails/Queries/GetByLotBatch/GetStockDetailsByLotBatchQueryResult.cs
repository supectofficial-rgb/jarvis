namespace Insurance.InventoryService.AppCore.Shared.StockDetails.Queries.GetByLotBatch;

using Insurance.InventoryService.AppCore.Shared.StockDetails.Queries.Common;

public class GetStockDetailsByLotBatchQueryResult
{
    public List<StockDetailListItem> Items { get; set; } = new();
}
