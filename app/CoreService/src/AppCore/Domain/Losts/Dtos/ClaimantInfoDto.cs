namespace Insurance.AppCore.Domain.Losts.Dtos;

using Insurance.AppCore.Domain.BaseData.CarSystemAndTips.Enums;
using Insurance.AppCore.Domain.BaseData.VehiclePlates.Enums;
using Insurance.AppCore.Domain.BaseData.VehiclePlates.ValueObjects;
using System;

/// <summary>
/// مشخصات زیان دیده
/// </summary>
public class ClaimantInfoDto
{
    /// <summary>
    /// بیمه گر
    /// </summary>
    public Guid InsuranceCompanyBusinessKey { get; set; }
    /// <summary>
    /// شعبه بیمه گر
    /// </summary>
    public Guid InsuranceCompanyBranch { get; set; }
    /// <summary>
    /// اطلاعات مالک/راننده
    /// </summary>
    public PolicyHolderInfo PolicyHolderInfo { get; set; }
    public string? FullName { get; set; }
    public string? PhoneNumber { get; set; }
    public string? MobileNumber { get; set; }
    /// <summary>
    /// نوع خودرو
    /// </summary>
    public GroupHeadType GroupHeadType { get; set; }
    /// <summary>
    /// سیستم و تیپ خودرو
    /// </summary>
    public Guid CarSystemAndTipBusinessKey { get; set; }
    /// <summary>
    /// نوع پلاک
    /// </summary>
    public VehiclePlateType VehiclePlateType { get; set; }
    /// <summary>
    /// نمونه پلاک
    /// </summary>
    public PlateModelType PlateModelType { get; set; }
    /// <summary>
    /// طرح پلاک
    /// </summary>
    public VehiclePlateDesign VehiclePlateDesign { get; set; }
    /// <summary>
    /// پلاک
    /// </summary>
    public VehiclePlate VehiclePlate { get; set; }

}