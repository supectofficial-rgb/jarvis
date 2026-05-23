namespace Insurance.InventoryService.AppCore.Shared.Warehouse.LocationStructures.Commands.UpdateLocationStructureNode;

using OysterFx.AppCore.Shared.Commands;

public sealed class UpdateLocationStructureNodeCommand : ICommand<UpdateLocationStructureNodeCommandResult>
{
    public Guid LocationStructureBusinessKey { get; set; }
    public Guid WarehouseRef { get; set; }
    public Guid? ParentStructureRef { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; } = true;
}
