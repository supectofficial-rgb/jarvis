namespace Insurance.InventoryService.AppCore.Shared.StockDetails.Commands.ApplyIssueToStockDetail;

using OysterFx.AppCore.Shared.Commands;

public class ApplyIssueToStockDetailCommand : ICommand<ApplyIssueToStockDetailCommandResult>
{
    public Guid StockDetailBusinessKey { get; set; }
    public decimal Quantity { get; set; }
    public DateTime OccurredAt { get; set; } = DateTime.UtcNow;
}
