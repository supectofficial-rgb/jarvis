namespace Insurance.InventoryService.AppCore.AppServices.Catalog.Categories.Queries.GetCategoryBreadcrumb;

using Insurance.InventoryService.AppCore.Shared.Catalog.Categories.Queries;
using Insurance.InventoryService.AppCore.Shared.Catalog.Categories.Queries.GetCategoryBreadcrumb;
using OysterFx.AppCore.AppServices.Queries;
using OysterFx.AppCore.Shared.Queries.Common;

public class GetCategoryBreadcrumbQueryHandler : QueryHandler<GetCategoryBreadcrumbQuery, GetCategoryBreadcrumbQueryResult>
{
    private readonly ICategoryQueryRepository _repository;

    public GetCategoryBreadcrumbQueryHandler(ICategoryQueryRepository repository)
    {
        _repository = repository;
    }

    public override async Task<QueryResult<GetCategoryBreadcrumbQueryResult>> ExecuteAsync(GetCategoryBreadcrumbQuery request)
    {
        var category = await _repository.GetByIdAsync(request.CategoryId);
        if (category is null)
            return QueryResult<GetCategoryBreadcrumbQueryResult>.Fail("Category was not found.", "NOT_FOUND");

        var items = await _repository.GetBreadcrumbAsync(request.CategoryId);
        return QueryResult<GetCategoryBreadcrumbQueryResult>.Success(new GetCategoryBreadcrumbQueryResult
        {
            CategoryBusinessKey = request.CategoryId,
            Items = items
        });
    }
}
