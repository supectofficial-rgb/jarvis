namespace OysterFx.Infra.Auth.UserServices.Policies;

/// <summary>
/// تنظیمات مربوط به ثبت خودکار دسترسی‌ها
/// </summary>
public class AutoPermissionPolicyOptions
{
    /// <summary>
    /// فعال کردن به‌روزرسانی دوره‌ای Policies
    /// </summary>
    public bool EnablePeriodicUpdate { get; set; } = false;

    /// <summary>
    /// فاصله زمانی به‌روزرسانی (دقیقه)
    /// </summary>
    public int UpdateIntervalMinutes { get; set; } = 60;

    /// <summary>
    /// پیشوند Policy (اختیاری)
    /// </summary>
    public string PolicyPrefix { get; set; } = string.Empty;

    /// <summary>
    /// حذف Policies قبلی قبل از ثبت مجدد
    /// </summary>
    public bool ClearExistingPolicies { get; set; } = false;
}