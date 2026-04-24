using System.ComponentModel;

namespace Insurance.AppCore.Domain.BaseData.CarSystemAndTips.Enums;

/// <summary>
/// نوع پلاک (قدیم/لیزری)
/// </summary>
public enum PlateModelType : byte
{
    [Description("پلاک قدیم")]
    Old = 1,

    [Description("پلاک لیزری جدید")]
    NewLaser = 2
}