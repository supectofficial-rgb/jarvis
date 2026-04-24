namespace Insurance.InventoryService.AppCore.Shared.Catalog.AttributeDefinitions.Commands.CreateAttributeDefinition;

public class CreateAttributeDefinitionCommandResult
{
    public Guid AttributeDefinitionBusinessKey { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string DataType { get; set; } = string.Empty;
    public string Scope { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}
