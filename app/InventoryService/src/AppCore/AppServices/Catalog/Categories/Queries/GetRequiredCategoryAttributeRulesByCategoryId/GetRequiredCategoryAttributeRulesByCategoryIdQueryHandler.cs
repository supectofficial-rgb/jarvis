namespace Insurance.InventoryService.AppCore.AppServices.Catalog.Categories.Queries.GetRequiredCategoryAttributeRulesByCategoryId;

using Insurance.InventoryService.AppCore.Shared.Catalog.Categories.Queries;
using Insurance.InventoryService.AppCore.Shared.Catalog.Categories.Queries.GetRequiredCategoryAttributeRulesByCategoryId;
using OysterFx.AppCore.AppServices.Queries;
using OysterFx.AppCore.Shared.Queries.Common;

public class GetRequiredCategoryAttributeRulesByCategoryIdQueryHandler : QueryHandler<GetRequiredCategoryAttributeRulesByCategoryIdQuery, GetRequiredCategoryAttributeRulesByCategoryIdQueryResult>
{
    private readonly ICategoryQueryRepository _repository;

    public GetRequiredCategoryAttributeRulesByCategoryIdQueryHandler(ICategoryQueryRepository repository)
    {
        _repository = repository;
    }

    public override async Task<QueryResult<GetRequiredCategoryAttributeRulesByCategoryIdQueryResult>> ExecuteAsync(GetRequiredCategoryAttributeRulesByCategoryIdQuery request)
    {
        var category = await _repository.GetByIdAsync(request.CategoryId);
        if (category is null)
            return QueryResult<GetRequiredCategoryAttributeRulesByCategoryIdQueryResult>.Fail("Category was not found.", "NOT_FOUND");

        var items = await _repository.GetCategoryAttributeRulesByCategoryIdAsync(request.CategoryId, request.IncludeInherited, request.IncludeInactive);
        return QueryResult<GetRequiredCategoryAttributeRulesByCategoryIdQueryResult>.Success(new GetRequiredCategoryAttributeRulesByCategoryIdQueryResult
        {
            CategoryBusinessKey = request.CategoryId,
            Items = items.Where(x => x.RuleIsRequired).ToList()
        });
    }
}
