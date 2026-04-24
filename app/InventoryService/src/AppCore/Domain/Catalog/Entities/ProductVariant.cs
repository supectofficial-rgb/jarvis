namespace Insurance.InventoryService.AppCore.Domain.Catalog.Entities;

using Insurance.InventoryService.AppCore.Domain.Catalog.Events;
using OysterFx.AppCore.Domain.Aggregates;
using OysterFx.AppCore.Domain.Exceptions;

public sealed class ProductVariant : AggregateRoot
{
    private readonly List<VariantAttributeValue> _attributeValues = new();
    private readonly List<VariantUomConversion> _uomConversions = new();

    public Guid ProductRef { get; private set; }
    public string VariantSku { get; private set; } = string.Empty;
    public string? Barcode { get; private set; }
    public TrackingPolicy TrackingPolicy { get; private set; }
    public Guid BaseUomRef { get; private set; }
    public bool IsActive { get; private set; }
    public bool InventoryMovementLocked { get; private set; }
    public IReadOnlyCollection<VariantAttributeValue> AttributeValues => _attributeValues.AsReadOnly();
    public IReadOnlyCollection<VariantUomConversion> UomConversions => _uomConversions.AsReadOnly();

    private ProductVariant()
    {
    }

    public static ProductVariant Create(Guid productRef, string variantSku, string? barcode, TrackingPolicy trackingPolicy, Guid baseUomRef)
    {
        var variant = new ProductVariant();

        variant.Apply(new ProductVariantCreatedEvent(
            variant.BusinessKey,
            productRef,
            NormalizeRequired(variantSku, nameof(variantSku)),
            NormalizeOptional(barcode),
            trackingPolicy,
            baseUomRef,
            true,
            false,
            Array.Empty<ProductVariantAttributeValueSnapshot>(),
            Array.Empty<ProductVariantUomConversionSnapshot>()));

        return variant;
    }

    public void ChangeVariantSku(string variantSku)
    {
        var normalized = NormalizeRequired(variantSku, nameof(variantSku));
        if (string.Equals(VariantSku, normalized, StringComparison.Ordinal))
            return;

        RaiseUpdatedEvent(variantSku: normalized);
    }

    public void ChangeBarcode(string? barcode)
    {
        var normalized = NormalizeOptional(barcode);
        if (Barcode == normalized)
            return;

        RaiseUpdatedEvent(barcode: normalized, updateBarcode: true);
    }

    public void ChangeTrackingPolicy(TrackingPolicy trackingPolicy)
    {
        EnsureInventoryControlledFieldsAreMutable();
        if (TrackingPolicy == trackingPolicy)
            return;

        Apply(new ProductVariantTrackingPolicyChangedEvent(BusinessKey, TrackingPolicy, trackingPolicy));
        RaiseUpdatedEvent(trackingPolicy: trackingPolicy);
    }

    public void ChangeBaseUom(Guid baseUomRef)
    {
        EnsureInventoryControlledFieldsAreMutable();
        if (BaseUomRef == baseUomRef)
            return;

        Apply(new ProductVariantBaseUomChangedEvent(BusinessKey, BaseUomRef, baseUomRef));
        RaiseUpdatedEvent(baseUomRef: baseUomRef);
    }

    public void MarkInventoryMovementStarted()
    {
        if (InventoryMovementLocked)
            return;

        Apply(new ProductVariantInventoryMovementLockedEvent(BusinessKey, true));
    }

    public void Activate()
    {
        if (IsActive)
            return;

        Apply(new ProductVariantActivationChangedEvent(BusinessKey, true));
    }

    public void Deactivate()
    {
        if (!IsActive)
            return;

        Apply(new ProductVariantActivationChangedEvent(BusinessKey, false));
    }

    public VariantAttributeValue SetAttributeValue(Guid attributeRef, string? value, Guid? optionRef = null)
    {
        if (attributeRef == Guid.Empty)
            throw new ArgumentException("AttributeRef is required.", nameof(attributeRef));

        Apply(new ProductVariantAttributeValueUpsertedEvent(BusinessKey, attributeRef, value, optionRef));
        return _attributeValues.First(x => x.AttributeRef == attributeRef);
    }

