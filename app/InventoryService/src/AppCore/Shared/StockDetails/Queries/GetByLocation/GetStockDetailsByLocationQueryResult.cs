namespace Insurance.InventoryService.AppCore.Shared.StockDetails.Queries.GetByLocation;

using Insurance.InventoryService.AppCore.Shared.StockDetails.Queries.Common;

public class GetStockDetailsByLocationQueryResult
{
    public List<StockDetailListItem> Items { get; set; } = new();
}
