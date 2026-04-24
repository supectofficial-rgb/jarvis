namespace Insurance.InventoryService.AppCore.Shared.StockDetails.Queries.GetBySeller;

using Insurance.InventoryService.AppCore.Shared.StockDetails.Queries.Common;

public class GetStockDetailsBySellerQueryResult
{
    public List<StockDetailListItem> Items { get; set; } = new();
}
