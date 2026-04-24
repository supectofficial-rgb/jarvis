namespace Insurance.InventoryService.AppCore.AppServices.Catalog.Categories.Queries.GetByBusinessKey;

using Insurance.InventoryService.AppCore.Shared.Catalog.Categories.Queries;
using Insurance.InventoryService.AppCore.Shared.Catalog.Categories.Queries.GetByBusinessKey;
using OysterFx.AppCore.AppServices.Queries;
using OysterFx.AppCore.Shared.Queries.Common;

public class GetCategoryByBusinessKeyQueryHandler
    : QueryHandler<GetCategoryByBusinessKeyQuery, GetCategoryByBusinessKeyQueryResult>
{
    private readonly ICategoryQueryRepository _repository;

    public GetCategoryByBusinessKeyQueryHandler(ICategoryQueryRepository repository)
    {
        _repository = repository;
    }

    public override async Task<QueryResult<GetCategoryByBusinessKeyQueryResult>> ExecuteAsync(GetCategoryByBusinessKeyQuery request)
    {
        var item = await _repository.GetByBusinessKeyAsync(request.CategoryBusinessKey);
        if (item is null)
            return QueryResult<GetCategoryByBusinessKeyQueryResult>.Fail("Category was not found.", "NOT_FOUND");

        return QueryResult<GetCategoryByBusinessKeyQueryResult>.Success(item);
    }
}
