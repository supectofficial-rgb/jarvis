namespace Insurance.InventoryService.AppCore.AppServices.Reservations.Commands.ConsumeReservationAllocation;

using Insurance.InventoryService.AppCore.Shared.Reservations.Commands;
using Insurance.InventoryService.AppCore.Shared.Reservations.Commands.ConsumeReservationAllocation;
using OysterFx.AppCore.AppServices.Commands;
using OysterFx.AppCore.Shared.Commands.Common;

public class ConsumeReservationAllocationCommandHandler : CommandHandler<ConsumeReservationAllocationCommand, Guid>
{
    private readonly IInventoryReservationCommandRepository _repository;

    public ConsumeReservationAllocationCommandHandler(IInventoryReservationCommandRepository repository)
    {
        _repository = repository;
    }

    public override async Task<CommandResult<Guid>> Handle(ConsumeReservationAllocationCommand command)
    {
        if (command.ReservationBusinessKey == Guid.Empty)
            return Fail("ReservationBusinessKey is required.");

        var reservation = await _repository.GetByBusinessKeyAsync(command.ReservationBusinessKey);
        if (reservation is null)
            return Fail("Reservation was not found.");

        try
        {
            reservation.ConsumeAllocation(command.AllocationBusinessKey, command.Quantity, command.ReasonCode);
        }
        catch (Exception ex)
        {
            return Fail(ex.Message);
        }

        await _repository.CommitAsync();
        return Ok(command.AllocationBusinessKey);
    }
}
