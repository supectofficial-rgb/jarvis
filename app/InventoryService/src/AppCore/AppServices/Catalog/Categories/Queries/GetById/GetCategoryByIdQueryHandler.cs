namespace Insurance.InventoryService.AppCore.AppServices.Catalog.Categories.Queries.GetById;

using Insurance.InventoryService.AppCore.Shared.Catalog.Categories.Queries;
using Insurance.InventoryService.AppCore.Shared.Catalog.Categories.Queries.GetByBusinessKey;
using Insurance.InventoryService.AppCore.Shared.Catalog.Categories.Queries.GetById;
using OysterFx.AppCore.AppServices.Queries;
using OysterFx.AppCore.Shared.Queries.Common;

public class GetCategoryByIdQueryHandler : QueryHandler<GetCategoryByIdQuery, GetCategoryByBusinessKeyQueryResult>
{
    private readonly ICategoryQueryRepository _repository;

    public GetCategoryByIdQueryHandler(ICategoryQueryRepository repository)
    {
        _repository = repository;
    }

    public override async Task<QueryResult<GetCategoryByBusinessKeyQueryResult>> ExecuteAsync(GetCategoryByIdQuery request)
    {
        var item = await _repository.GetByIdAsync(request.CategoryId);
        if (item is null)
            return QueryResult<GetCategoryByBusinessKeyQueryResult>.Fail("Category was not found.", "NOT_FOUND");

        return QueryResult<GetCategoryByBusinessKeyQueryResult>.Success(item);
    }
}
