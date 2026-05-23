namespace Insurance.InventoryService.AppCore.Shared.Warehouse.LocationStructures.Commands.CreateLocationStructureNode;

using OysterFx.AppCore.Shared.Commands;

public sealed class CreateLocationStructureNodeCommand : ICommand<CreateLocationStructureNodeCommandResult>
{
    public Guid WarehouseRef { get; set; }
    public Guid? ParentStructureRef { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public int DisplayOrder { get; set; }
}
