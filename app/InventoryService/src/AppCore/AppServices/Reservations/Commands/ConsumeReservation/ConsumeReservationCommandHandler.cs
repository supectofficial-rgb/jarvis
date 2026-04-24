namespace Insurance.InventoryService.AppCore.AppServices.Reservations.Commands.ConsumeReservation;

using Insurance.InventoryService.AppCore.Shared.Reservations.Commands;
using Insurance.InventoryService.AppCore.Shared.Reservations.Commands.ConsumeReservation;
using OysterFx.AppCore.AppServices.Commands;
using OysterFx.AppCore.Shared.Commands.Common;

public class ConsumeReservationCommandHandler : CommandHandler<ConsumeReservationCommand, Guid>
{
    private readonly IInventoryReservationCommandRepository _repository;

    public ConsumeReservationCommandHandler(IInventoryReservationCommandRepository repository)
    {
        _repository = repository;
    }

    public override async Task<CommandResult<Guid>> Handle(ConsumeReservationCommand command)
    {
        if (command.ReservationBusinessKey == Guid.Empty)
            return Fail("ReservationBusinessKey is required.");

        if (command.Quantity <= 0)
            return Fail("Quantity must be greater than zero.");

        var reservation = await _repository.GetByBusinessKeyAsync(command.ReservationBusinessKey);
        if (reservation is null)
            return Fail("Reservation was not found.");

        try
        {
            reservation.Consume(command.Quantity, command.ReasonCode);
        }
        catch (Exception ex)
        {
            return Fail(ex.Message);
        }

        await _repository.CommitAsync();
        return Ok(reservation.BusinessKey.Value);
    }
}
