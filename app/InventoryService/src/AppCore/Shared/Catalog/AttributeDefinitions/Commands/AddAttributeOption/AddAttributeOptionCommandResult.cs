namespace Insurance.InventoryService.AppCore.Shared.Catalog.AttributeDefinitions.Commands.AddAttributeOption;

public class AddAttributeOptionCommandResult
{
    public Guid AttributeDefinitionBusinessKey { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; }
}
