namespace Insurance.InventoryService.AppCore.Shared.Catalog.Categories.Queries.GetRootCategories;

using Insurance.InventoryService.AppCore.Shared.Catalog.Categories.Queries.Common;

public class GetRootCategoriesQueryResult
{
    public List<CategoryListItem> Items { get; set; } = new();
}
