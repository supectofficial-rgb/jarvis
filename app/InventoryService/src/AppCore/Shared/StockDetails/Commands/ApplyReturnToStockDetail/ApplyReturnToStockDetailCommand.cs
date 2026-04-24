namespace Insurance.InventoryService.AppCore.Shared.StockDetails.Commands.ApplyReturnToStockDetail;

using OysterFx.AppCore.Shared.Commands;

public class ApplyReturnToStockDetailCommand : ICommand<ApplyReturnToStockDetailCommandResult>
{
    public Guid StockDetailBusinessKey { get; set; }
    public decimal Quantity { get; set; }
    public DateTime OccurredAt { get; set; } = DateTime.UtcNow;
}
