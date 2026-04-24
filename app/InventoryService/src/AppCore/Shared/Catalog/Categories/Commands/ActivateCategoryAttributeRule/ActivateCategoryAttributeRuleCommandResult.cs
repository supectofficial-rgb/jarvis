namespace Insurance.InventoryService.AppCore.Shared.Catalog.Categories.Commands.ActivateCategoryAttributeRule;

public class ActivateCategoryAttributeRuleCommandResult
{
    public Guid CategoryBusinessKey { get; set; }
    public Guid CategorySchemaVersionRef { get; set; }
    public Guid AttributeRef { get; set; }
    public bool IsActive { get; set; }
}
