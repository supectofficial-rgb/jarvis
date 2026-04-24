namespace Insurance.InventoryService.AppCore.Shared.StockDetails.Commands.ApplyTransferToStockDetail;

public class ApplyTransferToStockDetailCommandResult
{
    public Guid StockDetailBusinessKey { get; set; }
    public decimal QuantityOnHand { get; set; }
}
