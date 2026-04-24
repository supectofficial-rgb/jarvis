namespace Insurance.InventoryService.AppCore.Shared.StockDetails.Commands.RebuildStockDetail;

public class RebuildStockDetailCommandResult
{
    public Guid StockDetailBusinessKey { get; set; }
    public decimal PreviousQuantityOnHand { get; set; }
    public decimal QuantityOnHand { get; set; }
}
