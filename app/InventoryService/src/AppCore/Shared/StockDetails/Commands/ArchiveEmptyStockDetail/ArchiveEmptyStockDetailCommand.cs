namespace Insurance.InventoryService.AppCore.Shared.StockDetails.Commands.ArchiveEmptyStockDetail;

using OysterFx.AppCore.Shared.Commands;

public class ArchiveEmptyStockDetailCommand : ICommand<ArchiveEmptyStockDetailCommandResult>
{
    public Guid StockDetailBusinessKey { get; set; }
}
