namespace Insurance.InventoryService.AppCore.Shared.Catalog.UnitOfMeasures.Queries.GetUnitOfMeasureLookup;

using Insurance.InventoryService.AppCore.Shared.Catalog.UnitOfMeasures.Queries.Common;

public class GetUnitOfMeasureLookupQueryResult
{
    public List<UnitOfMeasureLookupItem> Items { get; set; } = new();
}
