namespace Insurance.InventoryService.AppCore.Domain.Catalog.Entities;

using Insurance.InventoryService.AppCore.Domain.Catalog.Events;
using OysterFx.AppCore.Domain.Aggregates;

public sealed class Product : AggregateRoot
{
    private readonly List<ProductAttributeValue> _attributeValues = new();

    public Guid CategoryRef { get; private set; }
    public Guid CategorySchemaVersionRef { get; private set; }
    public string BaseSku { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public Guid DefaultUomRef { get; private set; }
    public Guid? TaxCategoryRef { get; private set; }
    public bool IsActive { get; private set; }
    public IReadOnlyCollection<ProductAttributeValue> AttributeValues => _attributeValues.AsReadOnly();

    private Product()
    {
    }

    public static Product Create(
        Guid categoryRef,
        Guid categorySchemaVersionRef,
        string baseSku,
        string name,
        Guid defaultUomRef,
        Guid? taxCategoryRef = null)
    {
        if (categorySchemaVersionRef == Guid.Empty)
            throw new ArgumentException("CategorySchemaVersionRef is required.", nameof(categorySchemaVersionRef));

        var product = new Product();

        product.Apply(new ProductCreatedEvent(
            product.BusinessKey,
            categoryRef,
            categorySchemaVersionRef,
            NormalizeRequired(baseSku, nameof(baseSku)),
            NormalizeRequired(name, nameof(name)),
            defaultUomRef,
            taxCategoryRef,
            true,
            Array.Empty<ProductAttributeValueSnapshot>()));

        return product;
    }

    public void Rename(string name)
    {
        var normalized = NormalizeRequired(name, nameof(name));
        if (string.Equals(Name, normalized, StringComparison.Ordinal))
            return;

        RaiseUpdatedEvent(name: normalized);
    }

    public void ChangeCategory(Guid categoryRef, Guid categorySchemaVersionRef)
    {
        if (categorySchemaVersionRef == Guid.Empty)
            throw new ArgumentException("CategorySchemaVersionRef is required.", nameof(categorySchemaVersionRef));

        if (CategoryRef == categoryRef && CategorySchemaVersionRef == categorySchemaVersionRef)
            return;

        Apply(new ProductCategoryChangedEvent(
            BusinessKey,
            CategoryRef,
            CategorySchemaVersionRef,
            categoryRef,
            categorySchemaVersionRef));

        RaiseUpdatedEvent(categoryRef: categoryRef, categorySchemaVersionRef: categorySchemaVersionRef);
    }

    public void ChangeDefaultUom(Guid defaultUomRef)
    {
        if (DefaultUomRef == defaultUomRef)
            return;

        RaiseUpdatedEvent(defaultUomRef: defaultUomRef);
    }

    public void ChangeTaxCategory(Guid? taxCategoryRef)
    {
        if (TaxCategoryRef == taxCategoryRef)
            return;

        RaiseUpdatedEvent(taxCategoryRef: taxCategoryRef, updateTaxCategoryRef: true);
    }

    public void Activate()
    {
        if (IsActive)
            return;

        Apply(new ProductActivationChangedEvent(BusinessKey, true));
    }

    public void Deactivate()
    {
        if (!IsActive)
            return;

        Apply(new ProductActivationChangedEvent(BusinessKey, false));
    }

    public ProductAttributeValue SetAttributeValue(Guid attributeRef, string? value, Guid? optionRef = null)
    {
        if (attributeRef == Guid.Empty)
            throw new ArgumentException("AttributeRef is required.", nameof(attributeRef));

        Apply(new ProductAttributeValueUpsertedEvent(BusinessKey, attributeRef, value, optionRef));

        return _attributeValues.First(x => x.AttributeRef == attributeRef);
    }

    public void RemoveAttributeValue(Guid attributeRef)
    {
        var existing = _attributeValues.FirstOrDefault(x => x.AttributeRef == attributeRef);
        if (existing is null)
            return;

        Apply(new ProductAttributeValueRemovedEvent(BusinessKey, attributeRef));
    }

    private static string NormalizeRequired(string value, string paramName)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Value is required.", paramName);

        return value.Trim();
    }

