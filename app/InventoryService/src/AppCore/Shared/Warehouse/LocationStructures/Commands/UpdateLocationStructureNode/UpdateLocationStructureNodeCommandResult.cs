namespace Insurance.InventoryService.AppCore.Shared.Warehouse.LocationStructures.Commands.UpdateLocationStructureNode;

public sealed class UpdateLocationStructureNodeCommandResult
{
    public Guid LocationStructureBusinessKey { get; set; }
    public Guid WarehouseRef { get; set; }
    public Guid? ParentStructureRef { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; }
}
