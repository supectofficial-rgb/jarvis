namespace Insurance.InventoryService.AppCore.AppServices.Catalog.ProductVariants.Queries.SearchVariants;

using Insurance.InventoryService.AppCore.Shared.Catalog.ProductVariants.Queries;
using Insurance.InventoryService.AppCore.Shared.Catalog.ProductVariants.Queries.SearchVariants;
using OysterFx.AppCore.AppServices.Queries;
using OysterFx.AppCore.Shared.Queries.Common;

public class SearchProductVariantsQueryHandler : QueryHandler<SearchVariantsQuery, SearchVariantsQueryResult>
{
    private readonly IProductVariantQueryRepository _repository;

    public SearchProductVariantsQueryHandler(IProductVariantQueryRepository repository)
    {
        _repository = repository;
    }

    public override async Task<QueryResult<SearchVariantsQueryResult>> ExecuteAsync(SearchVariantsQuery request)
    {
        var item = await _repository.SearchAsync(request);
        return QueryResult<SearchVariantsQueryResult>.Success(item);
    }
}



