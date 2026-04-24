namespace Insurance.InventoryService.AppCore.AppServices.StockDetails.Commands.AdjustStockDetail;

using Insurance.InventoryService.AppCore.Shared.StockDetails.Commands;
using Insurance.InventoryService.AppCore.Shared.StockDetails.Commands.AdjustStockDetail;
using OysterFx.AppCore.AppServices.Commands;
using OysterFx.AppCore.Shared.Commands.Common;

public class AdjustStockDetailCommandHandler
    : CommandHandler<AdjustStockDetailCommand, AdjustStockDetailCommandResult>
{
    private readonly IStockDetailCommandRepository _repository;

    public AdjustStockDetailCommandHandler(IStockDetailCommandRepository repository)
    {
        _repository = repository;
    }

    public override async Task<CommandResult<AdjustStockDetailCommandResult>> Handle(AdjustStockDetailCommand command)
    {
        _ = command;
        _ = _repository;

        return Fail("Direct stock adjustment command is disabled. Use InventoryDocument posting flow.");
    }
}
