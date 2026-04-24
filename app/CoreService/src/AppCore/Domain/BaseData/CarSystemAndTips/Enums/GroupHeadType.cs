namespace Insurance.AppCore.Domain.BaseData.CarSystemAndTips.Enums;

/// <summary>
/// انواع سرگروه خودرو
/// </summary>
public enum GroupHeadType : byte
{
    /// <summary>
    /// نامشخص
    /// </summary>
    Unknown = 0,

    /// <summary>
    /// اتوکار
    /// </summary>
    Wrecker = 1,

    /// <summary>
    /// بارکش
    /// </summary>
    Truck = 2,

    /// <summary>
    /// سواری
    /// </summary>
    PassengerCar = 3,

    /// <summary>
    /// ماشین‌آلات کشاورزی، راه‌سازی و ساختمانی
    /// </summary>
    ConstructionAndAgriculturalMachinery = 4,

    /// <summary>
    /// موتورسیکلت
    /// </summary>
    Motorcycle = 5,

    /// <summary>
    /// یدک‌کش
    /// </summary>
    Trailer = 6
}