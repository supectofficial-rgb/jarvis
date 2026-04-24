namespace Insurance.InventoryService.AppCore.AppServices.Catalog.Categories.Queries.GetRootCategories;

using Insurance.InventoryService.AppCore.Shared.Catalog.Categories.Queries;
using Insurance.InventoryService.AppCore.Shared.Catalog.Categories.Queries.GetRootCategories;
using OysterFx.AppCore.AppServices.Queries;
using OysterFx.AppCore.Shared.Queries.Common;

public class GetRootCategoriesQueryHandler : QueryHandler<GetRootCategoriesQuery, GetRootCategoriesQueryResult>
{
    private readonly ICategoryQueryRepository _repository;

    public GetRootCategoriesQueryHandler(ICategoryQueryRepository repository)
    {
        _repository = repository;
    }

    public override async Task<QueryResult<GetRootCategoriesQueryResult>> ExecuteAsync(GetRootCategoriesQuery request)
    {
        var items = await _repository.GetRootCategoriesAsync(request.IncludeInactive);
        return QueryResult<GetRootCategoriesQueryResult>.Success(new GetRootCategoriesQueryResult { Items = items });
    }
}
