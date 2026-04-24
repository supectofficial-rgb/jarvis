namespace Insurance.InventoryService.AppCore.Shared.Catalog.AttributeDefinitions.Commands.DeleteAttributeDefinition;

using OysterFx.AppCore.Shared.Commands;

public class DeleteAttributeDefinitionCommand : ICommand<DeleteAttributeDefinitionCommandResult>
{
    public Guid AttributeDefinitionBusinessKey { get; set; }
}
