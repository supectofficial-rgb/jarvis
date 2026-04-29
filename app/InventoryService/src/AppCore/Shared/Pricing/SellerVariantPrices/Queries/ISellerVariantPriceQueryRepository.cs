namespace Insurance.InventoryService.AppCore.Shared.Pricing.SellerVariantPrices.Queries;

using Insurance.InventoryService.AppCore.Shared.Pricing.SellerVariantPrices.Queries.GetByBusinessKey;
using Insurance.InventoryService.AppCore.Shared.Pricing.SellerVariantPrices.Queries.SearchSellerVariantPrices;
using OysterFx.AppCore.Shared.Queries;

public interface ISellerVariantPriceQueryRepository : IQueryRepository
{
    Task<GetSellerVariantPriceByBusinessKeyQueryResult?> GetByBusinessKeyAsync(Guid sellerVariantPriceBusinessKey);
    Task<SearchSellerVariantPricesQueryResult> SearchAsync(SearchSellerVariantPricesQuery query);
}
