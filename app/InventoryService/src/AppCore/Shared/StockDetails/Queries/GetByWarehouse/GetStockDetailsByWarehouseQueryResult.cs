namespace Insurance.InventoryService.AppCore.Shared.StockDetails.Queries.GetByWarehouse;

using Insurance.InventoryService.AppCore.Shared.StockDetails.Queries.Common;

public class GetStockDetailsByWarehouseQueryResult
{
    public List<StockDetailListItem> Items { get; set; } = new();
}
