namespace Insurance.InventoryService.AppCore.AppServices.SourceTracing.Events.GraphProjection;

using Insurance.InventoryService.AppCore.Domain.SourceTracing.Events;
using OysterFx.AppCore.Shared.Events;

public sealed class InventorySourceBalanceGraphProjectionEventHandler :
    IDomainEventHandler<InventorySourceBalanceOpenedEvent>,
    IDomainEventHandler<InventorySourceBalanceAllocatedEvent>,
    IDomainEventHandler<InventorySourceBalanceConsumedEvent>,
    IDomainEventHandler<InventorySourceBalanceClosedEvent>
{
    public Task Handle(InventorySourceBalanceOpenedEvent @event) => Task.CompletedTask;

    public Task Handle(InventorySourceBalanceAllocatedEvent @event) => Task.CompletedTask;

    public Task Handle(InventorySourceBalanceConsumedEvent @event) => Task.CompletedTask;

    public Task Handle(InventorySourceBalanceClosedEvent @event) => Task.CompletedTask;
}
