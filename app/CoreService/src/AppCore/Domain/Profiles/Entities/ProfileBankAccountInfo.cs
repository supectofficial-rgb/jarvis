namespace Insurance.AppCore.Domain.Profiles.Entities;

/// <summary>
/// اطلاعات حساب بانکی
/// </summary>
public sealed class ProfileBankAccountInfo
{
    /// <summary>
    /// نام بانک
    /// </summary>
    public string? BankName { get; private set; }

    /// <summary>
    /// شماره حساب
    /// </summary>
    public string? AccountNumber { get; private set; }

    /// <summary>
    /// شماره کارت
    /// </summary>
    public string? CardNumber { get; private set; }

    /// <summary>
    /// شماره شبا
    /// </summary>
    public string? ShebaNumber { get; private set; }

    private ProfileBankAccountInfo() { }
}