namespace Insurance.InventoryService.AppCore.Shared.Catalog.AttributeDefinitions.Commands.DeactivateAttributeOption;

public class DeactivateAttributeOptionCommandResult
{
    public Guid AttributeDefinitionBusinessKey { get; set; }
    public Guid OptionBusinessKey { get; set; }
    public bool IsActive { get; set; }
}
