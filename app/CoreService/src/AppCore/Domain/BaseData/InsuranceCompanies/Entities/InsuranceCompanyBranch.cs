namespace Insurance.AppCore.Domain.BaseData.Bimehgars.Entities;

/// <summary>
/// شعبه بیمه‌گر
/// </summary>
public sealed class InsuranceCompanyBranch
{
    public long InsuranceCompanyId { get; private set; }
    public string? FanavaranCode { get; private set; }
    public string? Name { get; private set; }
    public string? City { get; private set; }
    public string? CreatedByUsername { get; private set; }
    public int CreatedById { get; private set; }
    public DateTime CreatedAt { get; private set; }

    // Navigation property
    public InsuranceCompany? InsuranceCompany { get; private set; }
}
