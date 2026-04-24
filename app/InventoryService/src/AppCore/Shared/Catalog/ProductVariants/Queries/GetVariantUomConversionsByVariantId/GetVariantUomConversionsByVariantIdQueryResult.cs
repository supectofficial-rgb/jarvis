namespace Insurance.InventoryService.AppCore.Shared.Catalog.ProductVariants.Queries.GetVariantUomConversionsByVariantId;

using Insurance.InventoryService.AppCore.Shared.Catalog.ProductVariants.Queries.Common;

public class GetVariantUomConversionsByVariantIdQueryResult
{
    public List<VariantUomConversionViewItem> Items { get; set; } = new();
}
