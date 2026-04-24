namespace Insurance.AppCore.Domain.BaseData.CarGroups.Enums;

public enum TransmissionType : byte
{
    Unknown = 0,
    Manual = 1,         // دستی
    Automatic = 2,      // اتوماتیک
    CVT = 3,            // سی‌وی‌تی
    DSG = 4,            // دی‌اس‌جی
    Tiptronic = 5       // تیپ ترونیک
}