namespace Insurance.InventoryService.AppCore.Shared.Catalog.AttributeDefinitions.Commands.ActivateAttributeDefinition;

public class ActivateAttributeDefinitionCommandResult
{
    public Guid AttributeDefinitionBusinessKey { get; set; }
    public bool IsActive { get; set; }
}
