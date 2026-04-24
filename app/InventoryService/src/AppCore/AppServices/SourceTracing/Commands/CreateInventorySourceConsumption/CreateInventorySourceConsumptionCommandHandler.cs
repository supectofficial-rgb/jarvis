namespace Insurance.InventoryService.AppCore.AppServices.SourceTracing.Commands.CreateInventorySourceConsumption;

using Insurance.InventoryService.AppCore.Shared.SourceTracing.Commands;
using Insurance.InventoryService.AppCore.Shared.SourceTracing.Commands.CreateInventorySourceConsumption;
using OysterFx.AppCore.AppServices.Commands;
using OysterFx.AppCore.Shared.Commands.Common;

public class CreateInventorySourceConsumptionCommandHandler : CommandHandler<CreateInventorySourceConsumptionCommand, Guid>
{
    private readonly IInventorySourceBalanceCommandRepository _repository;

    public CreateInventorySourceConsumptionCommandHandler(IInventorySourceBalanceCommandRepository repository)
    {
        _repository = repository;
    }

    public override async Task<CommandResult<Guid>> Handle(CreateInventorySourceConsumptionCommand command)
    {
        if (command.SourceBalanceBusinessKey == Guid.Empty)
            return Fail("SourceBalanceBusinessKey is required.");

        var sourceBalance = await _repository.GetByBusinessKeyAsync(command.SourceBalanceBusinessKey);
        if (sourceBalance is null)
            return Fail("Inventory source balance was not found.");

        try
        {
            if (command.AllocationBusinessKey.HasValue)
                sourceBalance.ConsumeAllocated(
                    command.AllocationBusinessKey.Value,
                    command.Quantity,
                    command.OutboundTransactionRef,
                    command.OutboundTransactionLineRef,
                    command.ReasonCode);
            else
                sourceBalance.Consume(
                    command.Quantity,
                    command.OutboundTransactionRef,
                    command.OutboundTransactionLineRef,
                    command.ReasonCode);
        }
        catch (Exception ex)
        {
            return Fail(ex.Message);
        }

        await _repository.CommitAsync();
        return Ok(sourceBalance.BusinessKey.Value);
    }
}
