namespace Insurance.InventoryService.AppCore.AppServices.Reservations.Commands.CreateReservation;

using Insurance.InventoryService.AppCore.Domain.Reservations.Entities;
using Insurance.InventoryService.AppCore.Shared.Reservations.Commands;
using Insurance.InventoryService.AppCore.Shared.Reservations.Commands.CreateReservation;
using OysterFx.AppCore.AppServices.Commands;
using OysterFx.AppCore.Shared.Commands.Common;

public class CreateReservationCommandHandler : CommandHandler<CreateReservationCommand, Guid>
{
    private readonly IInventoryReservationCommandRepository _repository;

    public CreateReservationCommandHandler(IInventoryReservationCommandRepository repository)
    {
        _repository = repository;
    }

    public override async Task<CommandResult<Guid>> Handle(CreateReservationCommand command)
    {
        if (command.OrderRef == Guid.Empty ||
            command.OrderItemRef == Guid.Empty ||
            command.VariantRef == Guid.Empty ||
            command.SellerRef == Guid.Empty ||
            command.WarehouseRef == Guid.Empty ||
            command.ChannelRef == Guid.Empty)
        {
            return Fail("Order, item, variant, seller, warehouse and channel refs are required.");
        }

        if (command.RequestedQuantity <= 0)
            return Fail("RequestedQuantity must be greater than zero.");

        InventoryReservation reservation;
        try
        {
            reservation = InventoryReservation.Create(
                command.OrderRef,
                command.OrderItemRef,
                command.VariantRef,
                command.SellerRef,
                command.WarehouseRef,
                command.ChannelRef,
                command.RequestedQuantity,
                command.ExpiresAt,
                command.CorrelationId,
                command.IdempotencyKey,
                command.ReasonCode);
        }
        catch (Exception ex)
        {
            return Fail(ex.Message);
        }

        await _repository.InsertAsync(reservation);
        await _repository.CommitAsync();

        return Ok(reservation.BusinessKey.Value);
    }
}
