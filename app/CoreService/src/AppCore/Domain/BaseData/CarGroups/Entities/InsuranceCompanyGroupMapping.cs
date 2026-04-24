namespace Insurance.AppCore.Domain.BaseData.CarGroups.Entities;

using Insurance.AppCore.Domain.BaseData.Bimehgars.Entities;

/// <summary>
/// نگاشت گروه خودرو به بیمه‌گرهای مختلف
/// </summary>
public sealed class InsuranceCompanyGroupMapping
{
    public long CarGroupId { get; private set; }
    public long InsuranceCompanyId { get; private set; }
    public string? InsuranceCompanyGroupCode { get; private set; }
    public string? InsuranceCompanyGroupName { get; private set; }

    // Navigation properties
    public CarGroup? CarGroup { get; private set; }
    public InsuranceCompany? InsuranceCompany { get; private set; }
}
