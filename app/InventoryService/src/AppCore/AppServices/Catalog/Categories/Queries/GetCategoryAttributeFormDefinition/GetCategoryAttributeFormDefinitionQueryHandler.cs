namespace Insurance.InventoryService.AppCore.AppServices.Catalog.Categories.Queries.GetCategoryAttributeFormDefinition;

using Insurance.InventoryService.AppCore.Shared.Catalog.Categories.Queries;
using Insurance.InventoryService.AppCore.Shared.Catalog.Categories.Queries.Common;
using Insurance.InventoryService.AppCore.Shared.Catalog.Categories.Queries.GetCategoryAttributeFormDefinition;
using OysterFx.AppCore.AppServices.Queries;
using OysterFx.AppCore.Shared.Queries.Common;

public class GetCategoryAttributeFormDefinitionQueryHandler : QueryHandler<GetCategoryAttributeFormDefinitionQuery, GetCategoryAttributeFormDefinitionQueryResult>
{
    private readonly ICategoryQueryRepository _repository;

    public GetCategoryAttributeFormDefinitionQueryHandler(ICategoryQueryRepository repository)
    {
        _repository = repository;
    }

    public override async Task<QueryResult<GetCategoryAttributeFormDefinitionQueryResult>> ExecuteAsync(GetCategoryAttributeFormDefinitionQuery request)
    {
        var category = await _repository.GetByIdAsync(request.CategoryId);
        if (category is null)
            return QueryResult<GetCategoryAttributeFormDefinitionQueryResult>.Fail("Category was not found.", "NOT_FOUND");

        var items = await _repository.GetCategoryAttributeRulesByCategoryIdAsync(request.CategoryId, request.IncludeInherited, request.IncludeInactive);

        return QueryResult<GetCategoryAttributeFormDefinitionQueryResult>.Success(new GetCategoryAttributeFormDefinitionQueryResult
        {
            CategoryBusinessKey = category.CategoryBusinessKey,
            CategoryCode = category.Code,
            CategoryName = category.Name,
            FormDefinition = new CategoryAttributeFormDefinitionItem
            {
                ProductLevelAttributes = items.Where(x => !x.RuleIsVariant).ToList(),
                VariantLevelAttributes = items.Where(x => x.RuleIsVariant).ToList()
            }
        });
    }
}
