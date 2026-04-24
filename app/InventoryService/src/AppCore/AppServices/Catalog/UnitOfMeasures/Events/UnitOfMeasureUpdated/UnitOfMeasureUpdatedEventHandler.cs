namespace Insurance.InventoryService.AppCore.AppServices.Catalog.UnitOfMeasures.Events.UnitOfMeasureUpdated;

using Insurance.InventoryService.AppCore.Domain.Catalog.Events;
using OysterFx.AppCore.Shared.Events;

public sealed class UnitOfMeasureUpdatedEventHandler : IDomainEventHandler<UnitOfMeasureUpdatedEvent>
{
    public Task Handle(UnitOfMeasureUpdatedEvent @event) => Task.CompletedTask;
}
