namespace Insurance.InventoryService.AppCore.Shared.StockDetails.Commands.ArchiveEmptyStockDetail;

public class ArchiveEmptyStockDetailCommandResult
{
    public Guid StockDetailBusinessKey { get; set; }
    public bool Archived { get; set; }
}
