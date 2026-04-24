namespace Insurance.InventoryService.AppCore.AppServices.Catalog.Categories.Queries.GetCategoryAttributeRulesByCategoryId;

using Insurance.InventoryService.AppCore.Shared.Catalog.Categories.Queries;
using Insurance.InventoryService.AppCore.Shared.Catalog.Categories.Queries.GetCategoryAttributeRulesByCategoryId;
using OysterFx.AppCore.AppServices.Queries;
using OysterFx.AppCore.Shared.Queries.Common;

public class GetCategoryAttributeRulesByCategoryIdQueryHandler : QueryHandler<GetCategoryAttributeRulesByCategoryIdQuery, GetCategoryAttributeRulesByCategoryIdQueryResult>
{
    private readonly ICategoryQueryRepository _repository;

    public GetCategoryAttributeRulesByCategoryIdQueryHandler(ICategoryQueryRepository repository)
    {
        _repository = repository;
    }

    public override async Task<QueryResult<GetCategoryAttributeRulesByCategoryIdQueryResult>> ExecuteAsync(GetCategoryAttributeRulesByCategoryIdQuery request)
    {
        var category = await _repository.GetByIdAsync(request.CategoryId);
        if (category is null)
            return QueryResult<GetCategoryAttributeRulesByCategoryIdQueryResult>.Fail("Category was not found.", "NOT_FOUND");

        var items = await _repository.GetCategoryAttributeRulesByCategoryIdAsync(request.CategoryId, request.IncludeInherited, request.IncludeInactive);
        return QueryResult<GetCategoryAttributeRulesByCategoryIdQueryResult>.Success(new GetCategoryAttributeRulesByCategoryIdQueryResult
        {
            CategoryBusinessKey = request.CategoryId,
            Items = items
        });
    }
}
