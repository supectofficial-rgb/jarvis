namespace Insurance.InventoryService.AppCore.Shared.Seller.Sellers.Queries.GetActiveSellers;

using Insurance.InventoryService.AppCore.Shared.Seller.Sellers.Queries.Common;

public class GetActiveSellersQueryResult
{
    public List<SellerListItem> Items { get; set; } = new();
}
