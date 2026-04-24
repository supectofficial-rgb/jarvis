namespace Insurance.InventoryService.AppCore.AppServices.SerialItems.Events.GraphProjection;

using Insurance.InventoryService.AppCore.Domain.StockDetails.Events;
using OysterFx.AppCore.Shared.Events;

public sealed class SerialItemGraphProjectionEventHandler :
    IDomainEventHandler<SerialItemCreatedEvent>,
    IDomainEventHandler<SerialItemStatusUpdatedEvent>,
    IDomainEventHandler<SerialItemMovedEvent>,
    IDomainEventHandler<SerialItemAssignedToStockDetailEvent>
{
    public Task Handle(SerialItemCreatedEvent @event) => Task.CompletedTask;

    public Task Handle(SerialItemStatusUpdatedEvent @event) => Task.CompletedTask;

    public Task Handle(SerialItemMovedEvent @event) => Task.CompletedTask;

    public Task Handle(SerialItemAssignedToStockDetailEvent @event) => Task.CompletedTask;
}
