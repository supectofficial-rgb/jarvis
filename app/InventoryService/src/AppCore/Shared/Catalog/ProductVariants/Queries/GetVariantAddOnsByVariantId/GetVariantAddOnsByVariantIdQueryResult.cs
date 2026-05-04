namespace Insurance.InventoryService.AppCore.Shared.Catalog.ProductVariants.Queries.GetVariantAddOnsByVariantId;

using Insurance.InventoryService.AppCore.Shared.Catalog.ProductVariants.Queries.Common;

public class GetVariantAddOnsByVariantIdQueryResult
{
    public List<VariantAddOnViewItem> Items { get; set; } = new();
}
