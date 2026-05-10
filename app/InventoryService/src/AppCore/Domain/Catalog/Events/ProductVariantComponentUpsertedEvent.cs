namespace Insurance.InventoryService.AppCore.Domain.Catalog.Events;

using OysterFx.AppCore.Domain.Events;
using OysterFx.AppCore.Domain.ValueObjects;

public sealed record ProductVariantComponentUpsertedEvent(
    BusinessKey ProductVariantBusinessKey,
    Guid VariantComponentBusinessKey,
    Guid ComponentVariantRef,
    Guid WarehouseRef,
    Guid LocationRef,
    decimal Quantity) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
