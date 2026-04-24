namespace Insurance.InventoryService.AppCore.Shared.SourceTracing.Queries.GetOpenByVariant;

using Insurance.InventoryService.AppCore.Shared.SourceTracing.Queries.Common;

public class GetOpenInventorySourceBalancesByVariantQueryResult
{
    public List<InventorySourceBalanceListItem> Items { get; set; } = new();
}
