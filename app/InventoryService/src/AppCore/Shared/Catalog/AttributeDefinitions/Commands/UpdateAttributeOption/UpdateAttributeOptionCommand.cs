namespace Insurance.InventoryService.AppCore.Shared.Catalog.AttributeDefinitions.Commands.UpdateAttributeOption;

using OysterFx.AppCore.Shared.Commands;

public class UpdateAttributeOptionCommand : ICommand<UpdateAttributeOptionCommandResult>
{
    public Guid AttributeDefinitionBusinessKey { get; set; }
    public Guid OptionBusinessKey { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public int DisplayOrder { get; set; }
}
