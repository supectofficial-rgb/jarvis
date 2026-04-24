using Insurance.AppCore.Domain.Profiles.Enums;

namespace Insurance.AppCore.Domain.Profiles.Entities;

/// <summary>
/// اطلاعات سازمانی
/// </summary>
public sealed class ProfileOrganizationInfo
{
    /// <summary>
    /// نوع ارتباط
    /// </summary>
    public OrganizationRelationType RelationType { get; private set; }

    /// <summary>
    /// کد پرسنلی
    /// </summary>
    public string? PersonnelCode { get; private set; }

    /// <summary>
    /// بخش کاری
    /// </summary>
    public WorkSectionType WorkSectionType { get; private set; }

    /// <summary>
    /// پست سازمانی
    /// </summary>
    public string? OrganizationPosition { get; private set; }

    /// <summary>
    /// تخصص
    /// </summary>
    public string? Expertise { get; private set; }

    /// <summary>
    /// کد حسابها در حسابداری
    /// </summary>
    public string? AccountingAccountCodes { get; private set; }

    /// <summary>
    /// میزان شارژ پیامکی
    /// </summary>
    public decimal? SmsCredit { get; private set; }
    private ProfileOrganizationInfo() { }
}