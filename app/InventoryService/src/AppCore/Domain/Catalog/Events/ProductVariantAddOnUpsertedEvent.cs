namespace Insurance.InventoryService.AppCore.Domain.Catalog.Events;

using OysterFx.AppCore.Domain.Events;
using OysterFx.AppCore.Domain.ValueObjects;

public sealed record ProductVariantAddOnUpsertedEvent(
    BusinessKey ProductVariantBusinessKey,
    Guid VariantAddOnBusinessKey,
    Guid? AddOnVariantRef,
    Guid? TagId,
    bool IsRequired) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
