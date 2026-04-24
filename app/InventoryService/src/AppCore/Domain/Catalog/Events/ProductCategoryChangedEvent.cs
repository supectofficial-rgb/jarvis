namespace Insurance.InventoryService.AppCore.Domain.Catalog.Events;

using OysterFx.AppCore.Domain.Events;
using OysterFx.AppCore.Domain.ValueObjects;

public sealed record ProductCategoryChangedEvent : IDomainEvent
{
    public BusinessKey ProductBusinessKey { get; }
    public Guid PreviousCategoryRef { get; }
    public Guid PreviousCategorySchemaVersionRef { get; }
    public Guid CategoryRef { get; }
    public Guid CategorySchemaVersionRef { get; }
    public DateTime OccurredOn { get; }

    public ProductCategoryChangedEvent(
        BusinessKey productBusinessKey,
        Guid previousCategoryRef,
        Guid previousCategorySchemaVersionRef,
        Guid categoryRef,
        Guid categorySchemaVersionRef)
    {
        ProductBusinessKey = productBusinessKey;
        PreviousCategoryRef = previousCategoryRef;
        PreviousCategorySchemaVersionRef = previousCategorySchemaVersionRef;
        CategoryRef = categoryRef;
        CategorySchemaVersionRef = categorySchemaVersionRef;
        OccurredOn = DateTime.UtcNow;
    }
}
