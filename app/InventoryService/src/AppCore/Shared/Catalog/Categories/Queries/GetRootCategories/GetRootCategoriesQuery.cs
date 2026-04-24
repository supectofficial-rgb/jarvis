namespace Insurance.InventoryService.AppCore.Shared.Catalog.Categories.Queries.GetRootCategories;

using Insurance.InventoryService.AppCore.Shared.Catalog.Categories.Queries.Common;
using OysterFx.AppCore.Shared.Queries;

public class GetRootCategoriesQuery : IQuery<GetRootCategoriesQueryResult>
{
    public GetRootCategoriesQuery(bool includeInactive = false)
    {
        IncludeInactive = includeInactive;
    }

    public bool IncludeInactive { get; }
}
