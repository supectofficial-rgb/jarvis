namespace Insurance.InventoryService.AppCore.Shared.StockDetails.Commands.ApplyReturnToStockDetail;

public class ApplyReturnToStockDetailCommandResult
{
    public Guid StockDetailBusinessKey { get; set; }
    public decimal QuantityOnHand { get; set; }
}
