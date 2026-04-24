namespace Insurance.AppCore.Domain.BaseData.CarGroups.Enums;

public enum ImageType : byte
{
    Unknown = 0,
    Original = 1,               // تصویر اصلی
    Diagram = 2,                // دیاگرام
    Installation = 3,           // نصب
    Damage = 4,                 // آسیب
    Reference = 5               // مرجع
}
