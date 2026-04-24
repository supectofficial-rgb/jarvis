namespace Insurance.InventoryService.AppCore.AppServices.SourceTracing.Commands.AllocateInventorySourceBalance;

using Insurance.InventoryService.AppCore.Shared.SourceTracing.Commands;
using Insurance.InventoryService.AppCore.Shared.SourceTracing.Commands.AllocateInventorySourceBalance;
using OysterFx.AppCore.AppServices.Commands;
using OysterFx.AppCore.Shared.Commands.Common;

public class AllocateInventorySourceBalanceCommandHandler : CommandHandler<AllocateInventorySourceBalanceCommand, Guid>
{
    private readonly IInventorySourceBalanceCommandRepository _repository;

    public AllocateInventorySourceBalanceCommandHandler(IInventorySourceBalanceCommandRepository repository)
    {
        _repository = repository;
    }

    public override async Task<CommandResult<Guid>> Handle(AllocateInventorySourceBalanceCommand command)
    {
        if (command.SourceBalanceBusinessKey == Guid.Empty)
            return Fail("SourceBalanceBusinessKey is required.");

        var sourceBalance = await _repository.GetByBusinessKeyAsync(command.SourceBalanceBusinessKey);
        if (sourceBalance is null)
            return Fail("Inventory source balance was not found.");

        try
        {
            var allocation = sourceBalance.Allocate(command.ReservationRef, command.ReservationAllocationRef, command.Quantity);
            await _repository.CommitAsync();
            return Ok(allocation.BusinessKey.Value);
        }
        catch (Exception ex)
        {
            return Fail(ex.Message);
        }
    }
}
