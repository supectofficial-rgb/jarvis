namespace Insurance.AppCore.Domain.BaseData.Banks;

using System;

public sealed class Bank
{
    public string? Name { get; private set; }
    public string? Code { get; private set; }
    public string? SwiftCode { get; private set; }
    public bool IsActive { get; private set; }
    public int CreatedById { get; private set; }
    public string? CreatedByUsername { get; private set; }
    public DateTime CreatedAt { get; private set; }

    // Navigation properties
    public List<BankBranch>? Branches { get; private set; }

    private Bank() { }

    public Bank(string name, string? code = null, string? swiftCode = null)
    {
        Name = name;
        Code = code;
        SwiftCode = swiftCode;
        IsActive = true;
        CreatedAt = DateTime.UtcNow;
    }
}