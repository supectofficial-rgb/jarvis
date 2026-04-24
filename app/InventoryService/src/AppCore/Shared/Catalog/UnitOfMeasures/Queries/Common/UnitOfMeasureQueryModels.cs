namespace Insurance.InventoryService.AppCore.Shared.Catalog.UnitOfMeasures.Queries.Common;

public class UnitOfMeasureListItem
{
    public Guid UnitOfMeasureBusinessKey { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public int Precision { get; set; }
    public bool IsActive { get; set; }
}

public class UnitOfMeasureLookupItem
{
    public Guid UnitOfMeasureBusinessKey { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
}
