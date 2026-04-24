namespace Insurance.InventoryService.AppCore.Shared.StockDetails.Commands.ReconcileStockDetail;

using OysterFx.AppCore.Shared.Commands;

public class ReconcileStockDetailCommand : ICommand<ReconcileStockDetailCommandResult>
{
    public Guid StockDetailBusinessKey { get; set; }
    public decimal ExpectedQuantityOnHand { get; set; }
    public DateTime OccurredAt { get; set; } = DateTime.UtcNow;
}
