namespace Insurance.InventoryService.AppCore.Shared.Warehouse.Locations.Commands.CreateLocation;

using OysterFx.AppCore.Shared.Commands;

public class CreateLocationCommand : ICommand<CreateLocationCommandResult>
{
    public Guid WarehouseRef { get; set; }
    public string LocationCode { get; set; } = string.Empty;
    public string LocationType { get; set; } = string.Empty;
    public string? Aisle { get; set; }
    public string? Rack { get; set; }
    public string? Shelf { get; set; }
    public string? Bin { get; set; }
}
