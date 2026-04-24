namespace Insurance.InventoryService.AppCore.Shared.Catalog.AttributeDefinitions.Commands.ActivateAttributeOption;

public class ActivateAttributeOptionCommandResult
{
    public Guid AttributeDefinitionBusinessKey { get; set; }
    public Guid OptionBusinessKey { get; set; }
    public bool IsActive { get; set; }
}
