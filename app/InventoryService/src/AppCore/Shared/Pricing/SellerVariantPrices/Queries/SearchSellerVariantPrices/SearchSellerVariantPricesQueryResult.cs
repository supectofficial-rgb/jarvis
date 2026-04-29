namespace Insurance.InventoryService.AppCore.Shared.Pricing.SellerVariantPrices.Queries.SearchSellerVariantPrices;

using Insurance.InventoryService.AppCore.Shared.Pricing.SellerVariantPrices.Queries.Common;

public class SearchSellerVariantPricesQueryResult
{
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public List<SellerVariantPriceListItem> Items { get; set; } = new();
}
