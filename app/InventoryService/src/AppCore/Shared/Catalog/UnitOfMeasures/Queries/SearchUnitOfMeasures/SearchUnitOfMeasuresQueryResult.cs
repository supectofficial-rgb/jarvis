namespace Insurance.InventoryService.AppCore.Shared.Catalog.UnitOfMeasures.Queries.SearchUnitOfMeasures;

using Insurance.InventoryService.AppCore.Shared.Catalog.UnitOfMeasures.Queries.Common;

public class SearchUnitOfMeasuresQueryResult
{
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public List<UnitOfMeasureListItem> Items { get; set; } = new();
}
