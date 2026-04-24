namespace Insurance.AppCore.Domain.BaseData.Bimehgars.Entities;

using Insurance.AppCore.Domain.BaseData.Bimehgars.Enums;

/// <summary>
/// تنظیمات بیمه‌گر
/// </summary>
public sealed class InsuranceCompanySettings
{
    public InsuranceSystemType SystemType { get; private set; }
    public string? WebServiceUrl { get; private set; }
    public string? WebServiceUsername { get; private set; }
    public string? WebServicePassword { get; private set; }
    public string? WebServiceConfigurationJson { get; private set; }

    /// <summary>
    /// نام کاربری (درخواست)
    /// </summary>
    public string? RequestUsername { get; private set; }

    /// <summary>
    /// کد کارشناس پرونده
    /// </summary>
    public string? FileExpertCode { get; private set; }

    /// <summary>
    /// کد ارزیاب بدنه
    /// </summary>
    public string? BodyAssessorCode { get; private set; }

    /// <summary>
    /// کد کارشناس پرونده ثالث مالی
    /// </summary>
    public string? ThirdPartyFinancialFileExpertCode { get; private set; }

    /// <summary>
    /// کد ارزیاب ثالث مالی
    /// </summary>
    public string? ThirdPartyFinancialAssessorCode { get; private set; }

    /// <summary>
    /// نام دیتابیس اطلاعات پایه
    /// </summary>
    public string? BaseDataDatabaseName { get; private set; }

    // Navigation property
    public long InsuranceCompanyId { get; private set; }
    public InsuranceCompany? InsuranceCompany { get; private set; }
}