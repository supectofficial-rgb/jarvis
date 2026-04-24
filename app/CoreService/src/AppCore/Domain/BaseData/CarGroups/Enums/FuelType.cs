namespace Insurance.AppCore.Domain.BaseData.CarGroups.Enums;

public enum FuelType : byte
{
    Unknown = 0,
    Gasoline = 1,       // بنزینی
    Diesel = 2,         // دیزل
    Hybrid = 3,         // هیبریدی
    Electric = 4,       // الکتریکی
    CNG = 5,            // گاز طبیعی
    LPG = 6             // گاز مایع
}