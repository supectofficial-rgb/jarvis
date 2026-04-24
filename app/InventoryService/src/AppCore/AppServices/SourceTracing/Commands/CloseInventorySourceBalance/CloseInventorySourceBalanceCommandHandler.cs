namespace Insurance.InventoryService.AppCore.AppServices.SourceTracing.Commands.CloseInventorySourceBalance;

using Insurance.InventoryService.AppCore.Shared.SourceTracing.Commands;
using Insurance.InventoryService.AppCore.Shared.SourceTracing.Commands.CloseInventorySourceBalance;
using OysterFx.AppCore.AppServices.Commands;
using OysterFx.AppCore.Shared.Commands.Common;

public class CloseInventorySourceBalanceCommandHandler : CommandHandler<CloseInventorySourceBalanceCommand, Guid>
{
    private readonly IInventorySourceBalanceCommandRepository _repository;

    public CloseInventorySourceBalanceCommandHandler(IInventorySourceBalanceCommandRepository repository)
    {
        _repository = repository;
    }

    public override async Task<CommandResult<Guid>> Handle(CloseInventorySourceBalanceCommand command)
    {
        if (command.SourceBalanceBusinessKey == Guid.Empty)
            return Fail("SourceBalanceBusinessKey is required.");

        var sourceBalance = await _repository.GetByBusinessKeyAsync(command.SourceBalanceBusinessKey);
        if (sourceBalance is null)
            return Fail("Inventory source balance was not found.");

        try
        {
            if (sourceBalance.RemainingQty > 0)
            {
                if (sourceBalance.ConsumedQty > 0)
                    return Fail("Source balance with remaining quantity cannot be force-closed after partial consumption.");

                sourceBalance.Cancel();
            }
        }
        catch (Exception ex)
        {
            return Fail(ex.Message);
        }

        await _repository.CommitAsync();
        return Ok(sourceBalance.BusinessKey.Value);
    }
}
