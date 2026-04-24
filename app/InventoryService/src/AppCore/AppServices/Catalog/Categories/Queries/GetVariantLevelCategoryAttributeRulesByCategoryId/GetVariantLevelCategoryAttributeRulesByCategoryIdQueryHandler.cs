namespace Insurance.InventoryService.AppCore.AppServices.Catalog.Categories.Queries.GetVariantLevelCategoryAttributeRulesByCategoryId;

using Insurance.InventoryService.AppCore.Shared.Catalog.Categories.Queries;
using Insurance.InventoryService.AppCore.Shared.Catalog.Categories.Queries.GetVariantLevelCategoryAttributeRulesByCategoryId;
using OysterFx.AppCore.AppServices.Queries;
using OysterFx.AppCore.Shared.Queries.Common;

public class GetVariantLevelCategoryAttributeRulesByCategoryIdQueryHandler : QueryHandler<GetVariantLevelCategoryAttributeRulesByCategoryIdQuery, GetVariantLevelCategoryAttributeRulesByCategoryIdQueryResult>
{
    private readonly ICategoryQueryRepository _repository;

    public GetVariantLevelCategoryAttributeRulesByCategoryIdQueryHandler(ICategoryQueryRepository repository)
    {
        _repository = repository;
    }

    public override async Task<QueryResult<GetVariantLevelCategoryAttributeRulesByCategoryIdQueryResult>> ExecuteAsync(GetVariantLevelCategoryAttributeRulesByCategoryIdQuery request)
    {
        var category = await _repository.GetByIdAsync(request.CategoryId);
        if (category is null)
            return QueryResult<GetVariantLevelCategoryAttributeRulesByCategoryIdQueryResult>.Fail("Category was not found.", "NOT_FOUND");

        var items = await _repository.GetCategoryAttributeRulesByCategoryIdAsync(request.CategoryId, request.IncludeInherited, request.IncludeInactive);
        return QueryResult<GetVariantLevelCategoryAttributeRulesByCategoryIdQueryResult>.Success(new GetVariantLevelCategoryAttributeRulesByCategoryIdQueryResult
        {
            CategoryBusinessKey = request.CategoryId,
            Items = items.Where(x => x.RuleIsVariant).ToList()
        });
    }
}
