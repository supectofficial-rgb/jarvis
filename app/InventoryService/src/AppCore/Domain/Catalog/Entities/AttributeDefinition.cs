namespace Insurance.InventoryService.AppCore.Domain.Catalog.Entities;

using Insurance.InventoryService.AppCore.Domain.Catalog.Events;
using OysterFx.AppCore.Domain.Aggregates;

public sealed class AttributeDefinition : AggregateRoot
{
    private readonly List<AttributeOption> _options = new();

    public string Code { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public AttributeDataType DataType { get; private set; }
    public AttributeScope Scope { get; private set; }
    public bool IsActive { get; private set; }
    public IReadOnlyCollection<AttributeOption> Options => _options.AsReadOnly();

    private AttributeDefinition()
    {
    }

    public static AttributeDefinition Create(string code, string name, AttributeDataType dataType, AttributeScope scope)
    {
        var attributeDefinition = new AttributeDefinition();

        attributeDefinition.Apply(new AttributeDefinitionCreatedEvent(
            attributeDefinition.BusinessKey,
            NormalizeRequired(code, nameof(code)),
            NormalizeRequired(name, nameof(name)),
            dataType,
            scope,
            true,
            Array.Empty<AttributeDefinitionOptionSnapshot>()));

        return attributeDefinition;
    }

    public void Rename(string name)
    {
        var normalized = NormalizeRequired(name, nameof(name));
        if (string.Equals(Name, normalized, StringComparison.Ordinal))
            return;

        RaiseUpdatedEvent(name: normalized);
    }

    public void ChangeCode(string code)
    {
        var normalized = NormalizeRequired(code, nameof(code));
        if (string.Equals(Code, normalized, StringComparison.Ordinal))
            return;

        RaiseUpdatedEvent(code: normalized);
    }

    public void ChangeScope(AttributeScope scope)
    {
        if (Scope == scope)
            return;

        RaiseUpdatedEvent(scope: scope);
    }

    public void ChangeDataType(AttributeDataType dataType)
    {
        if (DataType == dataType)
            return;

        RaiseUpdatedEvent(dataType: dataType);
    }

    public void Activate()
    {
        if (IsActive)
            return;

        Apply(new AttributeDefinitionActivationChangedEvent(BusinessKey, true));
    }

    public void Deactivate()
    {
        if (!IsActive)
            return;

        Apply(new AttributeDefinitionActivationChangedEvent(BusinessKey, false));
    }

    public AttributeOption AddOption(string value, int displayOrder = 0)
        => AddOption(value, value, displayOrder);

    public AttributeOption AddOption(string name, string value, int displayOrder = 0)
    {
        var normalizedName = NormalizeRequired(name, nameof(name));
        var normalized = NormalizeRequired(value, nameof(value));
        var existing = _options.FirstOrDefault(x =>
            string.Equals(x.Value, normalized, StringComparison.OrdinalIgnoreCase)
            || string.Equals(x.Name, normalizedName, StringComparison.OrdinalIgnoreCase));

        if (existing is not null
            && existing.DisplayOrder == displayOrder
            && existing.IsActive
            && string.Equals(existing.Value, normalized, StringComparison.OrdinalIgnoreCase)
            && string.Equals(existing.Name, normalizedName, StringComparison.OrdinalIgnoreCase))
            return existing;

        var optionBusinessKey = existing?.BusinessKey.Value ?? Guid.NewGuid();

        Apply(new AttributeDefinitionOptionAddedEvent(BusinessKey, optionBusinessKey, normalized, displayOrder, true, normalizedName));

        return _options.First(x => x.BusinessKey.Value == optionBusinessKey);
    }

    public AttributeOption UpdateOption(Guid optionBusinessKey, string value, int displayOrder)
        => UpdateOption(optionBusinessKey, value, value, displayOrder);

    public AttributeOption UpdateOption(Guid optionBusinessKey, string name, string value, int displayOrder)
    {
        if (optionBusinessKey == Guid.Empty)
            throw new ArgumentException("OptionBusinessKey is required.", nameof(optionBusinessKey));

        var normalizedName = NormalizeRequired(name, nameof(name));
        var normalized = NormalizeRequired(value, nameof(value));
        var option = _options.FirstOrDefault(x => x.BusinessKey.Value == optionBusinessKey);
        if (option is null)
            throw new InvalidOperationException("Attribute option was not found.");

        Apply(new AttributeDefinitionOptionUpdatedEvent(BusinessKey, optionBusinessKey, normalized, displayOrder, normalizedName));

        return _options.First(x => x.BusinessKey.Value == optionBusinessKey);
    }

    public void SetOptionActive(Guid optionBusinessKey, bool isActive)
    {
        if (optionBusinessKey == Guid.Empty)
            throw new ArgumentException("OptionBusinessKey is required.", nameof(optionBusinessKey));

        var option = _options.FirstOrDefault(x => x.BusinessKey.Value == optionBusinessKey);
        if (option is null)
            throw new InvalidOperationException("Attribute option was not found.");

        if (option.IsActive == isActive)
            return;

        Apply(new AttributeDefinitionOptionActivationChangedEvent(BusinessKey, optionBusinessKey, isActive));
    }

    public void RemoveOption(string value)
    {
        var normalized = NormalizeRequired(value, nameof(value));
        var existing = _options.FirstOrDefault(x =>
            string.Equals(x.Value, normalized, StringComparison.OrdinalIgnoreCase)
            || string.Equals(x.Name, normalized, StringComparison.OrdinalIgnoreCase));
        if (existing is null)
            return;

        Apply(new AttributeDefinitionOptionRemovedEvent(BusinessKey, existing.BusinessKey.Value, existing.Value));
    }

    public void RemoveOption(Guid optionBusinessKey)
    {
        if (optionBusinessKey == Guid.Empty)
            throw new ArgumentException("OptionBusinessKey is required.", nameof(optionBusinessKey));

        var existing = _options.FirstOrDefault(x => x.BusinessKey.Value == optionBusinessKey);
        if (existing is null)
            return;

        Apply(new AttributeDefinitionOptionRemovedEvent(BusinessKey, existing.BusinessKey.Value, existing.Value));
    }

    private static string NormalizeRequired(string value, string paramName)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Value is required.", paramName);

        return value.Trim();
    }

