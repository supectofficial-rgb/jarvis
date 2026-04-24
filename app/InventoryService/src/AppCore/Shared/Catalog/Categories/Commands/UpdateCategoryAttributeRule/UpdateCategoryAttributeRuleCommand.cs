namespace Insurance.InventoryService.AppCore.Shared.Catalog.Categories.Commands.UpdateCategoryAttributeRule;

using OysterFx.AppCore.Shared.Commands;

public class UpdateCategoryAttributeRuleCommand : ICommand<UpdateCategoryAttributeRuleCommandResult>
{
    public Guid CategoryBusinessKey { get; set; }
    public Guid AttributeRef { get; set; }
    public bool IsRequired { get; set; }
    public bool IsVariant { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsOverridden { get; set; }
    public bool IsActive { get; set; }
}
