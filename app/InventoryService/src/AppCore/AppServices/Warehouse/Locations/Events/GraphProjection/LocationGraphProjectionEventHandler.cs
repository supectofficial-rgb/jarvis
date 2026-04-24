namespace Insurance.InventoryService.AppCore.AppServices.Warehouse.Locations.Events.GraphProjection;

using Insurance.InventoryService.AppCore.Domain.Warehouse.Events;
using OysterFx.AppCore.Shared.Events;

public sealed class LocationGraphProjectionEventHandler :
    IDomainEventHandler<LocationCreatedEvent>,
    IDomainEventHandler<LocationUpdatedEvent>,
    IDomainEventHandler<LocationActivatedEvent>,
    IDomainEventHandler<LocationDeactivatedEvent>,
    IDomainEventHandler<LocationMovedToWarehouseEvent>
{
    public Task Handle(LocationCreatedEvent @event) => Task.CompletedTask;

    public Task Handle(LocationUpdatedEvent @event) => Task.CompletedTask;

    public Task Handle(LocationActivatedEvent @event) => Task.CompletedTask;

    public Task Handle(LocationDeactivatedEvent @event) => Task.CompletedTask;

    public Task Handle(LocationMovedToWarehouseEvent @event) => Task.CompletedTask;
}
