namespace Insurance.InventoryService.AppCore.Shared.Catalog.Categories.Queries.GetCategoryAttributeFormDefinition;

using OysterFx.AppCore.Shared.Queries;

public class GetCategoryAttributeFormDefinitionQuery : IQuery<GetCategoryAttributeFormDefinitionQueryResult>
{
    public GetCategoryAttributeFormDefinitionQuery(Guid categoryId, bool includeInherited = true, bool includeInactive = false)
    {
        CategoryId = categoryId;
        IncludeInherited = includeInherited;
        IncludeInactive = includeInactive;
    }

    public Guid CategoryId { get; }
    public bool IncludeInherited { get; }
    public bool IncludeInactive { get; }
}
