namespace Insurance.InventoryService.AppCore.AppServices.StockDetails.Commands.ReconcileStockDetail;

using Insurance.InventoryService.AppCore.Shared.StockDetails.Commands;
using Insurance.InventoryService.AppCore.Shared.StockDetails.Commands.ReconcileStockDetail;
using OysterFx.AppCore.AppServices.Commands;
using OysterFx.AppCore.Shared.Commands.Common;

public class ReconcileStockDetailCommandHandler : CommandHandler<ReconcileStockDetailCommand, ReconcileStockDetailCommandResult>
{
    private readonly IStockDetailCommandRepository _repository;

    public ReconcileStockDetailCommandHandler(IStockDetailCommandRepository repository)
    {
        _repository = repository;
    }

    public override async Task<CommandResult<ReconcileStockDetailCommandResult>> Handle(ReconcileStockDetailCommand command)
    {
        if (command.StockDetailBusinessKey == Guid.Empty)
            return Fail("StockDetailBusinessKey is required.");

        if (command.ExpectedQuantityOnHand < 0)
            return Fail("ExpectedQuantityOnHand cannot be negative.");

        var stockDetail = await _repository.GetByBusinessKeyAsync(command.StockDetailBusinessKey);
        if (stockDetail is null)
            return Fail("Stock detail was not found.");

        var previous = stockDetail.QuantityOnHand;
        var delta = command.ExpectedQuantityOnHand - previous;
        if (delta != 0)
        {
            stockDetail.Adjust(delta, command.OccurredAt);
            await _repository.CommitAsync();
        }

        return Ok(new ReconcileStockDetailCommandResult
        {
            StockDetailBusinessKey = stockDetail.BusinessKey.Value,
            PreviousQuantityOnHand = previous,
            QuantityOnHand = stockDetail.QuantityOnHand,
            DeltaApplied = delta
        });
    }
}
