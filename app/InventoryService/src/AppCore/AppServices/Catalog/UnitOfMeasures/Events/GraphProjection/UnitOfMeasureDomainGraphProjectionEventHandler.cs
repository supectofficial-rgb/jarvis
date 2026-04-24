namespace Insurance.InventoryService.AppCore.AppServices.Catalog.UnitOfMeasures.Events.GraphProjection;

using Insurance.InventoryService.AppCore.Domain.Catalog.Events;
using OysterFx.AppCore.Shared.Events;

public sealed class UnitOfMeasureDomainGraphProjectionEventHandler :
    IDomainEventHandler<UnitOfMeasureCreatedEvent>,
    IDomainEventHandler<UnitOfMeasureUpdatedEvent>,
    IDomainEventHandler<UnitOfMeasureActivatedEvent>,
    IDomainEventHandler<UnitOfMeasureDeactivatedEvent>
{
    public Task Handle(UnitOfMeasureCreatedEvent @event) => Task.CompletedTask;

    public Task Handle(UnitOfMeasureUpdatedEvent @event) => Task.CompletedTask;

    public Task Handle(UnitOfMeasureActivatedEvent @event) => Task.CompletedTask;

    public Task Handle(UnitOfMeasureDeactivatedEvent @event) => Task.CompletedTask;
}
