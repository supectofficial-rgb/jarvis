using System.ComponentModel;

namespace Insurance.AppCore.Domain.BaseData.VehiclePlates.Enums;

/// <summary>
/// طرح پلاک وسیله نقلیه
/// </summary>
public enum VehiclePlateDesign
{
    [Description("طرح قدیم")]
    Old = 1,

    [Description("طرح کشوری")]
    National = 2,

    [Description("طرح لیزری")]
    Laser = 3,

    [Description("طرح بین المللی")]
    International = 4,

    [Description("فاقد پلاک")]
    WithoutPlate = 5,

    [Description("سایر")]
    Other = 6
}