    public void RemoveAttributeValue(Guid attributeRef)
    {
        var existing = _attributeValues.FirstOrDefault(x => x.AttributeRef == attributeRef);
        if (existing is null)
            return;

        Apply(new ProductVariantAttributeValueRemovedEvent(BusinessKey, attributeRef));
    }

    public VariantUomConversion AddOrUpdateConversion(Guid fromUomRef, Guid toUomRef, decimal factor, UomRoundingMode roundingMode, bool isBasePath)
    {
        if (factor <= 0)
            throw new ArgumentOutOfRangeException(nameof(factor), "Factor must be greater than zero.");

        if (fromUomRef == Guid.Empty)
            throw new ArgumentException("FromUomRef is required.", nameof(fromUomRef));

        if (toUomRef == Guid.Empty)
            throw new ArgumentException("ToUomRef is required.", nameof(toUomRef));

        Apply(new ProductVariantUomConversionUpsertedEvent(BusinessKey, fromUomRef, toUomRef, factor, roundingMode, isBasePath));

        return _uomConversions.First(x => x.FromUomRef == fromUomRef && x.ToUomRef == toUomRef);
    }

    public void RemoveConversion(Guid fromUomRef, Guid toUomRef)
    {
        var existing = _uomConversions.FirstOrDefault(x => x.FromUomRef == fromUomRef && x.ToUomRef == toUomRef);
        if (existing is null)
            return;

        Apply(new ProductVariantUomConversionRemovedEvent(BusinessKey, fromUomRef, toUomRef));
    }

    private void EnsureInventoryControlledFieldsAreMutable()
    {
        if (InventoryMovementLocked)
            throw new AggregateStateExceptions("Tracking policy and base UOM cannot change after first inventory movement.", nameof(InventoryMovementLocked));
    }

    private static string NormalizeRequired(string value, string paramName)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Value is required.", paramName);

