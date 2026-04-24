namespace Insurance.InventoryService.AppCore.Shared.Catalog.AttributeDefinitions.Commands.ActivateAttributeOption;

using OysterFx.AppCore.Shared.Commands;

public class ActivateAttributeOptionCommand : ICommand<ActivateAttributeOptionCommandResult>
{
    public Guid AttributeDefinitionBusinessKey { get; set; }
    public Guid OptionBusinessKey { get; set; }
}
