namespace Insurance.InventoryService.AppCore.AppServices.StockDetails.Commands.RebuildStockDetail;

using Insurance.InventoryService.AppCore.Shared.StockDetails.Commands;
using Insurance.InventoryService.AppCore.Shared.StockDetails.Commands.RebuildStockDetail;
using OysterFx.AppCore.AppServices.Commands;
using OysterFx.AppCore.Shared.Commands.Common;

public class RebuildStockDetailCommandHandler : CommandHandler<RebuildStockDetailCommand, RebuildStockDetailCommandResult>
{
    private readonly IStockDetailCommandRepository _repository;

    public RebuildStockDetailCommandHandler(IStockDetailCommandRepository repository)
    {
        _repository = repository;
    }

    public override async Task<CommandResult<RebuildStockDetailCommandResult>> Handle(RebuildStockDetailCommand command)
    {
        if (command.StockDetailBusinessKey == Guid.Empty)
            return Fail("StockDetailBusinessKey is required.");

        if (command.TargetQuantityOnHand < 0)
            return Fail("TargetQuantityOnHand cannot be negative.");

        var stockDetail = await _repository.GetByBusinessKeyAsync(command.StockDetailBusinessKey);
        if (stockDetail is null)
            return Fail("Stock detail was not found.");

        var previous = stockDetail.QuantityOnHand;
        var delta = command.TargetQuantityOnHand - previous;
        if (delta != 0)
        {
            stockDetail.Adjust(delta, command.OccurredAt);
            await _repository.CommitAsync();
        }

        return Ok(new RebuildStockDetailCommandResult
        {
            StockDetailBusinessKey = stockDetail.BusinessKey.Value,
            PreviousQuantityOnHand = previous,
            QuantityOnHand = stockDetail.QuantityOnHand
        });
    }
}
