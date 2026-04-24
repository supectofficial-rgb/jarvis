namespace Insurance.InventoryService.AppCore.Shared.SourceTracing.Queries.GetById;

using Insurance.InventoryService.AppCore.Shared.SourceTracing.Queries.Common;

public class GetInventorySourceBalanceByIdQueryResult
{
    public InventorySourceBalanceListItem Item { get; set; } = new();
}
