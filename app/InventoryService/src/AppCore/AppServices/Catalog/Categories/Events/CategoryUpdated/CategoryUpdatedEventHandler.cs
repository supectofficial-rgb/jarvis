namespace Insurance.InventoryService.AppCore.AppServices.Catalog.Categories.Events.CategoryUpdated;

using Insurance.InventoryService.AppCore.Domain.Catalog.Events;
using OysterFx.AppCore.Shared.Events;

public sealed class CategoryUpdatedEventHandler : IDomainEventHandler<CategoryUpdatedEvent>
{
    public Task Handle(CategoryUpdatedEvent @event)
    {
        return Task.CompletedTask;
    }
}
