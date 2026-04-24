namespace Insurance.InventoryService.AppCore.Shared.StockDetails.Commands.ApplyTransferToStockDetail;

using OysterFx.AppCore.Shared.Commands;

public class ApplyTransferToStockDetailCommand : ICommand<ApplyTransferToStockDetailCommandResult>
{
    public Guid StockDetailBusinessKey { get; set; }
    public decimal Quantity { get; set; }
    public DateTime OccurredAt { get; set; } = DateTime.UtcNow;
    public string Direction { get; set; } = "Out";
}
