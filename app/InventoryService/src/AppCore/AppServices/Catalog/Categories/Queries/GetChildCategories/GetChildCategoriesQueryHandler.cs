namespace Insurance.InventoryService.AppCore.AppServices.Catalog.Categories.Queries.GetChildCategories;

using Insurance.InventoryService.AppCore.Shared.Catalog.Categories.Queries;
using Insurance.InventoryService.AppCore.Shared.Catalog.Categories.Queries.GetChildCategories;
using OysterFx.AppCore.AppServices.Queries;
using OysterFx.AppCore.Shared.Queries.Common;

public class GetChildCategoriesQueryHandler : QueryHandler<GetChildCategoriesQuery, GetChildCategoriesQueryResult>
{
    private readonly ICategoryQueryRepository _repository;

    public GetChildCategoriesQueryHandler(ICategoryQueryRepository repository)
    {
        _repository = repository;
    }

    public override async Task<QueryResult<GetChildCategoriesQueryResult>> ExecuteAsync(GetChildCategoriesQuery request)
    {
        var parent = await _repository.GetByIdAsync(request.ParentCategoryId);
        if (parent is null)
            return QueryResult<GetChildCategoriesQueryResult>.Fail("Parent category was not found.", "NOT_FOUND");

        var items = await _repository.GetChildCategoriesAsync(request.ParentCategoryId, request.IncludeInactive);
        return QueryResult<GetChildCategoriesQueryResult>.Success(new GetChildCategoriesQueryResult
        {
            ParentCategoryId = request.ParentCategoryId,
            Items = items
        });
    }
}
