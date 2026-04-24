namespace Insurance.InventoryService.AppCore.AppServices.Catalog.Categories.Queries.GetProductLevelCategoryAttributeRulesByCategoryId;

using Insurance.InventoryService.AppCore.Shared.Catalog.Categories.Queries;
using Insurance.InventoryService.AppCore.Shared.Catalog.Categories.Queries.GetProductLevelCategoryAttributeRulesByCategoryId;
using OysterFx.AppCore.AppServices.Queries;
using OysterFx.AppCore.Shared.Queries.Common;

public class GetProductLevelCategoryAttributeRulesByCategoryIdQueryHandler : QueryHandler<GetProductLevelCategoryAttributeRulesByCategoryIdQuery, GetProductLevelCategoryAttributeRulesByCategoryIdQueryResult>
{
    private readonly ICategoryQueryRepository _repository;

    public GetProductLevelCategoryAttributeRulesByCategoryIdQueryHandler(ICategoryQueryRepository repository)
    {
        _repository = repository;
    }

    public override async Task<QueryResult<GetProductLevelCategoryAttributeRulesByCategoryIdQueryResult>> ExecuteAsync(GetProductLevelCategoryAttributeRulesByCategoryIdQuery request)
    {
        var category = await _repository.GetByIdAsync(request.CategoryId);
        if (category is null)
            return QueryResult<GetProductLevelCategoryAttributeRulesByCategoryIdQueryResult>.Fail("Category was not found.", "NOT_FOUND");

        var items = await _repository.GetCategoryAttributeRulesByCategoryIdAsync(request.CategoryId, request.IncludeInherited, request.IncludeInactive);
        return QueryResult<GetProductLevelCategoryAttributeRulesByCategoryIdQueryResult>.Success(new GetProductLevelCategoryAttributeRulesByCategoryIdQueryResult
        {
            CategoryBusinessKey = request.CategoryId,
            Items = items.Where(x => !x.RuleIsVariant).ToList()
        });
    }
}
