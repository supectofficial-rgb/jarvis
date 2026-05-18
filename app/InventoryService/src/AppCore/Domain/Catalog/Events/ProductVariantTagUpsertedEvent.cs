namespace Insurance.InventoryService.AppCore.Domain.Catalog.Events;

using OysterFx.AppCore.Domain.Events;
using OysterFx.AppCore.Domain.ValueObjects;

public sealed record ProductVariantTagUpsertedEvent(
    BusinessKey ProductVariantBusinessKey,
    Guid VariantTagBusinessKey,
    string TagName,
    string? TagColor,
    int DisplayOrder) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
