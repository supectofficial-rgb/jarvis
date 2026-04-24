namespace Insurance.InventoryService.AppCore.Shared.Catalog.Categories.Commands.UpdateCategory;

using OysterFx.AppCore.Shared.Commands;

public class UpdateCategoryCommand : ICommand<UpdateCategoryCommandResult>
{
    public Guid CategoryBusinessKey { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public int DisplayOrder { get; set; }
    public Guid? ParentCategoryRef { get; set; }
    public bool IsActive { get; set; } = true;
    public List<UpdateCategoryAttributeRuleItem> AttributeRules { get; set; } = new();
}

public class UpdateCategoryAttributeRuleItem
{
    public Guid AttributeRef { get; set; }
    public bool IsRequired { get; set; }
    public bool IsVariant { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsOverridden { get; set; }
    public bool IsActive { get; set; } = true;
}
