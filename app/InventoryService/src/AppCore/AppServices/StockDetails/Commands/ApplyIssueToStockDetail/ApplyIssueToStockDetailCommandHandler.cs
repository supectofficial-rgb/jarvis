namespace Insurance.InventoryService.AppCore.AppServices.StockDetails.Commands.ApplyIssueToStockDetail;

using Insurance.InventoryService.AppCore.Shared.StockDetails.Commands;
using Insurance.InventoryService.AppCore.Shared.StockDetails.Commands.ApplyIssueToStockDetail;
using OysterFx.AppCore.AppServices.Commands;
using OysterFx.AppCore.Shared.Commands.Common;

public class ApplyIssueToStockDetailCommandHandler : CommandHandler<ApplyIssueToStockDetailCommand, ApplyIssueToStockDetailCommandResult>
{
    private readonly IStockDetailCommandRepository _repository;

    public ApplyIssueToStockDetailCommandHandler(IStockDetailCommandRepository repository)
    {
        _repository = repository;
    }

    public override async Task<CommandResult<ApplyIssueToStockDetailCommandResult>> Handle(ApplyIssueToStockDetailCommand command)
    {
        if (command.StockDetailBusinessKey == Guid.Empty)
            return Fail("StockDetailBusinessKey is required.");

        if (command.Quantity <= 0)
            return Fail("Quantity must be greater than zero.");

        var stockDetail = await _repository.GetByBusinessKeyAsync(command.StockDetailBusinessKey);
        if (stockDetail is null)
            return Fail("Stock detail was not found.");

        try
        {
            stockDetail.ApplyQuantity(-command.Quantity, command.OccurredAt);
        }
        catch (Exception ex)
        {
            return Fail(ex.Message);
        }

        await _repository.CommitAsync();

        return Ok(new ApplyIssueToStockDetailCommandResult
        {
            StockDetailBusinessKey = stockDetail.BusinessKey.Value,
            QuantityOnHand = stockDetail.QuantityOnHand
        });
    }
}
