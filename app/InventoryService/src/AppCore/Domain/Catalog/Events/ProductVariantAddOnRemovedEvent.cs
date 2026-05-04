namespace Insurance.InventoryService.AppCore.Domain.Catalog.Events;

using OysterFx.AppCore.Domain.Events;
using OysterFx.AppCore.Domain.ValueObjects;

public sealed record ProductVariantAddOnRemovedEvent(
    BusinessKey ProductVariantBusinessKey,
    Guid AddOnVariantRef) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
