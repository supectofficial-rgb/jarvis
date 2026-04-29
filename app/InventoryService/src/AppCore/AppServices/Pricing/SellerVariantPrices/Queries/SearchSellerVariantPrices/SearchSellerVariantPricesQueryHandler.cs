namespace Insurance.InventoryService.AppCore.AppServices.Pricing.SellerVariantPrices.Queries.SearchSellerVariantPrices;

using Insurance.InventoryService.AppCore.Shared.Pricing.SellerVariantPrices.Queries;
using Insurance.InventoryService.AppCore.Shared.Pricing.SellerVariantPrices.Queries.SearchSellerVariantPrices;
using OysterFx.AppCore.AppServices.Queries;
using OysterFx.AppCore.Shared.Queries.Common;

public class SearchSellerVariantPricesQueryHandler : QueryHandler<SearchSellerVariantPricesQuery, SearchSellerVariantPricesQueryResult>
{
    private readonly ISellerVariantPriceQueryRepository _repository;

    public SearchSellerVariantPricesQueryHandler(ISellerVariantPriceQueryRepository repository)
    {
        _repository = repository;
    }

    public override async Task<QueryResult<SearchSellerVariantPricesQueryResult>> ExecuteAsync(SearchSellerVariantPricesQuery request)
        => QueryResult<SearchSellerVariantPricesQueryResult>.Success(await _repository.SearchAsync(request));
}
