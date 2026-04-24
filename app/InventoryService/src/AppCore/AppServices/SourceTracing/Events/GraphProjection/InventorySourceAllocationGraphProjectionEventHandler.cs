namespace Insurance.InventoryService.AppCore.AppServices.SourceTracing.Events.GraphProjection;

using Insurance.InventoryService.AppCore.Domain.SourceTracing.Events;
using OysterFx.AppCore.Shared.Events;

public sealed class InventorySourceAllocationGraphProjectionEventHandler :
    IDomainEventHandler<InventorySourceAllocationCreatedEvent>,
    IDomainEventHandler<InventorySourceAllocationReleasedEvent>
{
    public Task Handle(InventorySourceAllocationCreatedEvent @event) => Task.CompletedTask;

    public Task Handle(InventorySourceAllocationReleasedEvent @event) => Task.CompletedTask;
}
