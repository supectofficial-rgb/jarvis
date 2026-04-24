namespace Insurance.InventoryService.AppCore.Shared.Catalog.Categories.Queries.GetChildCategories;

using Insurance.InventoryService.AppCore.Shared.Catalog.Categories.Queries.Common;
using OysterFx.AppCore.Shared.Queries;

public class GetChildCategoriesQuery : IQuery<GetChildCategoriesQueryResult>
{
    public GetChildCategoriesQuery(Guid parentCategoryId, bool includeInactive = false)
    {
        ParentCategoryId = parentCategoryId;
        IncludeInactive = includeInactive;
    }

    public Guid ParentCategoryId { get; }
    public bool IncludeInactive { get; }
}
