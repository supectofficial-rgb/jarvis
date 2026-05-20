namespace Insurance.InventoryService.AppCore.Shared.Catalog.Tags.Commands.CreateTagDefinition;

using OysterFx.AppCore.Shared.Commands;

public class CreateTagDefinitionCommand : ICommand<CreateTagDefinitionCommandResult>
{
    public string TagName { get; set; } = string.Empty;
    public string? TagColor { get; set; }
}
