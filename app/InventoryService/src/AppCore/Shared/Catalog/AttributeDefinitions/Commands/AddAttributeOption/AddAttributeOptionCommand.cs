namespace Insurance.InventoryService.AppCore.Shared.Catalog.AttributeDefinitions.Commands.AddAttributeOption;

using OysterFx.AppCore.Shared.Commands;

public class AddAttributeOptionCommand : ICommand<AddAttributeOptionCommandResult>
{
    public Guid AttributeDefinitionBusinessKey { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public int DisplayOrder { get; set; }
}