    private void RaiseUpdatedEvent(
        Guid? categoryRef = null,
        Guid? categorySchemaVersionRef = null,
        string? name = null,
        Guid? defaultUomRef = null,
        Guid? taxCategoryRef = null,
        bool updateTaxCategoryRef = false,
        bool? isActive = null)
    {
        var nextTaxCategoryRef = updateTaxCategoryRef ? taxCategoryRef : TaxCategoryRef;

        Apply(new ProductUpdatedEvent(
            BusinessKey,
            categoryRef ?? CategoryRef,
            categorySchemaVersionRef ?? CategorySchemaVersionRef,
            BaseSku,
            name ?? Name,
            defaultUomRef ?? DefaultUomRef,
            nextTaxCategoryRef,
            isActive ?? IsActive,
            SnapshotAttributeValues(_attributeValues)));
    }

    private void On(ProductCreatedEvent @event)
    {
        CategoryRef = @event.CategoryRef;
        CategorySchemaVersionRef = @event.CategorySchemaVersionRef;
        BaseSku = @event.BaseSku;
        Name = @event.Name;
        DefaultUomRef = @event.DefaultUomRef;
        TaxCategoryRef = @event.TaxCategoryRef;
        IsActive = @event.IsActive;
        SyncAttributeValues(@event.AttributeValues);
    }

    private void On(ProductUpdatedEvent @event)
    {
        CategoryRef = @event.CategoryRef;
        CategorySchemaVersionRef = @event.CategorySchemaVersionRef;
        BaseSku = @event.BaseSku;
        Name = @event.Name;
        DefaultUomRef = @event.DefaultUomRef;
        TaxCategoryRef = @event.TaxCategoryRef;
        IsActive = @event.IsActive;
        SyncAttributeValues(@event.AttributeValues);
    }

    private void On(ProductCategoryChangedEvent @event)
    {
        CategoryRef = @event.CategoryRef;
        CategorySchemaVersionRef = @event.CategorySchemaVersionRef;
    }

    private void On(ProductActivationChangedEvent @event)
    {
        IsActive = @event.IsActive;
    }

    private void On(ProductAttributeValueUpsertedEvent @event)
    {
        if (@event.AttributeRef == Guid.Empty)
            return;

        var existing = _attributeValues.FirstOrDefault(x => x.AttributeRef == @event.AttributeRef);
        if (existing is null)
        {
            _attributeValues.Add(ProductAttributeValue.Create(BusinessKey.Value, @event.AttributeRef, @event.Value, @event.OptionRef));
            return;
        }

        existing.Update(@event.Value, @event.OptionRef);
    }

    private void On(ProductAttributeValueRemovedEvent @event)
    {
        var existing = _attributeValues.FirstOrDefault(x => x.AttributeRef == @event.AttributeRef);
        if (existing is null)
            return;

        _attributeValues.Remove(existing);
    }

    private void SyncAttributeValues(IReadOnlyCollection<ProductAttributeValueSnapshot> values)
    {
        _attributeValues.Clear();

        if (values is null || values.Count == 0)
            return;

        foreach (var snapshot in values)
        {
            if (snapshot.AttributeRef == Guid.Empty)
                continue;

            _attributeValues.Add(ProductAttributeValue.Create(BusinessKey.Value, snapshot.AttributeRef, snapshot.Value, snapshot.OptionRef));
        }
    }

    private static IReadOnlyCollection<ProductAttributeValueSnapshot> SnapshotAttributeValues(IEnumerable<ProductAttributeValue> values)
    {
        return values
            .Select(x => new ProductAttributeValueSnapshot(x.AttributeRef, x.Value, x.OptionRef))
            .ToList();
    }
}
