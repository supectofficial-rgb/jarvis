namespace Insurance.InventoryService.AppCore.Shared.StockDetails.Commands.RebuildStockDetail;

using OysterFx.AppCore.Shared.Commands;

public class RebuildStockDetailCommand : ICommand<RebuildStockDetailCommandResult>
{
    public Guid StockDetailBusinessKey { get; set; }
    public decimal TargetQuantityOnHand { get; set; }
    public DateTime OccurredAt { get; set; } = DateTime.UtcNow;
}
