namespace Insurance.InventoryService.AppCore.Shared.Catalog.AttributeDefinitions.Commands.RemoveAttributeOption;

using OysterFx.AppCore.Shared.Commands;

public class RemoveAttributeOptionCommand : ICommand<RemoveAttributeOptionCommandResult>
{
    public Guid AttributeDefinitionBusinessKey { get; set; }
    public string Value { get; set; } = string.Empty;
}
