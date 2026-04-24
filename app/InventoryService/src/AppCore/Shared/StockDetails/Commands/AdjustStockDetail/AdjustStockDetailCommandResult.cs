namespace Insurance.InventoryService.AppCore.Shared.StockDetails.Commands.AdjustStockDetail;

public class AdjustStockDetailCommandResult
{
    public Guid StockDetailBusinessKey { get; set; }
    public decimal QuantityOnHand { get; set; }
}
