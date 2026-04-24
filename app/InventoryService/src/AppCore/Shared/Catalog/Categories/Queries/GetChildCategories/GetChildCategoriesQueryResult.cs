namespace Insurance.InventoryService.AppCore.Shared.Catalog.Categories.Queries.GetChildCategories;

using Insurance.InventoryService.AppCore.Shared.Catalog.Categories.Queries.Common;

public class GetChildCategoriesQueryResult
{
    public Guid ParentCategoryId { get; set; }
    public List<CategoryListItem> Items { get; set; } = new();
}
