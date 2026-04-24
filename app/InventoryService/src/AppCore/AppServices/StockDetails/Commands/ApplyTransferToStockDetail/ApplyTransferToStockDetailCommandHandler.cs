namespace Insurance.InventoryService.AppCore.AppServices.StockDetails.Commands.ApplyTransferToStockDetail;

using Insurance.InventoryService.AppCore.Shared.StockDetails.Commands;
using Insurance.InventoryService.AppCore.Shared.StockDetails.Commands.ApplyTransferToStockDetail;
using OysterFx.AppCore.AppServices.Commands;
using OysterFx.AppCore.Shared.Commands.Common;

public class ApplyTransferToStockDetailCommandHandler : CommandHandler<ApplyTransferToStockDetailCommand, ApplyTransferToStockDetailCommandResult>
{
    private readonly IStockDetailCommandRepository _repository;

    public ApplyTransferToStockDetailCommandHandler(IStockDetailCommandRepository repository)
    {
        _repository = repository;
    }

    public override async Task<CommandResult<ApplyTransferToStockDetailCommandResult>> Handle(ApplyTransferToStockDetailCommand command)
    {
        if (command.StockDetailBusinessKey == Guid.Empty)
            return Fail("StockDetailBusinessKey is required.");

        if (command.Quantity <= 0)
            return Fail("Quantity must be greater than zero.");

        var direction = command.Direction.Trim();
        var isInbound = direction.Equals("In", StringComparison.OrdinalIgnoreCase)
            || direction.Equals("Inbound", StringComparison.OrdinalIgnoreCase);
        var isOutbound = direction.Equals("Out", StringComparison.OrdinalIgnoreCase)
            || direction.Equals("Outbound", StringComparison.OrdinalIgnoreCase);

        if (!isInbound && !isOutbound)
            return Fail("Direction must be In or Out.");

        var stockDetail = await _repository.GetByBusinessKeyAsync(command.StockDetailBusinessKey);
        if (stockDetail is null)
            return Fail("Stock detail was not found.");

        var delta = isInbound ? command.Quantity : -command.Quantity;

        try
        {
            stockDetail.ApplyQuantity(delta, command.OccurredAt);
        }
        catch (Exception ex)
        {
            return Fail(ex.Message);
        }

        await _repository.CommitAsync();

        return Ok(new ApplyTransferToStockDetailCommandResult
        {
            StockDetailBusinessKey = stockDetail.BusinessKey.Value,
            QuantityOnHand = stockDetail.QuantityOnHand
        });
    }
}
