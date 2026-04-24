namespace Insurance.InventoryService.AppCore.AppServices.Reservations.Events.GraphProjection;

using Insurance.InventoryService.AppCore.Domain.Reservations.Events;
using OysterFx.AppCore.Shared.Events;

public sealed class InventoryReservationGraphProjectionEventHandler :
    IDomainEventHandler<ReservationCreatedEvent>,
    IDomainEventHandler<ReservationConfirmedEvent>,
    IDomainEventHandler<ReservationReleasedEvent>,
    IDomainEventHandler<ReservationExpiredEvent>,
    IDomainEventHandler<ReservationConsumedEvent>,
    IDomainEventHandler<ReservationRejectedEvent>
{
    public Task Handle(ReservationCreatedEvent @event) => Task.CompletedTask;

    public Task Handle(ReservationConfirmedEvent @event) => Task.CompletedTask;

    public Task Handle(ReservationReleasedEvent @event) => Task.CompletedTask;

    public Task Handle(ReservationExpiredEvent @event) => Task.CompletedTask;

    public Task Handle(ReservationConsumedEvent @event) => Task.CompletedTask;

    public Task Handle(ReservationRejectedEvent @event) => Task.CompletedTask;
}
