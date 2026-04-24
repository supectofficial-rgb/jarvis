namespace Insurance.InventoryService.AppCore.AppServices.Reservations.Events.GraphProjection;

using Insurance.InventoryService.AppCore.Domain.Reservations.Events;
using OysterFx.AppCore.Shared.Events;

public sealed class ReservationAllocationGraphProjectionEventHandler :
    IDomainEventHandler<ReservationAllocationAllocatedEvent>,
    IDomainEventHandler<ReservationAllocationReleasedEvent>,
    IDomainEventHandler<ReservationAllocationConsumedEvent>
{
    public Task Handle(ReservationAllocationAllocatedEvent @event) => Task.CompletedTask;

    public Task Handle(ReservationAllocationReleasedEvent @event) => Task.CompletedTask;

    public Task Handle(ReservationAllocationConsumedEvent @event) => Task.CompletedTask;
}
