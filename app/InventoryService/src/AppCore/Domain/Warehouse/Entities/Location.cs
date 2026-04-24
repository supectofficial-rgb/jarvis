namespace Insurance.InventoryService.AppCore.Domain.Warehouse.Entities;

using OysterFx.AppCore.Domain.Aggregates;

public sealed class Location : AggregateRoot
{
    public Guid WarehouseRef { get; private set; }
    public string? Aisle { get; private set; }
    public string? Rack { get; private set; }
    public string? Shelf { get; private set; }
    public string? Bin { get; private set; }
    public string LocationCode { get; private set; } = string.Empty;
    public LocationType LocationType { get; private set; }
    public bool IsActive { get; private set; }

    private Location()
    {
    }

    private Location(
        Guid warehouseRef,
        string locationCode,
        LocationType locationType,
        string? aisle,
        string? rack,
        string? shelf,
        string? bin)
    {
        WarehouseRef = warehouseRef;
        LocationCode = NormalizeRequired(locationCode, nameof(locationCode));
        LocationType = locationType;
        Aisle = NormalizeOptional(aisle);
        Rack = NormalizeOptional(rack);
        Shelf = NormalizeOptional(shelf);
        Bin = NormalizeOptional(bin);
        IsActive = true;
    }

    public static Location Create(
        Guid warehouseRef,
        string locationCode,
        LocationType locationType,
        string? aisle = null,
        string? rack = null,
        string? shelf = null,
        string? bin = null)
    {
        return new Location(warehouseRef, locationCode, locationType, aisle, rack, shelf, bin);
    }

    public void ChangeWarehouse(Guid warehouseRef) => WarehouseRef = warehouseRef;

    public void ChangeCode(string locationCode) => LocationCode = NormalizeRequired(locationCode, nameof(locationCode));

    public void ChangeType(LocationType locationType) => LocationType = locationType;

    public void UpdateCoordinates(string? aisle, string? rack, string? shelf, string? bin)
    {
        Aisle = NormalizeOptional(aisle);
        Rack = NormalizeOptional(rack);
        Shelf = NormalizeOptional(shelf);
        Bin = NormalizeOptional(bin);
    }

    public void Activate() => IsActive = true;

    public void Deactivate() => IsActive = false;

    private static string NormalizeRequired(string value, string paramName)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Value is required.", paramName);

        return value.Trim();
    }

    private static string? NormalizeOptional(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
