namespace Insurance.InventoryService.AppCore.Shared.Catalog.UnitOfMeasures.Queries.GetActiveUnitOfMeasures;

using Insurance.InventoryService.AppCore.Shared.Catalog.UnitOfMeasures.Queries.Common;

public class GetActiveUnitOfMeasuresQueryResult
{
    public List<UnitOfMeasureListItem> Items { get; set; } = new();
}
