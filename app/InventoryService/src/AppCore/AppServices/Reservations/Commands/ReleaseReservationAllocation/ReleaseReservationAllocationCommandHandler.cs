namespace Insurance.InventoryService.AppCore.AppServices.Reservations.Commands.ReleaseReservationAllocation;

using Insurance.InventoryService.AppCore.Shared.Reservations.Commands;
using Insurance.InventoryService.AppCore.Shared.Reservations.Commands.ReleaseReservationAllocation;
using OysterFx.AppCore.AppServices.Commands;
using OysterFx.AppCore.Shared.Commands.Common;

public class ReleaseReservationAllocationCommandHandler : CommandHandler<ReleaseReservationAllocationCommand, Guid>
{
    private readonly IInventoryReservationCommandRepository _repository;

    public ReleaseReservationAllocationCommandHandler(IInventoryReservationCommandRepository repository)
    {
        _repository = repository;
    }

    public override async Task<CommandResult<Guid>> Handle(ReleaseReservationAllocationCommand command)
    {
        if (command.ReservationBusinessKey == Guid.Empty)
            return Fail("ReservationBusinessKey is required.");

        var reservation = await _repository.GetByBusinessKeyAsync(command.ReservationBusinessKey);
        if (reservation is null)
            return Fail("Reservation was not found.");

        try
        {
            reservation.ReleaseAllocation(command.AllocationBusinessKey, command.Quantity);
        }
        catch (Exception ex)
        {
            return Fail(ex.Message);
        }

        await _repository.CommitAsync();
        return Ok(command.AllocationBusinessKey);
    }
}
