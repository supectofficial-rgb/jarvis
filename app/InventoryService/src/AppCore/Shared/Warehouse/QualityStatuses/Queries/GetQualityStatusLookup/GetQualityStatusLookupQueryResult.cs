namespace Insurance.InventoryService.AppCore.Shared.Warehouse.QualityStatuses.Queries.GetQualityStatusLookup;

using Insurance.InventoryService.AppCore.Shared.Warehouse.QualityStatuses.Queries.Common;

public class GetQualityStatusLookupQueryResult
{
    public List<QualityStatusLookupItem> Items { get; set; } = new();
}
