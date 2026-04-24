namespace Insurance.InventoryService.AppCore.Domain.Catalog.Events;

using Insurance.InventoryService.AppCore.Domain.Catalog.Entities;
using OysterFx.AppCore.Domain.Events;
using OysterFx.AppCore.Domain.ValueObjects;

public sealed record ProductVariantTrackingPolicyChangedEvent : IDomainEvent
{
    public BusinessKey ProductVariantBusinessKey { get; }
    public TrackingPolicy PreviousTrackingPolicy { get; }
    public TrackingPolicy TrackingPolicy { get; }
    public DateTime OccurredOn { get; }

    public ProductVariantTrackingPolicyChangedEvent(
        BusinessKey productVariantBusinessKey,
        TrackingPolicy previousTrackingPolicy,
        TrackingPolicy trackingPolicy)
    {
        ProductVariantBusinessKey = productVariantBusinessKey;
        PreviousTrackingPolicy = previousTrackingPolicy;
        TrackingPolicy = trackingPolicy;
        OccurredOn = DateTime.UtcNow;
    }
}
