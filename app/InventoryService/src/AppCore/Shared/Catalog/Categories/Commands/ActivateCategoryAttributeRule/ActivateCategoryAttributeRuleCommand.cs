namespace Insurance.InventoryService.AppCore.Shared.Catalog.Categories.Commands.ActivateCategoryAttributeRule;

using OysterFx.AppCore.Shared.Commands;

public class ActivateCategoryAttributeRuleCommand : ICommand<ActivateCategoryAttributeRuleCommandResult>
{
    public Guid CategoryBusinessKey { get; set; }
    public Guid AttributeRef { get; set; }
}
