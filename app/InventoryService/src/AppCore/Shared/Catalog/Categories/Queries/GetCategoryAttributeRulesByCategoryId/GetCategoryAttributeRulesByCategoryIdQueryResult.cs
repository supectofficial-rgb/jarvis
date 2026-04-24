namespace Insurance.InventoryService.AppCore.Shared.Catalog.Categories.Queries.GetCategoryAttributeRulesByCategoryId;

using Insurance.InventoryService.AppCore.Shared.Catalog.Categories.Queries.Common;

public class GetCategoryAttributeRulesByCategoryIdQueryResult
{
    public Guid CategoryBusinessKey { get; set; }
    public List<CategoryAttributeRuleViewItem> Items { get; set; } = new();
}
