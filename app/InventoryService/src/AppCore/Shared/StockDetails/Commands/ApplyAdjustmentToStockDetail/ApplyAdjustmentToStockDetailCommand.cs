namespace Insurance.InventoryService.AppCore.Shared.StockDetails.Commands.ApplyAdjustmentToStockDetail;

using OysterFx.AppCore.Shared.Commands;

public class ApplyAdjustmentToStockDetailCommand : ICommand<ApplyAdjustmentToStockDetailCommandResult>
{
    public Guid StockDetailBusinessKey { get; set; }
    public decimal DeltaQuantity { get; set; }
    public DateTime OccurredAt { get; set; } = DateTime.UtcNow;
}
