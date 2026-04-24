namespace Insurance.InventoryService.AppCore.Shared.Catalog.AttributeDefinitions.Commands.DeleteAttributeOption;

public class DeleteAttributeOptionCommandResult
{
    public Guid AttributeDefinitionBusinessKey { get; set; }
    public Guid OptionBusinessKey { get; set; }
    public bool Removed { get; set; }
}
