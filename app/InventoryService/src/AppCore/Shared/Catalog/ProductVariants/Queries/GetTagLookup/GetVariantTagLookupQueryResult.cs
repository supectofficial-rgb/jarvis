namespace Insurance.InventoryService.AppCore.Shared.Catalog.ProductVariants.Queries.GetTagLookup;

using Insurance.InventoryService.AppCore.Shared.Catalog.ProductVariants.Queries.Common;

public sealed class GetVariantTagLookupQueryResult
{
    public List<VariantTagLookupItem> Items { get; set; } = new();
}
