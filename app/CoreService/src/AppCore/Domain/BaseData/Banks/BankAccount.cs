namespace Insurance.AppCore.Domain.BaseData.Banks;

using System;

/// <summary>
/// شماره حساب بانکی
/// </summary>
public sealed class BankAccount
{
    public long BankBranchId { get; private set; }
    public string? AccountNumber { get; private set; }
    public string? Iban { get; private set; }
    public string? AccountHolderName { get; private set; }
    public string? CardNumber { get; private set; }
    public bool IsActive { get; private set; }
    public int CreatedById { get; private set; }
    public string? CreatedByUsername { get; private set; }
    public DateTime CreatedAt { get; private set; }

    // Navigation properties
    public BankBranch? BankBranch { get; private set; }
    public List<InsuranceCompanyBankAccount>? InsuranceCompanyAccounts { get; private set; }
}
