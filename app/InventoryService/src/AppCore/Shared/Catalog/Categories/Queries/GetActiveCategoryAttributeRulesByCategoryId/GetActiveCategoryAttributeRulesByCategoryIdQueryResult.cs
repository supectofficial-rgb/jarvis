namespace Insurance.InventoryService.AppCore.Shared.Catalog.Categories.Queries.GetActiveCategoryAttributeRulesByCategoryId;

using Insurance.InventoryService.AppCore.Shared.Catalog.Categories.Queries.Common;

public class GetActiveCategoryAttributeRulesByCategoryIdQueryResult
{
    public Guid CategoryBusinessKey { get; set; }
    public List<CategoryAttributeRuleViewItem> Items { get; set; } = new();
}
