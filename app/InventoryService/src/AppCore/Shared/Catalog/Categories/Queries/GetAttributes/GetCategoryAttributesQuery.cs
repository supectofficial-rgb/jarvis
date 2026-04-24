namespace Insurance.InventoryService.AppCore.Shared.Catalog.Categories.Queries.GetAttributes;

using OysterFx.AppCore.Shared.Queries;

public class GetCategoryAttributesQuery : IQuery<GetCategoryAttributesQueryResult>
{
    public GetCategoryAttributesQuery(Guid categoryBusinessKey, bool includeInherited = true, bool includeInactive = false)
    {
        CategoryBusinessKey = categoryBusinessKey;
        IncludeInherited = includeInherited;
        IncludeInactive = includeInactive;
    }

    public Guid CategoryBusinessKey { get; }
    public bool IncludeInherited { get; }
    public bool IncludeInactive { get; }
}
