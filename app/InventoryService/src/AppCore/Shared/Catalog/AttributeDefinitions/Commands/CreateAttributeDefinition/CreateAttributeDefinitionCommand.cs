namespace Insurance.InventoryService.AppCore.Shared.Catalog.AttributeDefinitions.Commands.CreateAttributeDefinition;

using OysterFx.AppCore.Shared.Commands;

public class CreateAttributeDefinitionCommand : ICommand<CreateAttributeDefinitionCommandResult>
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string DataType { get; set; } = string.Empty;
    public string Scope { get; set; } = string.Empty;
    public List<CreateAttributeOptionItem> Options { get; set; } = new();
}

public class CreateAttributeOptionItem
{
    public string Name { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public int DisplayOrder { get; set; }
}
