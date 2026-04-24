namespace Insurance.AppCore.Domain.BaseData.Bimehgars.Entities;

/// <summary>
/// بیمه‌گر
/// </summary>
public sealed class InsuranceCompany
{
    /// <summary>
    /// نام
    /// </summary>
    public string? Name { get; private set; }

    /// <summary>
    /// کد (فناوران)
    /// </summary>
    public string? FanavaranCode { get; private set; }

    /// <summary>
    /// طرف قرارداد ما است
    /// </summary>
    public bool IsContractualParty { get; private set; }

    public string? CreatedByUsername { get; private set; }
    public int CreatedById { get; private set; }
    public DateTime CreatedAt { get; private set; }

    // Navigation property
    public List<InsuranceCompanyBranch> Branches { get; private set; } = new();
    public InsuranceCompanySettings? Settings { get; private set; }
}
