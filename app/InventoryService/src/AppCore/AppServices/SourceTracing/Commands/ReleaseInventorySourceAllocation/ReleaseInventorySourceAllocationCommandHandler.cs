namespace Insurance.InventoryService.AppCore.AppServices.SourceTracing.Commands.ReleaseInventorySourceAllocation;

using Insurance.InventoryService.AppCore.Shared.SourceTracing.Commands;
using Insurance.InventoryService.AppCore.Shared.SourceTracing.Commands.ReleaseInventorySourceAllocation;
using OysterFx.AppCore.AppServices.Commands;
using OysterFx.AppCore.Shared.Commands.Common;

public class ReleaseInventorySourceAllocationCommandHandler : CommandHandler<ReleaseInventorySourceAllocationCommand, Guid>
{
    private readonly IInventorySourceBalanceCommandRepository _repository;

    public ReleaseInventorySourceAllocationCommandHandler(IInventorySourceBalanceCommandRepository repository)
    {
        _repository = repository;
    }

    public override async Task<CommandResult<Guid>> Handle(ReleaseInventorySourceAllocationCommand command)
    {
        if (command.SourceBalanceBusinessKey == Guid.Empty)
            return Fail("SourceBalanceBusinessKey is required.");

        var sourceBalance = await _repository.GetByBusinessKeyAsync(command.SourceBalanceBusinessKey);
        if (sourceBalance is null)
            return Fail("Inventory source balance was not found.");

        try
        {
            sourceBalance.ReleaseAllocation(command.AllocationBusinessKey, command.Quantity);
        }
        catch (Exception ex)
        {
            return Fail(ex.Message);
        }

        await _repository.CommitAsync();
        return Ok(command.AllocationBusinessKey);
    }
}
