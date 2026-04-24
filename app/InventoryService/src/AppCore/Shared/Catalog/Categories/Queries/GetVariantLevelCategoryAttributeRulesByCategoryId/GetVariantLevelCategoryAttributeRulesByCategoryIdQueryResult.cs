namespace Insurance.InventoryService.AppCore.Shared.Catalog.Categories.Queries.GetVariantLevelCategoryAttributeRulesByCategoryId;

using Insurance.InventoryService.AppCore.Shared.Catalog.Categories.Queries.Common;

public class GetVariantLevelCategoryAttributeRulesByCategoryIdQueryResult
{
    public Guid CategoryBusinessKey { get; set; }
    public List<CategoryAttributeRuleViewItem> Items { get; set; } = new();
}
