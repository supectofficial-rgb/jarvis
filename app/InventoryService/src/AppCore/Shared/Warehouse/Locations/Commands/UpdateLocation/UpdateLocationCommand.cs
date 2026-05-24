namespace Insurance.InventoryService.AppCore.Shared.Warehouse.Locations.Commands.UpdateLocation;

using Insurance.InventoryService.AppCore.Shared.Warehouse.Locations.Commands;
using OysterFx.AppCore.Shared.Commands;

public class UpdateLocationCommand : ICommand<UpdateLocationCommandResult>
{
    public Guid LocationBusinessKey { get; set; }
    public Guid WarehouseRef { get; set; }
    public string LocationCode { get; set; } = string.Empty;
    public string LocationType { get; set; } = string.Empty;
    public List<LocationStructureSelectionItem> StructureSelections { get; set; } = new();
    public bool IsActive { get; set; } = true;
}
