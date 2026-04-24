namespace Insurance.InventoryService.AppCore.Shared.StockDetails.Queries.GetStockAging;

using Insurance.InventoryService.AppCore.Shared.StockDetails.Queries.Common;

public class GetStockAgingQueryResult
{
    public List<StockAgingItem> Items { get; set; } = new();
}
