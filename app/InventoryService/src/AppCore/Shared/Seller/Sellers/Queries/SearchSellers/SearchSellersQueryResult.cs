namespace Insurance.InventoryService.AppCore.Shared.Seller.Sellers.Queries.SearchSellers;

using Insurance.InventoryService.AppCore.Shared.Seller.Sellers.Queries.Common;

public class SearchSellersQueryResult
{
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public List<SellerListItem> Items { get; set; } = new();
}
