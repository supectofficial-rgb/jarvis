namespace Insurance.InventoryService.AppCore.Shared.Pricing.SellerVariantPrices.Queries.SearchSellerVariantPrices;

using OysterFx.AppCore.Shared.Queries;

public class SearchSellerVariantPricesQuery : IQuery<SearchSellerVariantPricesQueryResult>
{
    public Guid? SellerRef { get; set; }
    public Guid? VariantRef { get; set; }
    public Guid? PriceTypeRef { get; set; }
    public Guid? PriceChannelRef { get; set; }
    public bool? IsActive { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 50;
}
