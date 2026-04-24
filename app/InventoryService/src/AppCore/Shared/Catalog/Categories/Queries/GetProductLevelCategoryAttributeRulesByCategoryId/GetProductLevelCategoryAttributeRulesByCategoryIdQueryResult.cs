namespace Insurance.InventoryService.AppCore.Shared.Catalog.Categories.Queries.GetProductLevelCategoryAttributeRulesByCategoryId;

using Insurance.InventoryService.AppCore.Shared.Catalog.Categories.Queries.Common;

public class GetProductLevelCategoryAttributeRulesByCategoryIdQueryResult
{
    public Guid CategoryBusinessKey { get; set; }
    public List<CategoryAttributeRuleViewItem> Items { get; set; } = new();
}
