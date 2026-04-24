namespace Insurance.UserService.AppCore.Domain.Tenants.Entities;

using System;


public record TenantId
{
    public string Value { get; private set; }

    private TenantId(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("TenantId cannot be empty", nameof(value));

        Value = value;
    }

    public static TenantId FromString(string value) => new(value);
    public static TenantId New() => new($"TENANT_{Guid.NewGuid():N}");

    public override string ToString() => Value;
}