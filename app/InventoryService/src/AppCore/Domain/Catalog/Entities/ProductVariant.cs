namespace Insurance.InventoryService.AppCore.Domain.Catalog.Entities;

using Insurance.InventoryService.AppCore.Domain.Catalog.Events;
using OysterFx.AppCore.Domain.Aggregates;
using OysterFx.AppCore.Domain.Exceptions;

public sealed class ProductVariant : AggregateRoot
{
    private readonly List<VariantAttributeValue> _attributeValues = new();
    private readonly List<VariantUomConversion> _uomConversions = new();
    private readonly List<VariantComponent> _components = new();
    private readonly List<VariantAddOn> _addOns = new();
    private readonly List<VariantImage> _images = new();
    private readonly List<VariantTag> _tags = new();

    public Guid ProductRef { get; private set; }
    public string VariantSku { get; private set; } = string.Empty;
    public string? VariantName { get; private set; }
    public string? Barcode { get; private set; }
    public TrackingPolicy TrackingPolicy { get; private set; }
    public Guid BaseUomRef { get; private set; }
    public bool IsActive { get; private set; }
    public bool InventoryMovementLocked { get; private set; }
    public IReadOnlyCollection<VariantAttributeValue> AttributeValues => _attributeValues.AsReadOnly();
    public IReadOnlyCollection<VariantUomConversion> UomConversions => _uomConversions.AsReadOnly();
    public IReadOnlyCollection<VariantComponent> Components => _components.AsReadOnly();
    public IReadOnlyCollection<VariantAddOn> AddOns => _addOns.AsReadOnly();
    public IReadOnlyCollection<VariantImage> Images => _images.AsReadOnly();
    public IReadOnlyCollection<VariantTag> Tags => _tags.AsReadOnly();

    private ProductVariant()
    {
    }

    public static ProductVariant Create(
        Guid productRef,
        string variantSku,
        string? barcode,
        TrackingPolicy trackingPolicy,
        Guid baseUomRef,
        string? variantName = null)
    {
        var variant = new ProductVariant();

        variant.Apply(new ProductVariantCreatedEvent(
            variant.BusinessKey,
            productRef,
            NormalizeRequired(variantSku, nameof(variantSku)),
            NormalizeOptional(variantName) ?? NormalizeRequired(variantSku, nameof(variantSku)),
            NormalizeOptional(barcode),
            trackingPolicy,
            baseUomRef,
            true,
            false,
            Array.Empty<ProductVariantAttributeValueSnapshot>(),
            Array.Empty<ProductVariantUomConversionSnapshot>(),
            Array.Empty<ProductVariantImageSnapshot>()));

        return variant;
    }

    public void ChangeVariantSku(string variantSku)
    {
        var normalized = NormalizeRequired(variantSku, nameof(variantSku));
        if (string.Equals(VariantSku, normalized, StringComparison.Ordinal))
            return;

        RaiseUpdatedEvent(variantSku: normalized);
    }

