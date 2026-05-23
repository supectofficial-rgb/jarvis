namespace Insurance.InventoryService.AppCore.Shared.Warehouse.LocationStructures.Commands.UpdateLocationStructureValue;

public sealed class UpdateLocationStructureValueCommandResult
{
    public Guid LocationStructureValueBusinessKey { get; set; }
    public Guid StructureRef { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; }
}
