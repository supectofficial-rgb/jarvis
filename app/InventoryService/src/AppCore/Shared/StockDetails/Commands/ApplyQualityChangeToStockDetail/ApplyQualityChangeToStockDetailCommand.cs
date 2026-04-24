namespace Insurance.InventoryService.AppCore.Shared.StockDetails.Commands.ApplyQualityChangeToStockDetail;

using OysterFx.AppCore.Shared.Commands;

public class ApplyQualityChangeToStockDetailCommand : ICommand<ApplyQualityChangeToStockDetailCommandResult>
{
    public Guid StockDetailBusinessKey { get; set; }
    public decimal Quantity { get; set; }
    public DateTime OccurredAt { get; set; } = DateTime.UtcNow;
    public string Direction { get; set; } = "Out";
}
