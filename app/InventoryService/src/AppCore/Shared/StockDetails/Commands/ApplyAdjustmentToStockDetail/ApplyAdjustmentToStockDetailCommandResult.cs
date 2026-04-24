namespace Insurance.InventoryService.AppCore.Shared.StockDetails.Commands.ApplyAdjustmentToStockDetail;

public class ApplyAdjustmentToStockDetailCommandResult
{
    public Guid StockDetailBusinessKey { get; set; }
    public decimal QuantityOnHand { get; set; }
}
