namespace Insurance.InventoryService.AppCore.AppServices.Catalog.Categories.Queries.GetCategoryCatalogSetup;

using Insurance.InventoryService.AppCore.Shared.Catalog.Categories.Queries;
using Insurance.InventoryService.AppCore.Shared.Catalog.Categories.Queries.GetCategoryCatalogSetup;
using OysterFx.AppCore.AppServices.Queries;
using OysterFx.AppCore.Shared.Queries.Common;

public class GetCategoryCatalogSetupQueryHandler : QueryHandler<GetCategoryCatalogSetupQuery, GetCategoryCatalogSetupQueryResult>
{
    private readonly ICategoryQueryRepository _repository;

    public GetCategoryCatalogSetupQueryHandler(ICategoryQueryRepository repository)
    {
        _repository = repository;
    }

    public override async Task<QueryResult<GetCategoryCatalogSetupQueryResult>> ExecuteAsync(GetCategoryCatalogSetupQuery request)
    {
        var category = await _repository.GetByIdAsync(request.CategoryId);
        if (category is null)
            return QueryResult<GetCategoryCatalogSetupQueryResult>.Fail("Category was not found.", "NOT_FOUND");

        var rules = await _repository.GetCategoryAttributeRulesByCategoryIdAsync(request.CategoryId, request.IncludeInherited, request.IncludeInactive);

        var item = new CategoryCatalogSetupItem
        {
            CategoryBusinessKey = category.CategoryBusinessKey,
            CategoryCode = category.Code,
            CategoryName = category.Name,
            IncludeInherited = request.IncludeInherited,
            IncludeInactive = request.IncludeInactive,
            Rules = rules,
            ProductLevelRules = rules.Where(x => !x.RuleIsVariant).ToList(),
            VariantLevelRules = rules.Where(x => x.RuleIsVariant).ToList()
        };

        return QueryResult<GetCategoryCatalogSetupQueryResult>.Success(new GetCategoryCatalogSetupQueryResult { Item = item });
    }
}
