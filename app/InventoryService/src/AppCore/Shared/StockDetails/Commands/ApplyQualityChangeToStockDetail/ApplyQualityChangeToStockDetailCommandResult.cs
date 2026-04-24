namespace Insurance.InventoryService.AppCore.Shared.StockDetails.Commands.ApplyQualityChangeToStockDetail;

public class ApplyQualityChangeToStockDetailCommandResult
{
    public Guid StockDetailBusinessKey { get; set; }
    public decimal QuantityOnHand { get; set; }
}
