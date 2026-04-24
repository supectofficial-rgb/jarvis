namespace Insurance.InventoryService.AppCore.Shared.Warehouse.QualityStatuses.Queries.SearchQualityStatuses;

using Insurance.InventoryService.AppCore.Shared.Warehouse.QualityStatuses.Queries.Common;

public class SearchQualityStatusesQueryResult
{
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public List<QualityStatusListItem> Items { get; set; } = new();
}
