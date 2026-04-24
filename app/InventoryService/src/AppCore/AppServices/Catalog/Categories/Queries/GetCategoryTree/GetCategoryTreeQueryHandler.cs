namespace Insurance.InventoryService.AppCore.AppServices.Catalog.Categories.Queries.GetCategoryTree;

using Insurance.InventoryService.AppCore.Shared.Catalog.Categories.Queries;
using Insurance.InventoryService.AppCore.Shared.Catalog.Categories.Queries.GetCategoryTree;
using OysterFx.AppCore.AppServices.Queries;
using OysterFx.AppCore.Shared.Queries.Common;

public class GetCategoryTreeQueryHandler : QueryHandler<GetCategoryTreeQuery, GetCategoryTreeQueryResult>
{
    private readonly ICategoryQueryRepository _repository;

    public GetCategoryTreeQueryHandler(ICategoryQueryRepository repository)
    {
        _repository = repository;
    }

    public override async Task<QueryResult<GetCategoryTreeQueryResult>> ExecuteAsync(GetCategoryTreeQuery request)
    {
        var items = await _repository.GetTreeAsync(request.IncludeInactive);
        return QueryResult<GetCategoryTreeQueryResult>.Success(new GetCategoryTreeQueryResult { Items = items });
    }
}
