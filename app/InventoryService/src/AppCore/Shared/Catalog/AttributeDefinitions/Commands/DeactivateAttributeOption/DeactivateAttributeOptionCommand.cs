namespace Insurance.InventoryService.AppCore.Shared.Catalog.AttributeDefinitions.Commands.DeactivateAttributeOption;

using OysterFx.AppCore.Shared.Commands;

public class DeactivateAttributeOptionCommand : ICommand<DeactivateAttributeOptionCommandResult>
{
    public Guid AttributeDefinitionBusinessKey { get; set; }
    public Guid OptionBusinessKey { get; set; }
}
