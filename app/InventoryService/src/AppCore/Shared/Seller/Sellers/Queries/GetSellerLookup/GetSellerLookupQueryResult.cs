namespace Insurance.InventoryService.AppCore.Shared.Seller.Sellers.Queries.GetSellerLookup;

using Insurance.InventoryService.AppCore.Shared.Seller.Sellers.Queries.Common;

public class GetSellerLookupQueryResult
{
    public List<SellerLookupItem> Items { get; set; } = new();
}
