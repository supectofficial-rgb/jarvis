namespace Insurance.InventoryService.AppCore.Domain.Catalog.Events;

using OysterFx.AppCore.Domain.Events;
using OysterFx.AppCore.Domain.ValueObjects;

public sealed record CategoryMovedEvent : IDomainEvent
{
    public BusinessKey CategoryBusinessKey { get; }
    public Guid? PreviousParentCategoryRef { get; }
    public Guid? ParentCategoryRef { get; }
    public DateTime OccurredOn { get; }

    public CategoryMovedEvent(BusinessKey categoryBusinessKey, Guid? previousParentCategoryRef, Guid? parentCategoryRef)
    {
        CategoryBusinessKey = categoryBusinessKey;
        PreviousParentCategoryRef = previousParentCategoryRef;
        ParentCategoryRef = parentCategoryRef;
        OccurredOn = DateTime.UtcNow;
    }
}
