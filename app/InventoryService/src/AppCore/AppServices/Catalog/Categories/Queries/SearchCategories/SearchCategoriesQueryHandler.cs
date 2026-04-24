namespace Insurance.InventoryService.AppCore.AppServices.Catalog.Categories.Queries.SearchCategories;

using Insurance.InventoryService.AppCore.Shared.Catalog.Categories.Queries;
using Insurance.InventoryService.AppCore.Shared.Catalog.Categories.Queries.SearchCategories;
using OysterFx.AppCore.AppServices.Queries;
using OysterFx.AppCore.Shared.Queries.Common;

public class SearchCategoriesQueryHandler : QueryHandler<SearchCategoriesQuery, SearchCategoriesQueryResult>
{
    private readonly ICategoryQueryRepository _repository;

    public SearchCategoriesQueryHandler(ICategoryQueryRepository repository)
    {
        _repository = repository;
    }

    public override async Task<QueryResult<SearchCategoriesQueryResult>> ExecuteAsync(SearchCategoriesQuery request)
    {
        var result = await _repository.SearchAsync(request);
        return QueryResult<SearchCategoriesQueryResult>.Success(result);
    }
}
