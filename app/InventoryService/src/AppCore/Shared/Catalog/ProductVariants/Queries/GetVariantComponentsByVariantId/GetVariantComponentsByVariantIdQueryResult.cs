namespace Insurance.InventoryService.AppCore.Shared.Catalog.ProductVariants.Queries.GetVariantComponentsByVariantId;

using Insurance.InventoryService.AppCore.Shared.Catalog.ProductVariants.Queries.Common;

public class GetVariantComponentsByVariantIdQueryResult
{
    public List<VariantComponentViewItem> Items { get; set; } = new();
}