    public void ChangeVariantName(string? variantName)
    {
        var normalized = NormalizeOptional(variantName) ?? VariantSku;
        if (string.Equals(VariantName, normalized, StringComparison.Ordinal))
            return;

        RaiseUpdatedEvent(variantName: normalized, updateVariantName: true);
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

    public VariantComponent AddOrUpdateComponent(Guid? variantComponentBusinessKey, Guid componentVariantRef, decimal quantity)
    {
        if (componentVariantRef == Guid.Empty)
            throw new ArgumentException("ComponentVariantRef is required.", nameof(componentVariantRef));

        if (componentVariantRef == BusinessKey.Value)
            throw new ArgumentException("Variant cannot reference itself as a component.", nameof(componentVariantRef));

        if (quantity <= 0)
            throw new ArgumentOutOfRangeException(nameof(quantity), "Quantity must be greater than zero.");

        var existing = variantComponentBusinessKey.HasValue && variantComponentBusinessKey.Value != Guid.Empty
            ? _components.FirstOrDefault(x => x.BusinessKey.Value == variantComponentBusinessKey.Value)
            : _components.FirstOrDefault(x => x.ComponentVariantRef == componentVariantRef);

        var componentBusinessKey = existing?.BusinessKey.Value
            ?? (variantComponentBusinessKey.HasValue && variantComponentBusinessKey.Value != Guid.Empty
                ? variantComponentBusinessKey.Value
                : Guid.NewGuid());

        Apply(new ProductVariantComponentUpsertedEvent(
            BusinessKey,
            componentBusinessKey,
            componentVariantRef,
            quantity));

        return _components.First(x => x.BusinessKey.Value == componentBusinessKey);
    }

    public bool RemoveComponent(Guid variantComponentBusinessKey)
    {
        var existing = _components.FirstOrDefault(x => x.BusinessKey.Value == variantComponentBusinessKey);
        if (existing is null)
            return false;

        Apply(new ProductVariantComponentRemovedEvent(BusinessKey, existing.BusinessKey.Value));
        return true;
    }

    public VariantAddOn AddOrUpdateAddOn(Guid? addOnVariantRef, Guid? tagId, bool isRequired)
    {
        var hasVariantRef = addOnVariantRef.HasValue && addOnVariantRef.Value != Guid.Empty;
        var hasTagId = tagId.HasValue && tagId.Value != Guid.Empty;

        if (hasVariantRef == hasTagId)
            throw new ArgumentException("Exactly one of AddOnVariantRef or TagId is required.");

        if (hasVariantRef && addOnVariantRef == BusinessKey.Value)
            throw new ArgumentException("Variant cannot reference itself as an add-on.", nameof(addOnVariantRef));

        var existing = hasVariantRef
            ? _addOns.FirstOrDefault(x => x.AddOnVariantRef == addOnVariantRef)
            : _addOns.FirstOrDefault(x => x.TagId == tagId);

        if (existing is null)
        {
            existing = VariantAddOn.Create(BusinessKey, addOnVariantRef, tagId, isRequired, Guid.NewGuid());
            _addOns.Add(existing);
        }
        else
        {
            existing.Update(addOnVariantRef, tagId, isRequired);
        }

        Apply(new ProductVariantAddOnUpsertedEvent(
            BusinessKey,
            existing.BusinessKey.Value,
            addOnVariantRef,
            tagId,
            isRequired));
        return existing;
    }

    public bool RemoveAddOn(Guid variantAddOnBusinessKey)
    {
        var existing = _addOns.FirstOrDefault(x => x.BusinessKey.Value == variantAddOnBusinessKey);
        if (existing is null)
            return false;

        Apply(new ProductVariantAddOnRemovedEvent(BusinessKey, existing.BusinessKey.Value));
        return true;
    }

    public VariantTag AddOrUpdateTag(Guid? variantTagBusinessKey, Guid tagRef, string tagName, string? tagColor, int displayOrder)
    {
        var normalizedTagName = NormalizeRequired(tagName, nameof(tagName));
        var existing = variantTagBusinessKey.HasValue && variantTagBusinessKey.Value != Guid.Empty
            ? _tags.FirstOrDefault(x => x.BusinessKey.Value == variantTagBusinessKey.Value)
            : _tags.FirstOrDefault(x => x.TagRef == tagRef);

        var tagBusinessKey = existing?.BusinessKey.Value
            ?? (variantTagBusinessKey.HasValue && variantTagBusinessKey.Value != Guid.Empty
                ? variantTagBusinessKey.Value
                : Guid.NewGuid());

        Apply(new ProductVariantTagUpsertedEvent(
            BusinessKey,
            tagBusinessKey,
            tagRef,
            normalizedTagName,
            NormalizeOptional(tagColor),
            displayOrder));

        return _tags.First(x => x.BusinessKey.Value == tagBusinessKey);
    }

    public bool RemoveTag(Guid? variantTagBusinessKey)
    {
        var existing = variantTagBusinessKey.HasValue && variantTagBusinessKey.Value != Guid.Empty
            ? _tags.FirstOrDefault(x => x.BusinessKey.Value == variantTagBusinessKey.Value)
            : null;

        if (existing is null)
            return false;

        Apply(new ProductVariantTagRemovedEvent(BusinessKey, existing.BusinessKey.Value));
        return true;
    }

    public VariantImage AddOrUpdateImage(
        string fileKey,
        string originalFileName,
        string contentType,
        string originalUrl,
        string thumbnailUrl,
        int displayOrder,
        bool isPrimary)
    {
        var normalizedFileKey = NormalizeRequired(fileKey, nameof(fileKey));
        var normalizedOriginalFileName = NormalizeRequired(originalFileName, nameof(originalFileName));
        var normalizedContentType = NormalizeRequired(contentType, nameof(contentType));
        var normalizedOriginalUrl = NormalizeRequired(originalUrl, nameof(originalUrl));
        var normalizedThumbnailUrl = NormalizeRequired(thumbnailUrl, nameof(thumbnailUrl));

        Apply(new ProductVariantImageUpsertedEvent(
            BusinessKey,
            normalizedFileKey,
            normalizedOriginalFileName,
            normalizedContentType,
            normalizedOriginalUrl,
            normalizedThumbnailUrl,
            displayOrder,
            isPrimary));

        return _images.First(x => string.Equals(x.FileKey, normalizedFileKey, StringComparison.OrdinalIgnoreCase));
    }

    public bool RemoveImage(Guid variantImageBusinessKey)
    {
        if (variantImageBusinessKey == Guid.Empty)
            return false;

        var existing = _images.FirstOrDefault(x => x.BusinessKey.Value == variantImageBusinessKey);
        if (existing is null)
            return false;

        Apply(new ProductVariantImageRemovedEvent(BusinessKey, existing.FileKey));
        return true;
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
        string? variantName = null,
        string? barcode = null,
        bool updateBarcode = false,
        bool updateVariantName = false,
        TrackingPolicy? trackingPolicy = null,
        Guid? baseUomRef = null,
        bool? isActive = null,
        bool? inventoryMovementLocked = null,
        IReadOnlyCollection<ProductVariantImageSnapshot>? images = null,
        bool updateImages = false)
    {
        var nextBarcode = updateBarcode ? barcode : Barcode;
        var nextVariantName = updateVariantName ? (NormalizeOptional(variantName) ?? VariantSku) : VariantName;
        var nextImages = updateImages ? images ?? Array.Empty<ProductVariantImageSnapshot>() : SnapshotImages(_images);

        Apply(new ProductVariantUpdatedEvent(
            BusinessKey,
            ProductRef,
            variantSku ?? VariantSku,
            nextVariantName ?? variantSku ?? VariantSku,
            nextBarcode,
            trackingPolicy ?? TrackingPolicy,
            baseUomRef ?? BaseUomRef,
            isActive ?? IsActive,
            inventoryMovementLocked ?? InventoryMovementLocked,
            SnapshotAttributeValues(_attributeValues),
            SnapshotUomConversions(_uomConversions),
            nextImages));
    }

    private void On(ProductVariantCreatedEvent @event)
    {
        ProductRef = @event.ProductRef;
        VariantSku = @event.VariantSku;
        VariantName = @event.VariantName;
        Barcode = @event.Barcode;
        TrackingPolicy = @event.TrackingPolicy;
        BaseUomRef = @event.BaseUomRef;
        IsActive = @event.IsActive;
        InventoryMovementLocked = @event.InventoryMovementLocked;
        SyncAttributeValues(@event.AttributeValues);
        SyncUomConversions(@event.UomConversions);
        SyncImages(@event.Images);
    }

    private void On(ProductVariantUpdatedEvent @event)
    {
        ProductRef = @event.ProductRef;
        VariantSku = @event.VariantSku;
        VariantName = @event.VariantName;
        Barcode = @event.Barcode;
        TrackingPolicy = @event.TrackingPolicy;
        BaseUomRef = @event.BaseUomRef;
        IsActive = @event.IsActive;
        InventoryMovementLocked = @event.InventoryMovementLocked;
        SyncAttributeValues(@event.AttributeValues);
        SyncUomConversions(@event.UomConversions);
        SyncImages(@event.Images);
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

    private void On(ProductVariantComponentUpsertedEvent @event)
    {
        var existing = _components.FirstOrDefault(x => x.BusinessKey.Value == @event.VariantComponentBusinessKey);
        if (existing is null)
        {
            _components.Add(VariantComponent.Create(
                BusinessKey.Value,
                @event.VariantComponentBusinessKey,
                @event.ComponentVariantRef,
                @event.Quantity));
            return;
        }

        existing.Update(@event.ComponentVariantRef, @event.Quantity);
    }

    private void On(ProductVariantComponentRemovedEvent @event)
    {
        var existing = _components.FirstOrDefault(x => x.BusinessKey.Value == @event.VariantComponentBusinessKey);
        if (existing is null)
            return;

        _components.Remove(existing);
    }

    private void On(ProductVariantAddOnUpsertedEvent @event)
    {
        var existing = _addOns.FirstOrDefault(x => x.BusinessKey.Value == @event.VariantAddOnBusinessKey);
        if (existing is null)
        {
            _addOns.Add(VariantAddOn.Create(
                BusinessKey.Value,
                @event.AddOnVariantRef,
                @event.TagId,
                @event.IsRequired,
                @event.VariantAddOnBusinessKey));
            return;
        }

        existing.Update(@event.AddOnVariantRef, @event.TagId, @event.IsRequired);
    }

    private void On(ProductVariantAddOnRemovedEvent @event)
    {
        var existing = _addOns.FirstOrDefault(x => x.BusinessKey.Value == @event.VariantAddOnBusinessKey);
        if (existing is null)
            return;

        _addOns.Remove(existing);
    }

    private void On(ProductVariantImageUpsertedEvent @event)
    {
        var existing = _images.FirstOrDefault(x => string.Equals(x.FileKey, @event.FileKey, StringComparison.OrdinalIgnoreCase));

        if (@event.IsPrimary)
        {
            foreach (var image in _images)
                image.SetPrimary(string.Equals(image.FileKey, @event.FileKey, StringComparison.OrdinalIgnoreCase));
        }

        if (existing is null)
        {
            _images.Add(VariantImage.Create(
                BusinessKey.Value,
                @event.FileKey,
                @event.OriginalFileName,
                @event.ContentType,
                @event.OriginalUrl,
                @event.ThumbnailUrl,
                @event.DisplayOrder,
                @event.IsPrimary));
            return;
        }

        existing.Update(
            @event.OriginalFileName,
            @event.ContentType,
            @event.OriginalUrl,
            @event.ThumbnailUrl,
            @event.DisplayOrder,
            @event.IsPrimary);
    }

    private void On(ProductVariantImageRemovedEvent @event)
    {
        var existing = _images.FirstOrDefault(x => string.Equals(x.FileKey, @event.FileKey, StringComparison.OrdinalIgnoreCase));
        if (existing is null)
            return;

        var wasPrimary = existing.IsPrimary;
        _images.Remove(existing);

        if (wasPrimary && _images.Count > 0)
            _images.OrderBy(x => x.DisplayOrder).First().SetPrimary(true);
    }

    private void On(ProductVariantTagUpsertedEvent @event)
    {
        var existing = _tags.FirstOrDefault(x => x.BusinessKey.Value == @event.VariantTagBusinessKey);
        if (existing is null)
        {
            _tags.Add(VariantTag.Create(
                BusinessKey.Value,
                @event.VariantTagBusinessKey,
                @event.TagBusinessKey,
                @event.TagName,
                @event.TagColor,
                @event.DisplayOrder));
            return;
        }

        existing.Update(@event.TagBusinessKey, @event.TagName, @event.TagColor, @event.DisplayOrder);
    }

    private void On(ProductVariantTagRemovedEvent @event)
    {
        var existing = _tags.FirstOrDefault(x => x.BusinessKey.Value == @event.VariantTagBusinessKey);
        if (existing is null)
            return;

        _tags.Remove(existing);
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

    private void SyncImages(IReadOnlyCollection<ProductVariantImageSnapshot> images)
    {
        _images.Clear();

        if (images is null || images.Count == 0)
            return;

        foreach (var snapshot in images.OrderBy(x => x.DisplayOrder))
        {
            if (string.IsNullOrWhiteSpace(snapshot.FileKey))
                continue;

            _images.Add(VariantImage.Create(
                BusinessKey.Value,
                snapshot.FileKey,
                snapshot.OriginalFileName,
                snapshot.ContentType,
                snapshot.OriginalUrl,
                snapshot.ThumbnailUrl,
                snapshot.DisplayOrder,
                snapshot.IsPrimary));
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

    private static IReadOnlyCollection<ProductVariantImageSnapshot> SnapshotImages(IEnumerable<VariantImage> images)
    {
        return images
            .OrderBy(x => x.DisplayOrder)
            .Select(x => new ProductVariantImageSnapshot(
                x.FileKey,
                x.OriginalFileName,
                x.ContentType,
                x.OriginalUrl,
                x.ThumbnailUrl,
                x.DisplayOrder,
                x.IsPrimary))
            .ToList();
    }

}
