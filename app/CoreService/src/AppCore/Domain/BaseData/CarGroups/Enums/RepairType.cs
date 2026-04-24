namespace Insurance.AppCore.Domain.BaseData.CarGroups.Enums;

public enum RepairType : byte
{
    Unknown = 0,
    Replacement = 1,            // تعویض
    Repair = 2,                 // تعمیر
    Refurbishment = 3,          // بازسازی
    Painting = 4,               // رنگ‌آمیزی
    Alignment = 5,              // تراز
    Welding = 6,                // جوشکاری
    Polishing = 7               // پولیش
}