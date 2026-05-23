namespace Insurance.InventoryService.AppCore.Shared.Warehouse.LocationStructures.Commands.CreateLocationStructureValue;

using OysterFx.AppCore.Shared.Commands;

public sealed class CreateLocationStructureValueCommand : ICommand<CreateLocationStructureValueCommandResult>
{
    public Guid StructureRef { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public int DisplayOrder { get; set; }
}
