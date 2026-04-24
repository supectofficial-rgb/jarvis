namespace Insurance.InventoryService.AppCore.Shared.Warehouse.Warehouses.Queries.Common;

public class WarehouseListItem
{
    public Guid WarehouseBusinessKey { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}

public class WarehouseLookupItem
{
    public Guid WarehouseBusinessKey { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
}

public class WarehouseSummaryItem
{
    public Guid WarehouseBusinessKey { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public int LocationCount { get; set; }
    public int ActiveLocationCount { get; set; }
}

public class WarehouseWithLocationsItem
{
    public Guid WarehouseBusinessKey { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public List<WarehouseLocationItem> Locations { get; set; } = new();
}

public class WarehouseLocationItem
{
    public Guid LocationBusinessKey { get; set; }
    public string LocationCode { get; set; } = string.Empty;
    public string LocationType { get; set; } = string.Empty;
    public string? Aisle { get; set; }
    public string? Rack { get; set; }
    public string? Shelf { get; set; }
    public string? Bin { get; set; }
    public bool IsActive { get; set; }
}
