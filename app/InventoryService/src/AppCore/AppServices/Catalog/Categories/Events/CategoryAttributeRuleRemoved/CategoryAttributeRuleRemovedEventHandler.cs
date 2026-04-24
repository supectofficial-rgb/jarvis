namespace Insurance.InventoryService.AppCore.AppServices.Catalog.Categories.Events.CategoryAttributeRuleRemoved;

using Insurance.InventoryService.AppCore.Domain.Catalog.Events;
using OysterFx.AppCore.Shared.Events;

public sealed class CategoryAttributeRuleRemovedEventHandler : IDomainEventHandler<CategoryAttributeRuleRemovedEvent>
{
    public Task Handle(CategoryAttributeRuleRemovedEvent @event)
    {
        return Task.CompletedTask;
    }
}
