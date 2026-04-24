namespace Insurance.InventoryService.AppCore.Shared.Catalog.Categories.Queries.GetRequiredCategoryAttributeRulesByCategoryId;

using Insurance.InventoryService.AppCore.Shared.Catalog.Categories.Queries.Common;

public class GetRequiredCategoryAttributeRulesByCategoryIdQueryResult
{
    public Guid CategoryBusinessKey { get; set; }
    public List<CategoryAttributeRuleViewItem> Items { get; set; } = new();
}
