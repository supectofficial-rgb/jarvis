namespace Insurance.InventoryService.AppCore.Shared.StockDetails.Commands.ApplyIssueToStockDetail;

public class ApplyIssueToStockDetailCommandResult
{
    public Guid StockDetailBusinessKey { get; set; }
    public decimal QuantityOnHand { get; set; }
}
