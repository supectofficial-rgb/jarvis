namespace Insurance.InventoryService.AppCore.AppServices.Reservations.Commands.RejectReservation;

using Insurance.InventoryService.AppCore.Shared.Reservations.Commands;
using Insurance.InventoryService.AppCore.Shared.Reservations.Commands.RejectReservation;
using OysterFx.AppCore.AppServices.Commands;
using OysterFx.AppCore.Shared.Commands.Common;

public class RejectReservationCommandHandler : CommandHandler<RejectReservationCommand, Guid>
{
    private readonly IInventoryReservationCommandRepository _repository;

    public RejectReservationCommandHandler(IInventoryReservationCommandRepository repository)
    {
        _repository = repository;
    }

    public override async Task<CommandResult<Guid>> Handle(RejectReservationCommand command)
    {
        if (command.ReservationBusinessKey == Guid.Empty)
            return Fail("ReservationBusinessKey is required.");

        var reservation = await _repository.GetByBusinessKeyAsync(command.ReservationBusinessKey);
        if (reservation is null)
            return Fail("Reservation was not found.");

        try
        {
            reservation.Reject(command.ReasonCode);
        }
        catch (Exception ex)
        {
            return Fail(ex.Message);
        }

        await _repository.CommitAsync();
        return Ok(reservation.BusinessKey.Value);
    }
}
