namespace Insurance.UserService.AppCore.Shared.AAA.Services;

public sealed class LegacyOtpFallbackOptions
{
    public const string SectionName = "Otp:LegacyFallback";
    public bool Enabled { get; set; } = true;
}
