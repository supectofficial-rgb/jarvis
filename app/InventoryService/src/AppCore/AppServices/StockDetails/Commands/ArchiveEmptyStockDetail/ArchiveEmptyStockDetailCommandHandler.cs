namespace Insurance.InventoryService.AppCore.AppServices.StockDetails.Commands.ArchiveEmptyStockDetail;

using Insurance.InventoryService.AppCore.Shared.StockDetails.Commands;
using Insurance.InventoryService.AppCore.Shared.StockDetails.Commands.ArchiveEmptyStockDetail;
using OysterFx.AppCore.AppServices.Commands;
using OysterFx.AppCore.Shared.Commands.Common;

public class ArchiveEmptyStockDetailCommandHandler : CommandHandler<ArchiveEmptyStockDetailCommand, ArchiveEmptyStockDetailCommandResult>
{
    private readonly IStockDetailCommandRepository _repository;

    public ArchiveEmptyStockDetailCommandHandler(IStockDetailCommandRepository repository)
    {
        _repository = repository;
    }

    public override async Task<CommandResult<ArchiveEmptyStockDetailCommandResult>> Handle(ArchiveEmptyStockDetailCommand command)
    {
        if (command.StockDetailBusinessKey == Guid.Empty)
            return Fail("StockDetailBusinessKey is required.");

        var stockDetail = await _repository.GetByBusinessKeyAsync(command.StockDetailBusinessKey);
        if (stockDetail is null)
            return Fail("Stock detail was not found.");

        if (stockDetail.QuantityOnHand != 0)
            return Fail("Only empty stock detail bucket can be archived.");

        _repository.Delete(stockDetail);
        await _repository.CommitAsync();

        return Ok(new ArchiveEmptyStockDetailCommandResult
        {
            StockDetailBusinessKey = command.StockDetailBusinessKey,
            Archived = true
        });
    }
}
