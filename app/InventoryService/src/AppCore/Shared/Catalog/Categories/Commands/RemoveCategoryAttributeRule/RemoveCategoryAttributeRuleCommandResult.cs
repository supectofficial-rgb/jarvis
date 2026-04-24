namespace Insurance.InventoryService.AppCore.Shared.Catalog.Categories.Commands.RemoveCategoryAttributeRule;

public class RemoveCategoryAttributeRuleCommandResult
{
    public Guid CategoryBusinessKey { get; set; }
    public Guid CategorySchemaVersionRef { get; set; }
    public Guid AttributeRef { get; set; }
    public bool Removed { get; set; }
}
