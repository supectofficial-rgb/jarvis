namespace Insurance.AppCore.Domain.BaseData.SystemSettings.Entities;

/// <summary>
/// تنظیمات برنامه - سیستم
/// </summary>
public sealed class SystemSetting
{
    /// <summary>
    /// عنوان برنامه
    /// </summary>
    public string? ApplicationTitle { get; private set; }

    /// <summary>
    /// متن پایین سایت
    /// </summary>
    public string? FooterText { get; private set; }

    /// <summary>
    /// متن تبریک روز تولد
    /// </summary>
    public string? BirthdayText { get; private set; }

    /// <summary>
    /// عکس تبریک تولد
    /// </summary>
    public string? BirthdayImageBase64 { get; private set; }

    /// <summary>
    /// متن تبریک سالگرد ازدواج
    /// </summary>
    public string? MarriageAnniversaryText { get; private set; }

    /// <summary>
    /// لوگو گزارشات  
    /// </summary>
    public string? ReportLogoBase64 { get; private set; }

    /// <summary>
    /// قالب نامه
    /// </summary>
    public string? LetterTemplateBase64 { get; private set; }

    /// <summary>
    /// ایمیل
    /// </summary>
    public string? Email { get; private set; }

    /// <summary>
    /// بستن پرونده ها به صورت خودکار بعد از )روز(
    /// </summary>
    public int AutoCloseFileAfterDays { get; private set; }

    /// <summary>
    /// تعداد مجاز درخواست پرونده برای گروه بررسی و کارشناسی تخصصی
    /// </summary>
    public int MaxFileCountPerGroup { get; private set; }

    public SystemSetting()
    {
    }
}