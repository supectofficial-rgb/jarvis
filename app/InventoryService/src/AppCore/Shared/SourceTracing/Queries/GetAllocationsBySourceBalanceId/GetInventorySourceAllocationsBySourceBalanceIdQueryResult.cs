namespace Insurance.InventoryService.AppCore.Shared.SourceTracing.Queries.GetAllocationsBySourceBalanceId;

using Insurance.InventoryService.AppCore.Shared.SourceTracing.Queries.Common;

public class GetInventorySourceAllocationsBySourceBalanceIdQueryResult
{
    public List<InventorySourceAllocationListItem> Items { get; set; } = new();
}
