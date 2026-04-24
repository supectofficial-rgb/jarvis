namespace Insurance.InventoryService.AppCore.AppServices.Catalog.Categories.Queries.GetAttributes;

using Insurance.InventoryService.AppCore.Shared.Catalog.Categories.Queries;
using Insurance.InventoryService.AppCore.Shared.Catalog.Categories.Queries.GetAttributes;
using OysterFx.AppCore.AppServices.Queries;
using OysterFx.AppCore.Shared.Queries.Common;

public class GetCategoryAttributesQueryHandler
    : QueryHandler<GetCategoryAttributesQuery, GetCategoryAttributesQueryResult>
{
    private readonly ICategoryQueryRepository _repository;

    public GetCategoryAttributesQueryHandler(ICategoryQueryRepository repository)
    {
        _repository = repository;
    }

    public override async Task<QueryResult<GetCategoryAttributesQueryResult>> ExecuteAsync(GetCategoryAttributesQuery request)
    {
        var item = await _repository.GetAttributesAsync(request.CategoryBusinessKey, request.IncludeInherited, request.IncludeInactive);
        if (item is null)
            return QueryResult<GetCategoryAttributesQueryResult>.Fail("Category was not found.", "NOT_FOUND");

        return QueryResult<GetCategoryAttributesQueryResult>.Success(item);
    }
}
