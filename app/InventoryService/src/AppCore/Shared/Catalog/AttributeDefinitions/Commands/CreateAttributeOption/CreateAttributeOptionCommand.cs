namespace Insurance.InventoryService.AppCore.Shared.Catalog.AttributeDefinitions.Commands.CreateAttributeOption;

using OysterFx.AppCore.Shared.Commands;

public class CreateAttributeOptionCommand : ICommand<CreateAttributeOptionCommandResult>
{
    public Guid AttributeDefinitionBusinessKey { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public int DisplayOrder { get; set; }
}
