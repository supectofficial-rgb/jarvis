namespace Insurance.InventoryService.AppCore.AppServices.Catalog.AttributeDefinitions.Events.AttributeDefinitionOptionUpdated;

using Insurance.InventoryService.AppCore.Domain.Catalog.Events;
using OysterFx.AppCore.Shared.Events;

public sealed class AttributeDefinitionOptionUpdatedEventHandler : IDomainEventHandler<AttributeDefinitionOptionUpdatedEvent>
{
    public Task Handle(AttributeDefinitionOptionUpdatedEvent @event)
    {
        return Task.CompletedTask;
    }
}
