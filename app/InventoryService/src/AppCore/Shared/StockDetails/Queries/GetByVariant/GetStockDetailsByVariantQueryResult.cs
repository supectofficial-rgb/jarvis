namespace Insurance.InventoryService.AppCore.Shared.StockDetails.Queries.GetByVariant;

using Insurance.InventoryService.AppCore.Shared.StockDetails.Queries.Common;

public class GetStockDetailsByVariantQueryResult
{
    public List<StockDetailListItem> Items { get; set; } = new();
}
