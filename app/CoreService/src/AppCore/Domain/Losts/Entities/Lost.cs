namespace Insurance.AppCore.Domain.Parvandes.Entities;

using Insurance.AppCore.Domain.Losts.Dtos;
using Insurance.AppCore.Domain.Losts.Enums;
using Insurance.AppCore.Domain.Losts.ValueObjects;
using OysterFx.AppCore.Domain.Aggregates;
using OysterFx.AppCore.Domain.ValueObjects;
using System;

/// <summary>
/// پرونده
/// </summary>
public sealed class Lost : AggregateRoot
{
    public LostClaimantInfo LostClaimantInfo { get; private set; }
    public BusinessKey ProvinceBusinessKey { get; private set; }
    public BusinessKey CityBusinessKey { get; private set; }

    /// <summary>
    /// ارزیاب
    /// </summary>
    public BusinessKey AppraiserBusinessKey { get; private set; }

    /// <summary>
    /// محل وقوع حادثه
    /// </summary>
    public Location AccidentLocation { get; private set; }
    /// <summary>
    /// تاریخ و زمان وقوع حادثه
    /// </summary>
    public DateTime? AccidentDateTime { get; private set; }

    /// <summary>
    /// شماره پرونده
    /// </summary>
    public string? LostNumber { get; private set; }

    /// <summary>
    /// بیمه گر
    /// </summary>
    public string? PolicyHolder { get; private set; }

    /// <summary>
    /// ش.پ بیمه گر
    /// </summary>
    public string? PolicyNumber { get; private set; }

    /// <summary>
    /// نوع پرونده
    /// </summary>
    public LostType? ClaimType { get; private set; }

    /// <summary>
    /// شعبه
    /// </summary>
    public string? Branch { get; private set; }

    /// <summary>
    /// شهر بازدید
    /// </summary>
    public string? InspectionCity { get; private set; }
    public string? InspectionAddress { get; private set; }

    /// <summary>
    /// تاریخ اعلام
    /// </summary>
    public DateTime? ReportDate { get; private set; }

    /// <summary>
    /// تاریخ ارسال
    /// </summary>
    public DateTime? SubmissionDate { get; private set; }

    /// <summary>
    /// وضعیت پرونده
    /// </summary>
    public LostStatus Status { get; private set; }

    private Lost() { }
    /// <summary>
    /// صدور کد بیمه گران طرف قرارداد ثالث مالی
    /// </summary>
    /// <param name="provinceBusinessKey"></param>
    /// <param name="cityBusinessKey"></param>
    /// <param name="accidentLocation"></param>
    /// <param name="accidentDateTime"></param>
    /// <param name="appraiserBusinessKey"></param>
    /// <param name="branch"></param>
    private Lost(
        BusinessKey provinceBusinessKey,
        BusinessKey cityBusinessKey,
        Location accidentLocation,
        DateTime? accidentDateTime,
        BusinessKey appraiserBusinessKey,
        string? branch)
    {
        ProvinceBusinessKey = provinceBusinessKey;
        CityBusinessKey = cityBusinessKey;
        AccidentLocation = accidentLocation;
        AccidentDateTime = accidentDateTime;
        AppraiserBusinessKey = appraiserBusinessKey;
        Branch = branch;
    }

    /// <summary>
    /// صدور کد بیمه گران طرف قرارداد ثالث بدنه
    /// </summary>
    private Lost(
        BusinessKey provinceBusinessKey,
        BusinessKey cityBusinessKey,
        string? inspectionAddress,
        Location accidentLocation,
        DateTime? accidentDateTime,
        BusinessKey appraiserBusinessKey,
        string? branch,
        ClaimantInfoDto claimantInfo)
    {
        ProvinceBusinessKey = provinceBusinessKey;
        CityBusinessKey = cityBusinessKey;
        InspectionAddress = inspectionAddress;
        AccidentLocation = accidentLocation;
        AccidentDateTime = accidentDateTime;
        AppraiserBusinessKey = appraiserBusinessKey;
        Branch = branch;
        LostClaimantInfo = LostClaimantInfo.CreateClaimantInfo(
            claimantInfo.InsuranceCompanyBusinessKey,
            claimantInfo.InsuranceCompanyBranch,
            claimantInfo.PolicyHolderInfo.Gender,
            claimantInfo.PolicyHolderInfo.FullName,
            claimantInfo.FullName,
            claimantInfo.PhoneNumber,
            claimantInfo.MobileNumber,
            claimantInfo.GroupHeadType,
            claimantInfo.CarSystemAndTipBusinessKey,
            claimantInfo.VehiclePlateType,
            claimantInfo.PlateModelType,
            claimantInfo.VehiclePlateDesign,
            claimantInfo.VehiclePlate);

    }
    /// <summary>
    /// صدور کد بیمه گران طرف قرارداد ثالث مالی
    /// </summary>
    /// <returns></returns>
    public static Lost CreateThirdPartyFinancialInsurerCode(
        BusinessKey provinceBusinessKey,
        BusinessKey cityBusinessKey,
        Location accidentLocation,
        DateTime? accidentDateTime,
        BusinessKey appraiserBusinessKey,
        string? branch) => new(provinceBusinessKey, cityBusinessKey, accidentLocation, accidentDateTime, appraiserBusinessKey, branch);

    /// <summary>
    /// صدور کد بیمه گران طرف قرارداد ثالث بدنه
    /// </summary>
    public static Lost CreateHullThirdPartyInsurerCode(
        BusinessKey provinceBusinessKey,
        BusinessKey cityBusinessKey,
        string? inspectionAddress,
        Location accidentLocation,
        DateTime? accidentDateTime,
        BusinessKey appraiserBusinessKey,
        string? branch,
        ClaimantInfoDto claimantInfo) => new(
            provinceBusinessKey,
            cityBusinessKey,
            inspectionAddress,
            accidentLocation,
            accidentDateTime,
            appraiserBusinessKey,
            branch,
            claimantInfo);
}