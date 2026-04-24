namespace Insurance.InventoryService.AppCore.Shared.StockDetails.Commands.AdjustStockDetail;

using OysterFx.AppCore.Shared.Commands;

public class AdjustStockDetailCommand : ICommand<AdjustStockDetailCommandResult>
{
    public Guid StockDetailBusinessKey { get; set; }
    public decimal DeltaQuantity { get; set; }
    public DateTime OccurredAt { get; set; } = DateTime.UtcNow;
}
