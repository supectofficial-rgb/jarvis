namespace Insurance.InventoryService.AppCore.AppServices.Catalog.UnitOfMeasures.Events.UnitOfMeasureActivationChanged;

using Insurance.InventoryService.AppCore.Domain.Catalog.Events;
using OysterFx.AppCore.Shared.Events;

public sealed class UnitOfMeasureActivationChangedEventHandler : IDomainEventHandler<UnitOfMeasureActivationChangedEvent>
{
    public Task Handle(UnitOfMeasureActivationChangedEvent @event) => Task.CompletedTask;
}
