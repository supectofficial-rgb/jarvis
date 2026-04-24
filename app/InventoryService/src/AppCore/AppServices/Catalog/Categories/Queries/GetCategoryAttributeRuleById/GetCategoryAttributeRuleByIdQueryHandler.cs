namespace Insurance.InventoryService.AppCore.AppServices.Catalog.Categories.Queries.GetCategoryAttributeRuleById;

using Insurance.InventoryService.AppCore.Shared.Catalog.Categories.Queries;
using Insurance.InventoryService.AppCore.Shared.Catalog.Categories.Queries.GetCategoryAttributeRuleById;
using OysterFx.AppCore.AppServices.Queries;
using OysterFx.AppCore.Shared.Queries.Common;

public class GetCategoryAttributeRuleByIdQueryHandler : QueryHandler<GetCategoryAttributeRuleByIdQuery, GetCategoryAttributeRuleByIdQueryResult>
{
    private readonly ICategoryQueryRepository _repository;

    public GetCategoryAttributeRuleByIdQueryHandler(ICategoryQueryRepository repository)
    {
        _repository = repository;
    }

    public override async Task<QueryResult<GetCategoryAttributeRuleByIdQueryResult>> ExecuteAsync(GetCategoryAttributeRuleByIdQuery request)
    {
        var item = await _repository.GetCategoryAttributeRuleByIdAsync(request.CategoryAttributeRuleId);
        if (item is null)
            return QueryResult<GetCategoryAttributeRuleByIdQueryResult>.Fail("Category attribute rule was not found.", "NOT_FOUND");

        return QueryResult<GetCategoryAttributeRuleByIdQueryResult>.Success(new GetCategoryAttributeRuleByIdQueryResult { Item = item });
    }
}
