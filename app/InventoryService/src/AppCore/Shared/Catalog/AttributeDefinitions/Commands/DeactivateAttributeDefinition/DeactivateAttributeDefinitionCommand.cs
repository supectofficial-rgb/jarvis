namespace Insurance.InventoryService.AppCore.Shared.Catalog.AttributeDefinitions.Commands.DeactivateAttributeDefinition;

using OysterFx.AppCore.Shared.Commands;

public class DeactivateAttributeDefinitionCommand : ICommand<DeactivateAttributeDefinitionCommandResult>
{
    public Guid AttributeDefinitionBusinessKey { get; set; }
}
