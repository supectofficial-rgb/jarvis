namespace Insurance.AppCore.Domain.BaseData.CarSystemAndTips.Enums;

/// <summary>
/// انواع خودرو
/// </summary>
public enum CarType : byte
{
    /// <summary>
    /// نامشخص
    /// </summary>
    Unknown = 0,

    /// <summary>
    /// خودروی سنگین
    /// </summary>
    HeavyDuty = 1,

    /// <summary>
    /// خودروی سبک
    /// </summary>
    LightDuty = 2
}
