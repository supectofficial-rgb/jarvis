namespace Insurance.InventoryService.AppCore.Shared.Catalog.Categories.Queries.GetCategoryBreadcrumb;

using Insurance.InventoryService.AppCore.Shared.Catalog.Categories.Queries.Common;

public class GetCategoryBreadcrumbQueryResult
{
    public Guid CategoryBusinessKey { get; set; }
    public List<CategoryBreadcrumbItem> Items { get; set; } = new();
}
