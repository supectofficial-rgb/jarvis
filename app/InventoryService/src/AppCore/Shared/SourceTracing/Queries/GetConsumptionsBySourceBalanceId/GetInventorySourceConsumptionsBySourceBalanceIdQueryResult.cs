namespace Insurance.InventoryService.AppCore.Shared.SourceTracing.Queries.GetConsumptionsBySourceBalanceId;

using Insurance.InventoryService.AppCore.Shared.SourceTracing.Queries.Common;

public class GetInventorySourceConsumptionsBySourceBalanceIdQueryResult
{
    public List<InventorySourceConsumptionListItem> Items { get; set; } = new();
}
