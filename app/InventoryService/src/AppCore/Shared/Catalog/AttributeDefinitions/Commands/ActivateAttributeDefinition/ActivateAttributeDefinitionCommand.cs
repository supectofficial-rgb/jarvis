namespace Insurance.InventoryService.AppCore.Shared.Catalog.AttributeDefinitions.Commands.ActivateAttributeDefinition;

using OysterFx.AppCore.Shared.Commands;

public class ActivateAttributeDefinitionCommand : ICommand<ActivateAttributeDefinitionCommandResult>
{
    public Guid AttributeDefinitionBusinessKey { get; set; }
}
