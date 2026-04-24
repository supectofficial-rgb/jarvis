namespace Insurance.InventoryService.AppCore.AppServices.Seller.Sellers.Queries.SearchSellers;

using Insurance.InventoryService.AppCore.Shared.Seller.Sellers.Queries;
using Insurance.InventoryService.AppCore.Shared.Seller.Sellers.Queries.SearchSellers;
using OysterFx.AppCore.AppServices.Queries;
using OysterFx.AppCore.Shared.Queries.Common;

public class SearchSellersQueryHandler : QueryHandler<SearchSellersQuery, SearchSellersQueryResult>
{
    private readonly ISellerQueryRepository _repository;

    public SearchSellersQueryHandler(ISellerQueryRepository repository)
    {
        _repository = repository;
    }

    public override async Task<QueryResult<SearchSellersQueryResult>> ExecuteAsync(SearchSellersQuery request)
    {
        var result = await _repository.SearchAsync(request);
        return QueryResult<SearchSellersQueryResult>.Success(result);
    }
}
