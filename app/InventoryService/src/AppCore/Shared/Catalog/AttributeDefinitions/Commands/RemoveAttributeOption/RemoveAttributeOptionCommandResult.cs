namespace Insurance.InventoryService.AppCore.Shared.Catalog.AttributeDefinitions.Commands.RemoveAttributeOption;

public class RemoveAttributeOptionCommandResult
{
    public Guid AttributeDefinitionBusinessKey { get; set; }
    public string Value { get; set; } = string.Empty;
    public bool Removed { get; set; }
}
