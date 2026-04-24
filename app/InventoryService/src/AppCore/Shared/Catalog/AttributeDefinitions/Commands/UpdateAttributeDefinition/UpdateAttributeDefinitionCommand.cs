namespace Insurance.InventoryService.AppCore.Shared.Catalog.AttributeDefinitions.Commands.UpdateAttributeDefinition;

using OysterFx.AppCore.Shared.Commands;

public class UpdateAttributeDefinitionCommand : ICommand<UpdateAttributeDefinitionCommandResult>
{
    public Guid AttributeDefinitionBusinessKey { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string DataType { get; set; } = string.Empty;
    public string Scope { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public List<UpdateAttributeOptionItem> Options { get; set; } = new();
}

public class UpdateAttributeOptionItem
{
    public string Name { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; } = true;
}
