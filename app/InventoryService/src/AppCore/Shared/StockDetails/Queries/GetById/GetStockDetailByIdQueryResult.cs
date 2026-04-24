namespace Insurance.InventoryService.AppCore.Shared.StockDetails.Queries.GetById;

using Insurance.InventoryService.AppCore.Shared.StockDetails.Queries.Common;

public class GetStockDetailByIdQueryResult
{
    public StockDetailListItem Item { get; set; } = new();
}
