namespace Insurance.InventoryService.AppCore.Shared.Catalog.Categories.Commands.RemoveCategoryAttributeRule;

using OysterFx.AppCore.Shared.Commands;

public class RemoveCategoryAttributeRuleCommand : ICommand<RemoveCategoryAttributeRuleCommandResult>
{
    public Guid CategoryBusinessKey { get; set; }
    public Guid AttributeRef { get; set; }
}
