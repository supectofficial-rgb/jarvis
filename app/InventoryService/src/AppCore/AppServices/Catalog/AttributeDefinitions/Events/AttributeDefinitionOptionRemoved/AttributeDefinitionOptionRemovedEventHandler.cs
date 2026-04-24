namespace Insurance.InventoryService.AppCore.AppServices.Catalog.AttributeDefinitions.Events.AttributeDefinitionOptionRemoved;

using Insurance.InventoryService.AppCore.Domain.Catalog.Events;
using OysterFx.AppCore.Shared.Events;

public sealed class AttributeDefinitionOptionRemovedEventHandler : IDomainEventHandler<AttributeDefinitionOptionRemovedEvent>
{
    public Task Handle(AttributeDefinitionOptionRemovedEvent @event)
    {
        return Task.CompletedTask;
    }
}
