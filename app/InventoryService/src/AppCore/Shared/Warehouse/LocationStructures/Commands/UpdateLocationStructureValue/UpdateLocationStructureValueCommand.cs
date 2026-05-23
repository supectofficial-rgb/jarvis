namespace Insurance.InventoryService.AppCore.Shared.Warehouse.LocationStructures.Commands.UpdateLocationStructureValue;

using OysterFx.AppCore.Shared.Commands;

public sealed class UpdateLocationStructureValueCommand : ICommand<UpdateLocationStructureValueCommandResult>
{
    public Guid LocationStructureValueBusinessKey { get; set; }
    public Guid StructureRef { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; } = true;
}
