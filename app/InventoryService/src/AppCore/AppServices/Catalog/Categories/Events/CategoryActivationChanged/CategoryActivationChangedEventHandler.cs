namespace Insurance.InventoryService.AppCore.AppServices.Catalog.Categories.Events.CategoryActivationChanged;

using Insurance.InventoryService.AppCore.Domain.Catalog.Events;
using OysterFx.AppCore.Shared.Events;

public sealed class CategoryActivationChangedEventHandler : IDomainEventHandler<CategoryActivationChangedEvent>
{
    public Task Handle(CategoryActivationChangedEvent @event)
    {
        return Task.CompletedTask;
    }
}
