namespace Insurance.InventoryService.AppCore.Shared.Catalog.Categories.Commands.DeactivateCategoryAttributeRule;

using OysterFx.AppCore.Shared.Commands;

public class DeactivateCategoryAttributeRuleCommand : ICommand<DeactivateCategoryAttributeRuleCommandResult>
{
    public Guid CategoryBusinessKey { get; set; }
    public Guid AttributeRef { get; set; }
}
