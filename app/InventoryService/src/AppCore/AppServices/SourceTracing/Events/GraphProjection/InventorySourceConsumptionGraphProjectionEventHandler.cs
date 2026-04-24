namespace Insurance.InventoryService.AppCore.AppServices.SourceTracing.Events.GraphProjection;

using Insurance.InventoryService.AppCore.Domain.SourceTracing.Events;
using OysterFx.AppCore.Shared.Events;

public sealed class InventorySourceConsumptionGraphProjectionEventHandler :
    IDomainEventHandler<InventorySourceConsumptionCreatedEvent>
{
    public Task Handle(InventorySourceConsumptionCreatedEvent @event) => Task.CompletedTask;
}
