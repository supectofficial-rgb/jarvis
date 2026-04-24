namespace Insurance.InventoryService.AppCore.AppServices.StockDetails.Commands.ApplyAdjustmentToStockDetail;

using Insurance.InventoryService.AppCore.Shared.StockDetails.Commands;
using Insurance.InventoryService.AppCore.Shared.StockDetails.Commands.ApplyAdjustmentToStockDetail;
using OysterFx.AppCore.AppServices.Commands;
using OysterFx.AppCore.Shared.Commands.Common;

public class ApplyAdjustmentToStockDetailCommandHandler : CommandHandler<ApplyAdjustmentToStockDetailCommand, ApplyAdjustmentToStockDetailCommandResult>
{
    private readonly IStockDetailCommandRepository _repository;

    public ApplyAdjustmentToStockDetailCommandHandler(IStockDetailCommandRepository repository)
    {
        _repository = repository;
    }

    public override async Task<CommandResult<ApplyAdjustmentToStockDetailCommandResult>> Handle(ApplyAdjustmentToStockDetailCommand command)
    {
        if (command.StockDetailBusinessKey == Guid.Empty)
            return Fail("StockDetailBusinessKey is required.");

        if (command.DeltaQuantity == 0)
            return Fail("DeltaQuantity cannot be zero.");

        var stockDetail = await _repository.GetByBusinessKeyAsync(command.StockDetailBusinessKey);
        if (stockDetail is null)
            return Fail("Stock detail was not found.");

        try
        {
            stockDetail.ApplyQuantity(command.DeltaQuantity, command.OccurredAt);
        }
        catch (Exception ex)
        {
            return Fail(ex.Message);
        }

        await _repository.CommitAsync();

        return Ok(new ApplyAdjustmentToStockDetailCommandResult
        {
            StockDetailBusinessKey = stockDetail.BusinessKey.Value,
            QuantityOnHand = stockDetail.QuantityOnHand
        });
    }
}
