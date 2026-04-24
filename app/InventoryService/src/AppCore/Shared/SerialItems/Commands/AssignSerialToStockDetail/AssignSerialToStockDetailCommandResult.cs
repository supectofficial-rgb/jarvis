namespace Insurance.InventoryService.AppCore.Shared.SerialItems.Commands.AssignSerialToStockDetail;

public class AssignSerialToStockDetailCommandResult
{
    public Guid SerialItemBusinessKey { get; set; }
    public Guid StockDetailBusinessKey { get; set; }
}
