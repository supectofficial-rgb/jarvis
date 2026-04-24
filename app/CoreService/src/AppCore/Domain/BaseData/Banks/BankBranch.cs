namespace Insurance.AppCore.Domain.BaseData.Banks;

using System;

/// <summary>
/// شعبه بانک
/// </summary>
public sealed class BankBranch
{
    public long BankId { get; private set; }
    public string? BranchCode { get; private set; }
    public string? Name { get; private set; }
    public string? Address { get; private set; }
    public string? PhoneNumber { get; private set; }
    public string? City { get; private set; }
    public bool IsActive { get; private set; }
    public int CreatedById { get; private set; }
    public string? CreatedByUsername { get; private set; }
    public DateTime CreatedAt { get; private set; }

    // Navigation property
    public Bank? Bank { get; private set; }
}
