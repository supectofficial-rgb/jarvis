namespace Insurance.AppCore.Domain.BaseData.Banks;

using Insurance.AppCore.Domain.BaseData.Bimehgars.Entities;

/// <summary>
/// نگاشت حساب بانکی به بیمه‌گر
/// </summary>
public sealed class InsuranceCompanyBankAccount
{
    public long InsuranceCompanyId { get; private set; }
    public long BankAccountId { get; private set; }
    public string? AccountTitleInInsuranceSystem { get; private set; }
    public bool IsDefault { get; private set; }

    // Navigation properties
    public InsuranceCompany? InsuranceCompany { get; private set; }
    public BankAccount? BankAccount { get; private set; }
}