namespace Insurance.InventoryService.AppCore.AppServices.SerialItems.Commands.AssignSerialToStockDetail;

using Insurance.InventoryService.AppCore.Shared.SerialItems.Commands;
using Insurance.InventoryService.AppCore.Shared.SerialItems.Commands.AssignSerialToStockDetail;
using Insurance.InventoryService.AppCore.Shared.StockDetails.Commands;
using OysterFx.AppCore.AppServices.Commands;
using OysterFx.AppCore.Domain.ValueObjects;
using OysterFx.AppCore.Shared.Commands.Common;

public class AssignSerialItemToStockDetailCommandHandler : CommandHandler<AssignSerialToStockDetailCommand, AssignSerialToStockDetailCommandResult>
{
    private readonly ISerialItemCommandRepository _serialRepository;
    private readonly IStockDetailCommandRepository _stockDetailRepository;

    public AssignSerialItemToStockDetailCommandHandler(
        ISerialItemCommandRepository serialRepository,
        IStockDetailCommandRepository stockDetailRepository)
    {
        _serialRepository = serialRepository;
        _stockDetailRepository = stockDetailRepository;
    }

    public override async Task<CommandResult<AssignSerialToStockDetailCommandResult>> Handle(AssignSerialToStockDetailCommand command)
    {
        if (command.SerialItemBusinessKey == Guid.Empty || command.StockDetailBusinessKey == Guid.Empty)
            return Fail("SerialItemBusinessKey and StockDetailBusinessKey are required.");

        var serialItem = await _serialRepository.GetByBusinessKeyAsync(command.SerialItemBusinessKey);
        if (serialItem is null)
            return Fail("Serial item was not found.");

        var stockDetail = await _stockDetailRepository.GetByBusinessKeyAsync(command.StockDetailBusinessKey);
        if (stockDetail is null)
            return Fail("Stock detail was not found.");

        serialItem.LinkStockDetail(BusinessKey.FromGuid(command.StockDetailBusinessKey));
        await _serialRepository.CommitAsync();

        return Ok(new AssignSerialToStockDetailCommandResult
        {
            SerialItemBusinessKey = serialItem.BusinessKey.Value,
            StockDetailBusinessKey = command.StockDetailBusinessKey
        });
    }
}