    private void RaiseUpdatedEvent(
        string? code = null,
        string? name = null,
        AttributeDataType? dataType = null,
        AttributeScope? scope = null,
        bool? isActive = null)
    {
        Apply(new AttributeDefinitionUpdatedEvent(
            BusinessKey,
            code ?? Code,
            name ?? Name,
            dataType ?? DataType,
            scope ?? Scope,
            isActive ?? IsActive,
            SnapshotOptions(_options)));
    }

    private void On(AttributeDefinitionCreatedEvent @event)
    {
        Code = @event.Code;
        Name = @event.Name;
        DataType = @event.DataType;
        Scope = @event.Scope;
        IsActive = @event.IsActive;
        SyncOptions(@event.Options);
    }

    private void On(AttributeDefinitionUpdatedEvent @event)
    {
        Code = @event.Code;
        Name = @event.Name;
        DataType = @event.DataType;
        Scope = @event.Scope;
        IsActive = @event.IsActive;
        SyncOptions(@event.Options);
    }

    private void On(AttributeDefinitionActivationChangedEvent @event)
    {
        IsActive = @event.IsActive;
    }

    private void On(AttributeDefinitionOptionAddedEvent @event)
    {
        var normalizedName = NormalizeRequired(@event.Name, nameof(@event.Name));
        var normalized = NormalizeRequired(@event.Value, nameof(@event.Value));
        var existing = _options.FirstOrDefault(x => x.BusinessKey.Value == @event.OptionBusinessKey)
            ?? _options.FirstOrDefault(x =>
                string.Equals(x.Value, normalized, StringComparison.OrdinalIgnoreCase)
                || string.Equals(x.Name, normalizedName, StringComparison.OrdinalIgnoreCase));

        if (existing is null)
        {
            var option = AttributeOption.Create(BusinessKey.Value, @event.OptionBusinessKey, normalizedName, normalized, @event.DisplayOrder);
            if (!@event.IsActive)
                option.Deactivate();

            _options.Add(option);
            return;
        }

        existing.Rename(normalizedName);
        existing.ChangeValue(normalized);
        existing.ChangeDisplayOrder(@event.DisplayOrder);
        if (@event.IsActive)
            existing.Activate();
        else
            existing.Deactivate();
    }

    private void On(AttributeDefinitionOptionUpdatedEvent @event)
    {
        var option = _options.FirstOrDefault(x => x.BusinessKey.Value == @event.OptionBusinessKey);
        if (option is null)
            return;

        option.Rename(@event.Name);
        option.ChangeValue(@event.Value);
        option.ChangeDisplayOrder(@event.DisplayOrder);
    }

    private void On(AttributeDefinitionOptionActivationChangedEvent @event)
    {
        var option = _options.FirstOrDefault(x => x.BusinessKey.Value == @event.OptionBusinessKey);
        if (option is null)
            return;

        if (@event.IsActive)
            option.Activate();
        else
            option.Deactivate();
    }

    private void On(AttributeDefinitionOptionRemovedEvent @event)
    {
        var existing = _options.FirstOrDefault(x => x.BusinessKey.Value == @event.OptionBusinessKey);
        if (existing is null && !string.IsNullOrWhiteSpace(@event.Value))
        {
            existing = _options.FirstOrDefault(x =>
                string.Equals(x.Value, @event.Value, StringComparison.OrdinalIgnoreCase)
                || string.Equals(x.Name, @event.Value, StringComparison.OrdinalIgnoreCase));
        }

        if (existing is null)
            return;

        _options.Remove(existing);
    }

    private void SyncOptions(IReadOnlyCollection<AttributeDefinitionOptionSnapshot> options)
    {
        _options.Clear();

        if (options is null || options.Count == 0)
            return;

        foreach (var snapshot in options)
        {
            if (string.IsNullOrWhiteSpace(snapshot.Value))
                continue;

            var option = AttributeOption.Create(
                BusinessKey.Value,
                snapshot.OptionBusinessKey,
                snapshot.Name,
                snapshot.Value.Trim(),
                snapshot.DisplayOrder);
            if (!snapshot.IsActive)
                option.Deactivate();

            _options.Add(option);
        }
    }

    private static IReadOnlyCollection<AttributeDefinitionOptionSnapshot> SnapshotOptions(IEnumerable<AttributeOption> options)
    {
        return options
            .Select(x => new AttributeDefinitionOptionSnapshot(x.BusinessKey.Value, x.Value, x.DisplayOrder, x.IsActive, x.Name))
            .ToList();
    }
}
