namespace Insurance.AppCore.Domain.BaseData.CarColors.Entities;

using Insurance.AppCore.Domain.BaseData.Bimehgars.Entities;

/// <summary>
/// نگاشت رنگ خودرو به بیمه‌گرهای مختلف
/// </summary>
public sealed class InsuranceCompanyColorMapping
{
    public long CarColorId { get; private set; }
    public long InsuranceCompanyId { get; private set; }
    public string? InsuranceCompanyColorCode { get; private set; }
    public string? InsuranceCompanyColorName { get; private set; }

    // Navigation properties
    public CarColor? CarColor { get; private set; }
    public InsuranceCompany? InsuranceCompany { get; private set; }
}