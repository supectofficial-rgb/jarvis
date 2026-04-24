namespace Insurance.InventoryService.AppCore.Shared.Catalog.Products.Queries.GetActiveProducts;

using Insurance.InventoryService.AppCore.Shared.Catalog.Products.Queries.Common;

public class GetActiveProductsQueryResult
{
    public List<ProductListItem> Items { get; set; } = new();
}
