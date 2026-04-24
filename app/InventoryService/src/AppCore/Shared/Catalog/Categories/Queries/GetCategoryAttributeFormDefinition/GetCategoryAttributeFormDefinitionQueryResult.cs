namespace Insurance.InventoryService.AppCore.Shared.Catalog.Categories.Queries.GetCategoryAttributeFormDefinition;

using Insurance.InventoryService.AppCore.Shared.Catalog.Categories.Queries.Common;

public class GetCategoryAttributeFormDefinitionQueryResult
{
    public Guid CategoryBusinessKey { get; set; }
    public string CategoryCode { get; set; } = string.Empty;
    public string CategoryName { get; set; } = string.Empty;
    public CategoryAttributeFormDefinitionItem FormDefinition { get; set; } = new();
}
