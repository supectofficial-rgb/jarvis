namespace Insurance.InventoryService.AppCore.Shared.Catalog.Products.Queries.GetMissingRequiredAttributes;

using Insurance.InventoryService.AppCore.Shared.Catalog.Products.Queries.Common;

public class GetMissingRequiredProductAttributesQueryResult
{
    public List<MissingRequiredProductAttributeItem> Items { get; set; } = new();
}
