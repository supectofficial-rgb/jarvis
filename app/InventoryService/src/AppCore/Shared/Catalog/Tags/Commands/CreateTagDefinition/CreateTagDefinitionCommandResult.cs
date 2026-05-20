namespace Insurance.InventoryService.AppCore.Shared.Catalog.Tags.Commands.CreateTagDefinition;

public class CreateTagDefinitionCommandResult
{
    public Guid TagBusinessKey { get; set; }
    public string TagName { get; set; } = string.Empty;
    public string? TagColor { get; set; }
}
