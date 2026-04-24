namespace Insurance.InventoryService.AppCore.AppServices.Catalog.AttributeDefinitions.Events.AttributeDefinitionOptionAdded;

using Insurance.InventoryService.AppCore.Domain.Catalog.Events;
using OysterFx.AppCore.Shared.Events;

public sealed class AttributeDefinitionOptionAddedEventHandler : IDomainEventHandler<AttributeDefinitionOptionAddedEvent>
{
    public Task Handle(AttributeDefinitionOptionAddedEvent @event)
    {
        return Task.CompletedTask;
    }
}
