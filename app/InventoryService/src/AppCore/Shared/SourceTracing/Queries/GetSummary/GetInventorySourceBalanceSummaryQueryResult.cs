namespace Insurance.InventoryService.AppCore.Shared.SourceTracing.Queries.GetSummary;

using Insurance.InventoryService.AppCore.Shared.SourceTracing.Queries.Common;

public class GetInventorySourceBalanceSummaryQueryResult
{
    public InventorySourceBalanceSummaryItem Item { get; set; } = new();
}
