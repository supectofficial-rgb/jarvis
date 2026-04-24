namespace Insurance.InventoryService.AppCore.Shared.SourceTracing.Queries.GetConsumptionsByTransactionLine;

using Insurance.InventoryService.AppCore.Shared.SourceTracing.Queries.Common;

public class GetInventorySourceConsumptionsByTransactionLineQueryResult
{
    public List<InventorySourceConsumptionListItem> Items { get; set; } = new();
}
