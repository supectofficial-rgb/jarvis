namespace Insurance.InventoryService.AppCore.AppServices.Reservations.Commands.ConfirmReservation;

using Insurance.InventoryService.AppCore.Shared.Reservations.Commands;
using Insurance.InventoryService.AppCore.Shared.Reservations.Commands.ConfirmReservation;
using OysterFx.AppCore.AppServices.Commands;
using OysterFx.AppCore.Shared.Commands.Common;

public class ConfirmReservationCommandHandler : CommandHandler<ConfirmReservationCommand, Guid>
{
    private readonly IInventoryReservationCommandRepository _repository;

    public ConfirmReservationCommandHandler(IInventoryReservationCommandRepository repository)
    {
        _repository = repository;
    }

    public override async Task<CommandResult<Guid>> Handle(ConfirmReservationCommand command)
    {
        if (command.ReservationBusinessKey == Guid.Empty)
            return Fail("ReservationBusinessKey is required.");

        var reservation = await _repository.GetByBusinessKeyAsync(command.ReservationBusinessKey);
        if (reservation is null)
            return Fail("Reservation was not found.");

        try
        {
            if (command.AllocatedQuantity.HasValue)
                reservation.Confirm(command.AllocatedQuantity.Value);
            else
                reservation.Confirm();
        }
        catch (Exception ex)
        {
            return Fail(ex.Message);
        }

        await _repository.CommitAsync();
        return Ok(reservation.BusinessKey.Value);
    }
}
