namespace Insurance.InventoryService.AppCore.Shared.SerialItems.Commands.AssignSerialToStockDetail;

using OysterFx.AppCore.Shared.Commands;

public class AssignSerialToStockDetailCommand : ICommand<AssignSerialToStockDetailCommandResult>
{
    public Guid SerialItemBusinessKey { get; set; }
    public Guid StockDetailBusinessKey { get; set; }
}
