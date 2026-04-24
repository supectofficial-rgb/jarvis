namespace Insurance.InventoryService.AppCore.Shared.Catalog.Categories.Queries.GetActiveCategories;

using Insurance.InventoryService.AppCore.Shared.Catalog.Categories.Queries.Common;

public class GetActiveCategoriesQueryResult
{
    public List<CategoryListItem> Items { get; set; } = new();
}
