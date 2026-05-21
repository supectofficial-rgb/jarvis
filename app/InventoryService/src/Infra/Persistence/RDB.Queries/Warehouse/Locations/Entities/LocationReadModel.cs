namespace Insurance.InventoryService.Infra.Persistence.RDB.Queries.Warehouse.Locations.Entities;

using Insurance.InventoryService.AppCore.Domain.Warehouse.Entities;

public class LocationReadModel
{
    public long Id { get; set; }
    public Guid BusinessKey { get; set; }
    public Guid WarehouseRef { get; set; }
    public string LocationCode { get; set; } = string.Empty;
    public LocationType LocationType { get; set; }
    public string? Aisle { get; set; }
    public string? Rack { get; set; }
    public string? Shelf { get; set; }
    public string? Bin { get; set; }
    public bool IsActive { get; set; }
}
