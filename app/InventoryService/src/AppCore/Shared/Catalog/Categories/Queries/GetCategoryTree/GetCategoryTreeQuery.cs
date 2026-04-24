namespace Insurance.InventoryService.AppCore.Shared.Catalog.Categories.Queries.GetCategoryTree;

using Insurance.InventoryService.AppCore.Shared.Catalog.Categories.Queries.Common;
using OysterFx.AppCore.Shared.Queries;

public class GetCategoryTreeQuery : IQuery<GetCategoryTreeQueryResult>
{
    public GetCategoryTreeQuery(bool includeInactive = false)
    {
        IncludeInactive = includeInactive;
    }

    public bool IncludeInactive { get; }
}
