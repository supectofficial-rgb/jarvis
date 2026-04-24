namespace Insurance.InventoryService.AppCore.AppServices.Catalog.AttributeDefinitions.Events.AttributeDefinitionActivationChanged;

using Insurance.InventoryService.AppCore.Domain.Catalog.Events;
using OysterFx.AppCore.Shared.Events;

public sealed class AttributeDefinitionActivationChangedEventHandler : IDomainEventHandler<AttributeDefinitionActivationChangedEvent>
{
    public Task Handle(AttributeDefinitionActivationChangedEvent @event)
    {
        return Task.CompletedTask;
    }
}
