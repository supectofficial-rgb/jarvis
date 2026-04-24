namespace Insurance.InventoryService.AppCore.AppServices.Catalog.AttributeDefinitions.Events.AttributeDefinitionCreated;

using Insurance.InventoryService.AppCore.Domain.Catalog.Events;
using OysterFx.AppCore.Shared.Events;

public sealed class AttributeDefinitionCreatedEventHandler : IDomainEventHandler<AttributeDefinitionCreatedEvent>
{
    public Task Handle(AttributeDefinitionCreatedEvent @event)
    {
        return Task.CompletedTask;
    }
}
