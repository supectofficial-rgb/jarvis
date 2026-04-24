namespace Insurance.InventoryService.AppCore.Shared.Catalog.Products.Queries.GetAttributeValuesWithDefinition;

using Insurance.InventoryService.AppCore.Shared.Catalog.Products.Queries.Common;

public class GetProductAttributeValuesWithDefinitionQueryResult
{
    public List<ProductAttributeValueWithDefinitionItem> Items { get; set; } = new();
}
