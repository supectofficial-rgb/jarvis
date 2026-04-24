namespace Insurance.InventoryService.AppCore.Shared.Catalog.Categories.Queries.GetVariantLevelCategoryAttributeRulesByCategoryId;

using OysterFx.AppCore.Shared.Queries;

public class GetVariantLevelCategoryAttributeRulesByCategoryIdQuery : IQuery<GetVariantLevelCategoryAttributeRulesByCategoryIdQueryResult>
{
    public GetVariantLevelCategoryAttributeRulesByCategoryIdQuery(Guid categoryId, bool includeInherited = true, bool includeInactive = false)
    {
        CategoryId = categoryId;
        IncludeInherited = includeInherited;
        IncludeInactive = includeInactive;
    }

    public Guid CategoryId { get; }
    public bool IncludeInherited { get; }
    public bool IncludeInactive { get; }
}
