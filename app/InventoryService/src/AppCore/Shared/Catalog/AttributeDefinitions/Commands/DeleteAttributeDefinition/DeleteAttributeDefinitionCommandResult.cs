namespace Insurance.InventoryService.AppCore.Shared.Catalog.AttributeDefinitions.Commands.DeleteAttributeDefinition;

public class DeleteAttributeDefinitionCommandResult
{
    public Guid AttributeDefinitionBusinessKey { get; set; }
    public bool Deleted { get; set; }
}
