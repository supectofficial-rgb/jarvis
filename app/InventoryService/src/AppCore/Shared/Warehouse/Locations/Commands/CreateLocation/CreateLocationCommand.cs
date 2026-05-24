namespace Insurance.InventoryService.AppCore.Shared.Warehouse.Locations.Commands.CreateLocation;

using Insurance.InventoryService.AppCore.Shared.Warehouse.Locations.Commands;
using OysterFx.AppCore.Shared.Commands;

public class CreateLocationCommand : ICommand<CreateLocationCommandResult>
{
    public Guid WarehouseRef { get; set; }
    public string LocationCode { get; set; } = string.Empty;
    public string LocationType { get; set; } = string.Empty;
    public List<LocationStructureSelectionItem> StructureSelections { get; set; } = new();
}
