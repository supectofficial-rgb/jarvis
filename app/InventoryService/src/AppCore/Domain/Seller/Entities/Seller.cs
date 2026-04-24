namespace Insurance.InventoryService.AppCore.Domain.Seller.Entities;

using OysterFx.AppCore.Domain.Aggregates;

public sealed class Seller : AggregateRoot
{
    public string Code { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public bool IsSystemOwner { get; private set; }
    public bool IsActive { get; private set; }

    private Seller()
    {
    }

    private Seller(string code, string name, bool isSystemOwner)
    {
        Code = NormalizeRequired(code, nameof(code));
        Name = NormalizeRequired(name, nameof(name));
        IsSystemOwner = isSystemOwner;
        IsActive = true;
    }

    public static Seller Create(string code, string name, bool isSystemOwner = false)
        => new(code, name, isSystemOwner);

    public void Rename(string name) => Name = NormalizeRequired(name, nameof(name));

    public void ChangeCode(string code) => Code = NormalizeRequired(code, nameof(code));

    public void SetSystemOwner(bool isSystemOwner) => IsSystemOwner = isSystemOwner;

    public void Activate() => IsActive = true;

    public void Deactivate() => IsActive = false;

    private static string NormalizeRequired(string value, string paramName)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Value is required.", paramName);

        return value.Trim();
    }
}
