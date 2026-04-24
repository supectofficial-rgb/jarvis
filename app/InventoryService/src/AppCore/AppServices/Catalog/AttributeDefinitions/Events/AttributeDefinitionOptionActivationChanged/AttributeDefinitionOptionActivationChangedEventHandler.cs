namespace Insurance.InventoryService.AppCore.AppServices.Catalog.AttributeDefinitions.Events.AttributeDefinitionOptionActivationChanged;

using Insurance.InventoryService.AppCore.Domain.Catalog.Events;
using OysterFx.AppCore.Shared.Events;

public sealed class AttributeDefinitionOptionActivationChangedEventHandler : IDomainEventHandler<AttributeDefinitionOptionActivationChangedEvent>
{
    public Task Handle(AttributeDefinitionOptionActivationChangedEvent @event)
    {
        return Task.CompletedTask;
    }
}
