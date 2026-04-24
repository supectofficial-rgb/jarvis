namespace Insurance.InventoryService.AppCore.AppServices.Catalog.Categories.Events.CategoryMoved;

using Insurance.InventoryService.AppCore.Domain.Catalog.Events;
using OysterFx.AppCore.Shared.Events;

public sealed class CategoryMovedEventHandler : IDomainEventHandler<CategoryMovedEvent>
{
    public Task Handle(CategoryMovedEvent @event)
    {
        return Task.CompletedTask;
    }
}
