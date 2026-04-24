namespace Insurance.InventoryService.AppCore.AppServices.Catalog.Categories.Queries.GetActiveCategoryAttributeRulesByCategoryId;

using Insurance.InventoryService.AppCore.Shared.Catalog.Categories.Queries;
using Insurance.InventoryService.AppCore.Shared.Catalog.Categories.Queries.GetActiveCategoryAttributeRulesByCategoryId;
using OysterFx.AppCore.AppServices.Queries;
using OysterFx.AppCore.Shared.Queries.Common;

public class GetActiveCategoryAttributeRulesByCategoryIdQueryHandler : QueryHandler<GetActiveCategoryAttributeRulesByCategoryIdQuery, GetActiveCategoryAttributeRulesByCategoryIdQueryResult>
{
    private readonly ICategoryQueryRepository _repository;

    public GetActiveCategoryAttributeRulesByCategoryIdQueryHandler(ICategoryQueryRepository repository)
    {
        _repository = repository;
    }

    public override async Task<QueryResult<GetActiveCategoryAttributeRulesByCategoryIdQueryResult>> ExecuteAsync(GetActiveCategoryAttributeRulesByCategoryIdQuery request)
    {
        var category = await _repository.GetByIdAsync(request.CategoryId);
        if (category is null)
            return QueryResult<GetActiveCategoryAttributeRulesByCategoryIdQueryResult>.Fail("Category was not found.", "NOT_FOUND");

        var items = await _repository.GetCategoryAttributeRulesByCategoryIdAsync(request.CategoryId, request.IncludeInherited, includeInactive: false);
        return QueryResult<GetActiveCategoryAttributeRulesByCategoryIdQueryResult>.Success(new GetActiveCategoryAttributeRulesByCategoryIdQueryResult
        {
            CategoryBusinessKey = request.CategoryId,
            Items = items.Where(x => x.RuleIsActive).ToList()
        });
    }
}
