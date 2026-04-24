namespace Insurance.InventoryService.AppCore.Shared.Catalog.Categories.Queries.GetCategoryCatalogSetup;

using Insurance.InventoryService.AppCore.Shared.Catalog.Categories.Queries.Common;

public class GetCategoryCatalogSetupQueryResult
{
    public CategoryCatalogSetupItem? Item { get; set; }
}

public class CategoryCatalogSetupItem
{
    public Guid CategoryBusinessKey { get; set; }
    public string CategoryCode { get; set; } = string.Empty;
    public string CategoryName { get; set; } = string.Empty;
    public bool IncludeInherited { get; set; }
    public bool IncludeInactive { get; set; }
    public List<CategoryAttributeRuleViewItem> Rules { get; set; } = new();
    public List<CategoryAttributeRuleViewItem> ProductLevelRules { get; set; } = new();
    public List<CategoryAttributeRuleViewItem> VariantLevelRules { get; set; } = new();
}
