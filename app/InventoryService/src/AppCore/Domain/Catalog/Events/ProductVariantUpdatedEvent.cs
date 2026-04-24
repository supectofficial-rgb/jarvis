namespace Insurance.InventoryService.AppCore.Domain.Catalog.Events;

using Insurance.InventoryService.AppCore.Domain.Catalog.Entities;
using OysterFx.AppCore.Domain.Events;
using OysterFx.AppCore.Domain.ValueObjects;

public sealed record ProductVariantUpdatedEvent : IDomainEvent
{
    public BusinessKey ProductVariantBusinessKey { get; }
    public Guid ProductRef { get; }
    public string VariantSku { get; }
    public string? Barcode { get; }
    public TrackingPolicy TrackingPolicy { get; }
    public Guid BaseUomRef { get; }
    public bool IsActive { get; }
    public bool InventoryMovementLocked { get; }
    public IReadOnlyCollection<ProductVariantAttributeValueSnapshot> AttributeValues { get; }
    public IReadOnlyCollection<ProductVariantUomConversionSnapshot> UomConversions { get; }
    public DateTime OccurredOn { get; }

    public ProductVariantUpdatedEvent(
        BusinessKey productVariantBusinessKey,
        Guid productRef,
        string variantSku,
        string? barcode,
        TrackingPolicy trackingPolicy,
        Guid baseUomRef,
        bool isActive,
        bool inventoryMovementLocked,
        IReadOnlyCollection<ProductVariantAttributeValueSnapshot> attributeValues,
        IReadOnlyCollection<ProductVariantUomConversionSnapshot> uomConversions)
    {
        ProductVariantBusinessKey = productVariantBusinessKey;
        ProductRef = productRef;
        VariantSku = variantSku;
        Barcode = barcode;
        TrackingPolicy = trackingPolicy;
        BaseUomRef = baseUomRef;
        IsActive = isActive;
        InventoryMovementLocked = inventoryMovementLocked;
        AttributeValues = attributeValues;
        UomConversions = uomConversions;
        OccurredOn = DateTime.UtcNow;
    }
}
