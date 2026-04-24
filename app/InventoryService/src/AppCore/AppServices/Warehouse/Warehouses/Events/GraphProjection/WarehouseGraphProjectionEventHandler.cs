namespace Insurance.InventoryService.AppCore.AppServices.Warehouse.Warehouses.Events.GraphProjection;

using Insurance.InventoryService.AppCore.Domain.Warehouse.Events;
using OysterFx.AppCore.Shared.Events;

public sealed class WarehouseGraphProjectionEventHandler :
    IDomainEventHandler<WarehouseCreatedEvent>,
    IDomainEventHandler<WarehouseUpdatedEvent>,
    IDomainEventHandler<WarehouseActivatedEvent>,
    IDomainEventHandler<WarehouseDeactivatedEvent>
{
    public Task Handle(WarehouseCreatedEvent @event) => Task.CompletedTask;

    public Task Handle(WarehouseUpdatedEvent @event) => Task.CompletedTask;

    public Task Handle(WarehouseActivatedEvent @event) => Task.CompletedTask;

    public Task Handle(WarehouseDeactivatedEvent @event) => Task.CompletedTask;
}
