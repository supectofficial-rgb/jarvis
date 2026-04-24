namespace Insurance.InventoryService.AppCore.Shared.Warehouse.QualityStatuses.Queries.GetActiveQualityStatuses;

using Insurance.InventoryService.AppCore.Shared.Warehouse.QualityStatuses.Queries.Common;

public class GetActiveQualityStatusesQueryResult
{
    public List<QualityStatusListItem> Items { get; set; } = new();
}
