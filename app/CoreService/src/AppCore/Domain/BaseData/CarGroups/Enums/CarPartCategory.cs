namespace Insurance.AppCore.Domain.BaseData.CarGroups.Enums;

public enum CarPartCategory : byte
{
    Unknown = 0,
    Engine = 1,                 // موتور
    Transmission = 2,           // گیربکس
    Body = 3,                   // بدنه
    Interior = 4,               // داخلی
    Electrical = 5,             // برقی
    Suspension = 6,             // تعلیق
    Brake = 7,                  // ترمز
    Exterior = 8,               // خارجی
    Glass = 9,                  // شیشه
    Lighting = 10,              // نور
    Cooling = 11,               // خنک‌کننده
    Exhaust = 12,               // اگزوز
    Fuel = 13,                  // سوخت
    Accessory = 14              // اکسسوری
}