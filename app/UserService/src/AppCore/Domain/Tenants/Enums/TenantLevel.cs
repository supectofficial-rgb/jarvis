namespace Insurance.UserService.AppCore.Domain.Tenants.Enums;

public enum TenantLevel : byte
{
    System = 1,      // سطح سیستم (تولیدکننده نرم‌افزار)
    Organization = 2, // سطح سازمان (مشتری)
    Application = 3   // سطح اپراتور نرم‌افزاری
}