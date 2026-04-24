namespace Insurance.InventoryService.AppCore.Shared.StockDetails.Queries.GetVariantStockSummary;

using Insurance.InventoryService.AppCore.Shared.StockDetails.Queries.Common;

public class GetVariantStockSummaryQueryResult
{
    public VariantStockSummaryItem Item { get; set; } = new();
}
