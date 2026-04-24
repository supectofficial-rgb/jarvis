namespace Insurance.InventoryService.AppCore.Shared.Catalog.Categories.Queries.GetCategoryTree;

using Insurance.InventoryService.AppCore.Shared.Catalog.Categories.Queries.Common;

public class GetCategoryTreeQueryResult
{
    public List<CategoryTreeItem> Items { get; set; } = new();
}
