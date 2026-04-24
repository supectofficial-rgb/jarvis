namespace Insurance.InventoryService.AppCore.Shared.Catalog.AttributeDefinitions.Commands.DeleteAttributeOption;

using OysterFx.AppCore.Shared.Commands;

public class DeleteAttributeOptionCommand : ICommand<DeleteAttributeOptionCommandResult>
{
    public Guid AttributeDefinitionBusinessKey { get; set; }
    public Guid OptionBusinessKey { get; set; }
}
