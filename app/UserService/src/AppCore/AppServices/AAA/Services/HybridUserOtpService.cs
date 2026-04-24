namespace Insurance.UserService.AppCore.AppServices.AAA.Services;

using Insurance.UserService.AppCore.Shared.AAA.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

public sealed class HybridUserOtpService : IUserOtpService
{
    private readonly UserOtpService _primaryUserOtpService;
    private readonly ILegacyUserOtpService _legacyUserOtpService;
    private readonly IOptions<LegacyOtpFallbackOptions> _fallbackOptions;
    private readonly ILogger<HybridUserOtpService> _logger;

    public HybridUserOtpService(
        UserOtpService primaryUserOtpService,
        ILegacyUserOtpService legacyUserOtpService,
        IOptions<LegacyOtpFallbackOptions> fallbackOptions,
        ILogger<HybridUserOtpService> logger)
    {
        _primaryUserOtpService = primaryUserOtpService;
        _legacyUserOtpService = legacyUserOtpService;
        _fallbackOptions = fallbackOptions;
        _logger = logger;
    }

    public async Task<(bool Success, string? Code, string? Error, TimeSpan? Expiration)> CreateAsync(string mobileNumber)
    {
        var primary = await _primaryUserOtpService.CreateAsync(mobileNumber);

        if (primary.Success || !IsCacheError(primary.Error) || !_fallbackOptions.Value.Enabled)
        {
            return primary;
        }

        _logger.LogWarning("Primary OTP service failed with cache error. Falling back to legacy OTP service for mobile {MobileNumber}", mobileNumber);

        try
        {
            return await _legacyUserOtpService.CreateAsync(mobileNumber);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Legacy OTP fallback create failed for mobile {MobileNumber}", mobileNumber);
            return (false, null, "OTP_SERVICE_UNAVAILABLE", null);
        }
    }

    public async Task<(bool Verified, string Message)> VerifyAsync(string mobileNumber, string code)
    {
        var primary = await _primaryUserOtpService.VerifyAsync(mobileNumber, code);

        if (primary.Verified || !IsCacheError(primary.Message) || !_fallbackOptions.Value.Enabled)
        {
            return primary;
        }

        _logger.LogWarning("Primary OTP verify failed with cache error. Falling back to legacy OTP service for mobile {MobileNumber}", mobileNumber);

        try
        {
            return await _legacyUserOtpService.VerifyAsync(mobileNumber, code);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Legacy OTP fallback verify failed for mobile {MobileNumber}", mobileNumber);
            return (false, "OTP_SERVICE_UNAVAILABLE");
        }
    }

    private static bool IsCacheError(string? message)
        => string.Equals(message, "CACHE_ERROR", StringComparison.OrdinalIgnoreCase)
           || string.Equals(message, "CACHE_UNAVAILABLE", StringComparison.OrdinalIgnoreCase);
}
