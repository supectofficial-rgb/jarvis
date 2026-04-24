namespace Insurance.InventoryService.AppCore.Domain.Catalog.Entities;

using Insurance.InventoryService.AppCore.Domain.Catalog.Events;
using OysterFx.AppCore.Domain.Aggregates;

public sealed class UnitOfMeasure : AggregateRoot
{
    public string Code { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public int Precision { get; private set; }
    public bool IsActive { get; private set; }

    private UnitOfMeasure()
    {
    }

    public static UnitOfMeasure Create(string code, string name, int precision)
    {
        if (precision < 0)
            throw new ArgumentOutOfRangeException(nameof(precision));

        var uom = new UnitOfMeasure();
        uom.Apply(new UnitOfMeasureCreatedEvent(
            uom.BusinessKey,
            NormalizeRequired(code, nameof(code)),
            NormalizeRequired(name, nameof(name)),
            precision,
            true));

        return uom;
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

    public void ChangePrecision(int precision)
    {
        if (precision < 0)
            throw new ArgumentOutOfRangeException(nameof(precision));

        if (Precision == precision)
            return;

        RaiseUpdatedEvent(precision: precision);
    }

    public void Activate()
    {
        if (IsActive)
            return;

        Apply(new UnitOfMeasureActivationChangedEvent(BusinessKey, true));
    }

    public void Deactivate()
    {
        if (!IsActive)
            return;

        Apply(new UnitOfMeasureActivationChangedEvent(BusinessKey, false));
    }

    private static string NormalizeRequired(string value, string paramName)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Value is required.", paramName);

        return value.Trim();
    }

    private void RaiseUpdatedEvent(string? code = null, string? name = null, int? precision = null, bool? isActive = null)
    {
        Apply(new UnitOfMeasureUpdatedEvent(
            BusinessKey,
            code ?? Code,
            name ?? Name,
            precision ?? Precision,
            isActive ?? IsActive));
    }

    private void On(UnitOfMeasureCreatedEvent @event)
    {
        Code = @event.Code;
        Name = @event.Name;
        Precision = @event.Precision;
        IsActive = @event.IsActive;
    }

    private void On(UnitOfMeasureUpdatedEvent @event)
    {
        Code = @event.Code;
        Name = @event.Name;
        Precision = @event.Precision;
        IsActive = @event.IsActive;
    }

    private void On(UnitOfMeasureActivationChangedEvent @event)
    {
        IsActive = @event.IsActive;
    }
}
