namespace Insurance.InventoryService.AppCore.Shared.StockDetails.Commands.ApplyReceiptToStockDetail;

using OysterFx.AppCore.Shared.Commands;

public class ApplyReceiptToStockDetailCommand : ICommand<ApplyReceiptToStockDetailCommandResult>
{
    public Guid StockDetailBusinessKey { get; set; }
    public decimal Quantity { get; set; }
    public DateTime OccurredAt { get; set; } = DateTime.UtcNow;
}
