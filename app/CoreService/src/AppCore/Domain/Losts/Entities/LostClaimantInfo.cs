namespace Insurance.AppCore.Domain.Parvandes.Entities;

using Insurance.AppCore.Domain.BaseData.CarSystemAndTips.Enums;
using Insurance.AppCore.Domain.BaseData.VehiclePlates.Enums;
using Insurance.AppCore.Domain.BaseData.VehiclePlates.ValueObjects;
using Insurance.AppCore.Domain.Losts.Enums;
using OysterFx.AppCore.Domain.Aggregates;
using OysterFx.AppCore.Domain.ValueObjects;
using System;

public class LostClaimantInfo : Aggregate
{

    public BusinessKey LostBusinessKey { get; private set; }
    /// <summary>
    /// بیمه گر
    /// </summary>
    public BusinessKey InsuranceCompanyBusinessKey { get; private set; }
    /// <summary>
    /// شعبه بیمه گر
    /// </summary>
    public BusinessKey InsuranceCompanyBranch { get; private set; }
    /// <summary>
    /// اطلاعات مالک/راننده
    /// </summary>
    public PolicyHolderGender Gender { get; private set; }
    public string? PolicyHolderFullName { get; private set; }

    public string? FullName { get; private set; }
    public string? PhoneNumber { get; private set; }
    public string? MobileNumber { get; private set; }
    /// <summary>
    /// نوع خودرو
    /// </summary>
    public GroupHeadType GroupHeadType { get; private set; }
    /// <summary>
    /// سیستم و تیپ خودرو
    /// </summary>
    public Guid CarSystemAndTipBusinessKey { get; private set; }
    /// <summary>
    /// نوع پلاک
    /// </summary>
    public VehiclePlateType VehiclePlateType { get; private set; }
    /// <summary>
    /// نمونه پلاک
    /// </summary>
    public PlateModelType PlateModelType { get; private set; }
    /// <summary>
    /// طرح پلاک
    /// </summary>
    public VehiclePlateDesign VehiclePlateDesign { get; private set; }
    /// <summary>
    /// پلاک
    /// </summary>
    public VehiclePlate VehiclePlate { get; private set; }
    private LostClaimantInfo() { }
    private LostClaimantInfo(
        Guid insuranceCompanyBusinessKey,
        Guid insuranceCompanyBranch,
        PolicyHolderGender gender,
        string? policyHolderFullName,
        string? fullName,
        string? phoneNumber,
        string? mobileNumber,
        GroupHeadType groupHeadType,
        Guid carSystemAndTipBusinessKey,
        VehiclePlateType vehiclePlateType,
        PlateModelType plateModelType,
        VehiclePlateDesign vehiclePlateDesign,
        VehiclePlate vehiclePlate)
    {
        InsuranceCompanyBusinessKey = insuranceCompanyBusinessKey;
        InsuranceCompanyBranch = insuranceCompanyBranch;
        Gender = gender;
        PolicyHolderFullName = policyHolderFullName;
        FullName = fullName;
        PhoneNumber = phoneNumber;
        MobileNumber = mobileNumber;
        GroupHeadType = groupHeadType;
        CarSystemAndTipBusinessKey = carSystemAndTipBusinessKey;
        VehiclePlateType = vehiclePlateType;
        PlateModelType = plateModelType;
        VehiclePlateDesign = vehiclePlateDesign;
        VehiclePlate = vehiclePlate;
    }

    public static LostClaimantInfo CreateClaimantInfo(
        Guid insuranceCompanyBusinessKey,
        Guid insuranceCompanyBranch,
        PolicyHolderGender gender,
        string? policyHolderFullName,
        string? fullName,
        string? phoneNumber,
        string? mobileNumber,
        GroupHeadType groupHeadType,
        Guid carSystemAndTipBusinessKey,
        VehiclePlateType vehiclePlateType,
        PlateModelType plateModelType,
        VehiclePlateDesign vehiclePlateDesign,
        VehiclePlate vehiclePlate) => new(
            insuranceCompanyBusinessKey,
            insuranceCompanyBranch,
            gender,
            policyHolderFullName,
            fullName,
            phoneNumber,
            mobileNumber,
            groupHeadType,
            carSystemAndTipBusinessKey,
            vehiclePlateType,
            plateModelType,
            vehiclePlateDesign,
            vehiclePlate);
}