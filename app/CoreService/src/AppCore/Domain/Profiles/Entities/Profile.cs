namespace Insurance.AppCore.Domain.Profiles.Entities;

using Insurance.AppCore.Domain.Profiles.Enums;
/// <summary>
/// پروفایل کاربران سیستم
/// </summary>
/// <summary>
/// پروفایل کاربران سیستم
/// </summary>
public sealed class Profile
{
    /// <summary>
    /// نوع شخص
    /// </summary>
    public PersonType PersonType { get; private set; }

    /// <summary>
    /// جنسیت
    /// </summary>
    public Gender Gender { get; private set; }

    public string? FirstName { get; private set; }
    public string? LastName { get; private set; }
    public string? FatherName { get; private set; }

    /// <summary>
    /// کدملی
    /// </summary>
    public string? NationalId { get; private set; }

    /// <summary>
    /// شماره شناسنامه
    /// </summary>
    public string? NationalCode { get; private set; }

    /// <summary>
    /// سریال شناسنامه
    /// </summary>
    public string? SerialNumber { get; private set; }

    public string? BirthDate { get; private set; }

    /// <summary>
    /// استان محل تولد
    /// </summary>
    public string? BirthProvinceName { get; private set; }

    /// <summary>
    /// شهر محل تولد
    /// </summary>
    public string? BirthCityName { get; private set; }

    /// <summary>
    /// وضعیت نظام وظیفه
    /// </summary>
    public MilitaryServiceStatus MilitaryServiceStatus { get; private set; }

    /// <summary>
    /// وضعیت تحصیلات
    /// </summary>
    public EducationLevel EducationStatus { get; private set; }

    /// <summary>
    /// رشته تحصیلی
    /// </summary>
    public string? FieldOfStudy { get; private set; }

    /// <summary>
    /// نام دانشگاه
    /// </summary>
    public string? UniversityName { get; private set; }

    /// <summary>
    /// تاریخ فارغ التحصیلی
    /// </summary>
    public DateTime? GraduationDate { get; private set; }

    /// <summary>
    /// وضعیت تاهل
    /// </summary>
    public MaritalStatus MaritalStatus { get; private set; }

    /// <summary>
    /// تاریخ ازدواج
    /// </summary>
    public DateTime? MarriageDate { get; private set; }

    /// <summary>
    /// تعداد بچه
    /// </summary>
    public long NumberOfChildren { get; private set; }

    /// <summary>
    /// تابعیت
    /// </summary>
    public string? Nationality { get; private set; }

    public ProfileAddress Address { get; private set; }

    /// <summary>
    /// اطلاعات محل کار
    /// </summary>
    public ProfileWorkplaceInfo WorkplaceInfo { get; private set; }

    /// <summary>
    /// اطلاعات حساب بانکی
    /// </summary>
    public ProfileBankAccountInfo BankAccountInfo { get; private set; }

    /// <summary>
    /// اطلاعات سازمانی
    /// </summary>
    public ProfileOrganizationInfo OrganizationInfo { get; private set; }

    public string? Description { get; private set; }

    /// <summary>
    /// تصویر امضا
    /// </summary>
    public string? SignatureImageAsBase64 { get; private set; }

    /// <summary>
    /// تصویر کارت ملی
    /// </summary>
    public string? NationalCardImageAsBase64 { get; private set; }

    /// <summary>
    /// تصویر شناسنامه
    /// </summary>
    public string? BirthCertificateImageAsBase64 { get; private set; }

    /// <summary>
    /// تصویر کارت پایان خدمت
    /// </summary>
    public string? MilitaryServiceCardImageAsBase64 { get; private set; }

    /// <summary>
    /// تصویر مدرک تحصیلی
    /// </summary>
    public string? DiplomaImageAsBase64 { get; private set; }

    /// <summary>
    /// تصویر رزومه
    /// </summary>
    public string? ResumeImageAsBase64 { get; private set; }

    private Profile()
    {
    }
}