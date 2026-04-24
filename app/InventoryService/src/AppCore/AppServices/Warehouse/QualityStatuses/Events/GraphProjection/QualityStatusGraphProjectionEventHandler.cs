namespace Insurance.InventoryService.AppCore.AppServices.Warehouse.QualityStatuses.Events.GraphProjection;

using Insurance.InventoryService.AppCore.Domain.Warehouse.Events;
using OysterFx.AppCore.Shared.Events;

public sealed class QualityStatusGraphProjectionEventHandler :
    IDomainEventHandler<QualityStatusCreatedEvent>,
    IDomainEventHandler<QualityStatusUpdatedEvent>,
    IDomainEventHandler<QualityStatusActivatedEvent>,
    IDomainEventHandler<QualityStatusDeactivatedEvent>
{
    public Task Handle(QualityStatusCreatedEvent @event) => Task.CompletedTask;

    public Task Handle(QualityStatusUpdatedEvent @event) => Task.CompletedTask;

    public Task Handle(QualityStatusActivatedEvent @event) => Task.CompletedTask;

    public Task Handle(QualityStatusDeactivatedEvent @event) => Task.CompletedTask;
}
