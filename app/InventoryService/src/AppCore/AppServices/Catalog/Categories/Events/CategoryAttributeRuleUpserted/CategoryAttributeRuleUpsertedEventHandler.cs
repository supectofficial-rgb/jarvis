namespace Insurance.InventoryService.AppCore.AppServices.Catalog.Categories.Events.CategoryAttributeRuleUpserted;

using Insurance.InventoryService.AppCore.Domain.Catalog.Events;
using OysterFx.AppCore.Shared.Events;

public sealed class CategoryAttributeRuleUpsertedEventHandler : IDomainEventHandler<CategoryAttributeRuleUpsertedEvent>
{
    public Task Handle(CategoryAttributeRuleUpsertedEvent @event)
    {
        return Task.CompletedTask;
    }
}
