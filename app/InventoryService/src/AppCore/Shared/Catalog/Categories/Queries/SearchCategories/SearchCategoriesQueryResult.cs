namespace Insurance.InventoryService.AppCore.Shared.Catalog.Categories.Queries.SearchCategories;

using Insurance.InventoryService.AppCore.Shared.Catalog.Categories.Queries.Common;

public class SearchCategoriesQueryResult
{
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public List<CategoryListItem> Items { get; set; } = new();
}
