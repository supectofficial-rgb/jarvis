namespace Insurance.InventoryService.AppCore.Domain.Warehouse.Entities;

using OysterFx.AppCore.Domain.Aggregates;

public sealed class Warehouse : AggregateRoot
{
    public string Code { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public bool IsActive { get; private set; }

    private Warehouse()
    {
    }

    private Warehouse(string code, string name)
    {
        Code = NormalizeRequired(code, nameof(code));
        Name = NormalizeRequired(name, nameof(name));
        IsActive = true;
    }

    public static Warehouse Create(string code, string name) => new(code, name);

    public void Rename(string name) => Name = NormalizeRequired(name, nameof(name));

    public void ChangeCode(string code) => Code = NormalizeRequired(code, nameof(code));

    public void Activate() => IsActive = true;

    public void Deactivate() => IsActive = false;

    private static string NormalizeRequired(string value, string paramName)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Value is required.", paramName);

        return value.Trim();
    }
}
