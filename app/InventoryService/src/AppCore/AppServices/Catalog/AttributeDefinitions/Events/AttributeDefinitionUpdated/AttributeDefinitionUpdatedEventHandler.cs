namespace Insurance.InventoryService.AppCore.AppServices.Catalog.AttributeDefinitions.Events.AttributeDefinitionUpdated;

using Insurance.InventoryService.AppCore.Domain.Catalog.Events;
using OysterFx.AppCore.Shared.Events;

public sealed class AttributeDefinitionUpdatedEventHandler : IDomainEventHandler<AttributeDefinitionUpdatedEvent>
{
    public Task Handle(AttributeDefinitionUpdatedEvent @event)
    {
        return Task.CompletedTask;
    }
}
