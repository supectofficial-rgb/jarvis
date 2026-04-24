namespace Insurance.InventoryService.AppCore.Shared.Catalog.Products.Queries.GetByCategoryId;

using Insurance.InventoryService.AppCore.Shared.Catalog.Products.Queries.Common;

public class GetProductsByCategoryIdQueryResult
{
    public List<ProductListItem> Items { get; set; } = new();
}
