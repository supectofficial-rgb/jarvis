namespace Insurance.InventoryService.AppCore.Shared.Catalog.Categories.Queries.GetCategoryBreadcrumb;

using Insurance.InventoryService.AppCore.Shared.Catalog.Categories.Queries.Common;
using OysterFx.AppCore.Shared.Queries;

public class GetCategoryBreadcrumbQuery : IQuery<GetCategoryBreadcrumbQueryResult>
{
    public GetCategoryBreadcrumbQuery(Guid categoryId)
    {
        CategoryId = categoryId;
    }

    public Guid CategoryId { get; }
}
