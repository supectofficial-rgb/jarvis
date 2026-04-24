namespace Insurance.InventoryService.AppCore.AppServices.Reservations.Commands.AllocateReservation;

using Insurance.InventoryService.AppCore.Shared.Reservations.Commands;
using Insurance.InventoryService.AppCore.Shared.Reservations.Commands.AllocateReservation;
using OysterFx.AppCore.AppServices.Commands;
using OysterFx.AppCore.Shared.Commands.Common;

public class AllocateReservationCommandHandler : CommandHandler<AllocateReservationCommand, Guid>
{
    private readonly IInventoryReservationCommandRepository _repository;

    public AllocateReservationCommandHandler(IInventoryReservationCommandRepository repository)
    {
        _repository = repository;
    }

    public override async Task<CommandResult<Guid>> Handle(AllocateReservationCommand command)
    {
        if (command.ReservationBusinessKey == Guid.Empty)
            return Fail("ReservationBusinessKey is required.");

        var reservation = await _repository.GetByBusinessKeyAsync(command.ReservationBusinessKey);
        if (reservation is null)
            return Fail("Reservation was not found.");

        try
        {
            var allocation = reservation.AddAllocation(
                command.StockDetailRef,
                command.WarehouseRef,
                command.LocationRef,
                command.QualityStatusRef,
                command.AllocatedQty,
                command.LotBatchNo,
                command.SerialRef);

            await _repository.CommitAsync();
            return Ok(allocation.BusinessKey.Value);
        }
        catch (Exception ex)
        {
            return Fail(ex.Message);
        }
    }
}
