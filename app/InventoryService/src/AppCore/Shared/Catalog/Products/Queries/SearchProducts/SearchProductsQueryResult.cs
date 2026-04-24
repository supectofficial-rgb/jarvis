namespace Insurance.InventoryService.AppCore.Shared.Catalog.Products.Queries.SearchProducts;

using Insurance.InventoryService.AppCore.Shared.Catalog.Products.Queries.Common;

public class SearchProductsQueryResult
{
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public List<ProductListItem> Items { get; set; } = new();
}
