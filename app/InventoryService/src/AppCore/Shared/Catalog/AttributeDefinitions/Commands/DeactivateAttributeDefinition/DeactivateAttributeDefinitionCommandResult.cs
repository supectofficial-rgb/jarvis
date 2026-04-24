namespace Insurance.InventoryService.AppCore.Shared.Catalog.AttributeDefinitions.Commands.DeactivateAttributeDefinition;

public class DeactivateAttributeDefinitionCommandResult
{
    public Guid AttributeDefinitionBusinessKey { get; set; }
    public bool IsActive { get; set; }
}
