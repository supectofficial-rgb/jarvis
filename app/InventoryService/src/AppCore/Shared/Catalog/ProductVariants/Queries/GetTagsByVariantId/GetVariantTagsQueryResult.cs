namespace Insurance.InventoryService.AppCore.Shared.Catalog.ProductVariants.Queries.GetTagsByVariantId;

using Insurance.InventoryService.AppCore.Shared.Catalog.ProductVariants.Queries.Common;

public class GetVariantTagsQueryResult
{
    public List<VariantTagViewItem> Items { get; set; } = new();
}
