namespace Insurance.AppCore.Domain.BaseData.CarGroups.Enums;

public enum ReplacementType : byte
{
    Unknown = 0,
    DirectReplacement = 1,      // جایگزین مستقیم
    Alternative = 2,            // جایگزین جایگزین
    Compatible = 3,             // سازگار
    Upgrade = 4,                // آپگرید
    Downgrade = 5,              // دانگرید
    CrossReference = 6          // ارجاع متقابل
}
