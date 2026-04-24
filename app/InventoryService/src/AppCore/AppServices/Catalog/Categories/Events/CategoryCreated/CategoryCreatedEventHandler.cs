namespace Insurance.InventoryService.AppCore.AppServices.Catalog.Categories.Events.CategoryCreated;

using Insurance.InventoryService.AppCore.Domain.Catalog.Events;
using OysterFx.AppCore.Shared.Events;

public sealed class CategoryCreatedEventHandler : IDomainEventHandler<CategoryCreatedEvent>
{
    public Task Handle(CategoryCreatedEvent @event)
    {
        return Task.CompletedTask;
    }
}
