namespace Insurance.InventoryService.AppCore.AppServices.Catalog.Categories.Queries.GetActiveCategories;

using Insurance.InventoryService.AppCore.Shared.Catalog.Categories.Queries;
using Insurance.InventoryService.AppCore.Shared.Catalog.Categories.Queries.GetActiveCategories;
using OysterFx.AppCore.AppServices.Queries;
using OysterFx.AppCore.Shared.Queries.Common;

public class GetActiveCategoriesQueryHandler : QueryHandler<GetActiveCategoriesQuery, GetActiveCategoriesQueryResult>
{
    private readonly ICategoryQueryRepository _repository;

    public GetActiveCategoriesQueryHandler(ICategoryQueryRepository repository)
    {
        _repository = repository;
    }

    public override async Task<QueryResult<GetActiveCategoriesQueryResult>> ExecuteAsync(GetActiveCategoriesQuery request)
    {
        var items = await _repository.GetActiveCategoriesAsync();
        return QueryResult<GetActiveCategoriesQueryResult>.Success(new GetActiveCategoriesQueryResult { Items = items });
    }
}
