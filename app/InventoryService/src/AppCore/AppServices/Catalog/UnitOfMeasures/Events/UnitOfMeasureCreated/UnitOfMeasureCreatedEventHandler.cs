namespace Insurance.InventoryService.AppCore.AppServices.Catalog.UnitOfMeasures.Events.UnitOfMeasureCreated;

using Insurance.InventoryService.AppCore.Domain.Catalog.Events;
using OysterFx.AppCore.Shared.Events;

public sealed class UnitOfMeasureCreatedEventHandler : IDomainEventHandler<UnitOfMeasureCreatedEvent>
{
    public Task Handle(UnitOfMeasureCreatedEvent @event) => Task.CompletedTask;
}
