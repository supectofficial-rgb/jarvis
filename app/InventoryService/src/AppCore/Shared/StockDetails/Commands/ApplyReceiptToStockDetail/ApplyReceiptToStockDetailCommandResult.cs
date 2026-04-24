namespace Insurance.InventoryService.AppCore.Shared.StockDetails.Commands.ApplyReceiptToStockDetail;

public class ApplyReceiptToStockDetailCommandResult
{
    public Guid StockDetailBusinessKey { get; set; }
    public decimal QuantityOnHand { get; set; }
}
