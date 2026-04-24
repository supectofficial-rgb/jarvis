namespace Insurance.InventoryService.AppCore.Shared.StockDetails.Queries.SearchStockDetails;

using Insurance.InventoryService.AppCore.Shared.StockDetails.Queries.Common;

public class SearchStockDetailsQueryResult
{
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public List<StockDetailListItem> Items { get; set; } = new();
}
