namespace Insurance.InventoryService.AppCore.Shared.Catalog.Products.Queries.GetAttributeValuesByProductId;

using Insurance.InventoryService.AppCore.Shared.Catalog.Products.Queries.Common;

public class GetProductAttributeValuesByProductIdQueryResult
{
    public List<ProductAttributeValueViewItem> Items { get; set; } = new();
}
