namespace Insurance.InventoryService.AppCore.Shared.StockDetails.Commands.ReconcileStockDetail;

public class ReconcileStockDetailCommandResult
{
    public Guid StockDetailBusinessKey { get; set; }
    public decimal PreviousQuantityOnHand { get; set; }
    public decimal QuantityOnHand { get; set; }
    public decimal DeltaApplied { get; set; }
}