        return value.Trim();
    }

    private static string? NormalizeOptional(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();

    private void RaiseUpdatedEvent(
        string? variantSku = null,
        string? barcode = null,
        bool updateBarcode = false,
        TrackingPolicy? trackingPolicy = null,
        Guid? baseUomRef = null,
        bool? isActive = null,
        bool? inventoryMovementLocked = null)
    {
        var nextBarcode = updateBarcode ? barcode : Barcode;

        Apply(new ProductVariantUpdatedEvent(
            BusinessKey,
            ProductRef,
            variantSku ?? VariantSku,
            nextBarcode,
            trackingPolicy ?? TrackingPolicy,
            baseUomRef ?? BaseUomRef,
            isActive ?? IsActive,
            inventoryMovementLocked ?? InventoryMovementLocked,
            SnapshotAttributeValues(_attributeValues),
            SnapshotUomConversions(_uomConversions)));
    }

    private void On(ProductVariantCreatedEvent @event)
    {
        ProductRef = @event.ProductRef;
        VariantSku = @event.VariantSku;
        Barcode = @event.Barcode;
        TrackingPolicy = @event.TrackingPolicy;
        BaseUomRef = @event.BaseUomRef;
        IsActive = @event.IsActive;
        InventoryMovementLocked = @event.InventoryMovementLocked;
        SyncAttributeValues(@event.AttributeValues);
        SyncUomConversions(@event.UomConversions);
    }

    private void On(ProductVariantUpdatedEvent @event)
    {
        ProductRef = @event.ProductRef;
        VariantSku = @event.VariantSku;
        Barcode = @event.Barcode;
        TrackingPolicy = @event.TrackingPolicy;
        BaseUomRef = @event.BaseUomRef;
        IsActive = @event.IsActive;
        InventoryMovementLocked = @event.InventoryMovementLocked;
        SyncAttributeValues(@event.AttributeValues);
        SyncUomConversions(@event.UomConversions);
    }

    private void On(ProductVariantTrackingPolicyChangedEvent @event)
    {
        TrackingPolicy = @event.TrackingPolicy;
    }

    private void On(ProductVariantBaseUomChangedEvent @event)
    {
        BaseUomRef = @event.BaseUomRef;
    }

    private void On(ProductVariantActivationChangedEvent @event)
    {
        IsActive = @event.IsActive;
    }

    private void On(ProductVariantInventoryMovementLockedEvent @event)
    {
        InventoryMovementLocked = @event.InventoryMovementLocked;
    }

    private void On(ProductVariantAttributeValueUpsertedEvent @event)
    {
        if (@event.AttributeRef == Guid.Empty)
            return;

        var existing = _attributeValues.FirstOrDefault(x => x.AttributeRef == @event.AttributeRef);
        if (existing is null)
        {
            _attributeValues.Add(VariantAttributeValue.Create(BusinessKey.Value, @event.AttributeRef, @event.Value, @event.OptionRef));
            return;
        }

        existing.Update(@event.Value, @event.OptionRef);
    }

    private void On(ProductVariantAttributeValueRemovedEvent @event)
    {
        var existing = _attributeValues.FirstOrDefault(x => x.AttributeRef == @event.AttributeRef);
        if (existing is null)
            return;

        _attributeValues.Remove(existing);
    }

    private void On(ProductVariantUomConversionUpsertedEvent @event)
    {
        var existing = _uomConversions.FirstOrDefault(x => x.FromUomRef == @event.FromUomRef && x.ToUomRef == @event.ToUomRef);
        if (existing is null)
        {
            _uomConversions.Add(VariantUomConversion.Create(
                BusinessKey.Value,
                @event.FromUomRef,
                @event.ToUomRef,
                @event.Factor,
                @event.RoundingMode,
                @event.IsBasePath));
            return;
        }

        existing.Update(@event.Factor, @event.RoundingMode, @event.IsBasePath);
    }

    private void On(ProductVariantUomConversionRemovedEvent @event)
    {
        var existing = _uomConversions.FirstOrDefault(x => x.FromUomRef == @event.FromUomRef && x.ToUomRef == @event.ToUomRef);
        if (existing is null)
            return;

        _uomConversions.Remove(existing);
    }

    private void SyncAttributeValues(IReadOnlyCollection<ProductVariantAttributeValueSnapshot> values)
    {
        _attributeValues.Clear();

        if (values is null || values.Count == 0)
            return;

        foreach (var snapshot in values)
        {
            if (snapshot.AttributeRef == Guid.Empty)
                continue;

            _attributeValues.Add(VariantAttributeValue.Create(BusinessKey.Value, snapshot.AttributeRef, snapshot.Value, snapshot.OptionRef));
        }
    }

    private void SyncUomConversions(IReadOnlyCollection<ProductVariantUomConversionSnapshot> conversions)
    {
        _uomConversions.Clear();

        if (conversions is null || conversions.Count == 0)
            return;

        foreach (var snapshot in conversions)
        {
            if (snapshot.FromUomRef == Guid.Empty || snapshot.ToUomRef == Guid.Empty)
                continue;

            _uomConversions.Add(VariantUomConversion.Create(
                BusinessKey.Value,
                snapshot.FromUomRef,
                snapshot.ToUomRef,
                snapshot.Factor,
                snapshot.RoundingMode,
                snapshot.IsBasePath));
        }
    }

    private static IReadOnlyCollection<ProductVariantAttributeValueSnapshot> SnapshotAttributeValues(IEnumerable<VariantAttributeValue> values)
    {
        return values
            .Select(x => new ProductVariantAttributeValueSnapshot(x.AttributeRef, x.Value, x.OptionRef))
            .ToList();
    }

    private static IReadOnlyCollection<ProductVariantUomConversionSnapshot> SnapshotUomConversions(IEnumerable<VariantUomConversion> conversions)
    {
        return conversions
            .Select(x => new ProductVariantUomConversionSnapshot(x.FromUomRef, x.ToUomRef, x.Factor, x.RoundingMode, x.IsBasePath))
            .ToList();
    }
}
