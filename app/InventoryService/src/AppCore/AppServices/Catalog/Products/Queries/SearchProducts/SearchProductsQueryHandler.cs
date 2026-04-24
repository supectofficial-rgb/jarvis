namespace Insurance.InventoryService.AppCore.AppServices.Catalog.Products.Queries.SearchProducts;

using Insurance.InventoryService.AppCore.Shared.Catalog.Products.Queries;
using Insurance.InventoryService.AppCore.Shared.Catalog.Products.Queries.SearchProducts;
using OysterFx.AppCore.AppServices.Queries;
using OysterFx.AppCore.Shared.Queries.Common;

public class SearchProductsQueryHandler : QueryHandler<SearchProductsQuery, SearchProductsQueryResult>
{
    private readonly IProductQueryRepository _repository;

    public SearchProductsQueryHandler(IProductQueryRepository repository)
    {
        _repository = repository;
    }

    public override async Task<QueryResult<SearchProductsQueryResult>> ExecuteAsync(SearchProductsQuery request)
    {
        var result = await _repository.SearchAsync(request);
        return QueryResult<SearchProductsQueryResult>.Success(result);
    }
}
