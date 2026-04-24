namespace Insurance.InventoryService.AppCore.Shared.Catalog.Categories.Queries.GetActiveCategoryAttributeRulesByCategoryId;

using OysterFx.AppCore.Shared.Queries;

public class GetActiveCategoryAttributeRulesByCategoryIdQuery : IQuery<GetActiveCategoryAttributeRulesByCategoryIdQueryResult>
{
    public GetActiveCategoryAttributeRulesByCategoryIdQuery(Guid categoryId, bool includeInherited = true)
    {
        CategoryId = categoryId;
        IncludeInherited = includeInherited;
    }

    public Guid CategoryId { get; }
    public bool IncludeInherited { get; }
}
