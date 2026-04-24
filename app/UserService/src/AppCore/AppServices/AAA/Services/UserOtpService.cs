namespace Insurance.UserService.AppCore.AppServices.AAA.Services;

using Insurance.CacheService.Infra.CallerService.Abstractions;
using Insurance.CacheService.Infra.CallerService.Models.Dtos.ExistsInCache;
using Insurance.CacheService.Infra.CallerService.Models.Dtos.GetFromCache;
using Insurance.CacheService.Infra.CallerService.Models.Dtos.IncrementInCache;
using Insurance.CacheService.Infra.CallerService.Models.Dtos.RemoveFromCache;
using Insurance.CacheService.Infra.CallerService.Models.Dtos.SetIfNotExistsInCache;
using Insurance.CacheService.Infra.CallerService.Models.Dtos.SetToCache;
using Insurance.UserService.AppCore.Shared.AAA.Services;
using System.Security.Cryptography;

public sealed class UserOtpService
{
    private const int OtpLength = 5;
    private const int OtpExpirationMinutes = 2;
    private const int MaxVerifyAttempts = 3;
    private const int SendCooldownMinutes = 1;
    private const int VerifyBlockMinutes = 10;

    private const string CacheErrorMessage = "CACHE_ERROR";

    private readonly ICacheServiceCaller _cacheServiceCaller;

    public UserOtpService(ICacheServiceCaller cacheServiceCaller)
    {
        _cacheServiceCaller = cacheServiceCaller;
    }

    public async Task<(bool Success, string? Code, string? Error, TimeSpan? Expiration)> CreateAsync(string mobileNumber)
    {
        if (string.IsNullOrWhiteSpace(mobileNumber))
        {
            return (false, null, "Invalid mobile number", null);
        }

        var normalizedMobile = mobileNumber.Trim();

        var blocked = await _cacheServiceCaller.ExistsAsync(new ExistsInCacheRequest(BlockKey(normalizedMobile)));
        if (blocked.Error is { Count: > 0 })
        {
            return (false, null, CacheErrorMessage, null);
        }

        if (blocked.Success.Exists)
        {
            return (false, null, "You are temporarily blocked", null);
        }

        var sendLock = await _cacheServiceCaller.SetIfNotExistsAsync(new SetIfNotExistsInCacheRequest(
            Key: SendLockKey(normalizedMobile),
            Value: DateTime.UtcNow.ToString("O"),
            AbsoluteExpirationMinutes: SendCooldownMinutes));

        if (sendLock.Error is { Count: > 0 })
        {
            return (false, null, CacheErrorMessage, null);
        }

        if (!sendLock.Success.Success)
        {
            return (false, null, "Please try again later", null);
        }

        var code = GenerateOtpCode(OtpLength);

        var setOtp = await _cacheServiceCaller.SetAsync(new SetToCacheRequest(
            Key: OtpKey(normalizedMobile),
            Value: code,
            AbsoluteExpirationMinutes: OtpExpirationMinutes));

        if (setOtp.Error is { Count: > 0 })
        {
            return (false, null, CacheErrorMessage, null);
        }

        await _cacheServiceCaller.RemoveAsync(new RemoveFromCacheRequest(AttemptsKey(normalizedMobile)));

        return (true, code, null, TimeSpan.FromMinutes(OtpExpirationMinutes));
    }

    public async Task<(bool Verified, string Message)> VerifyAsync(string mobileNumber, string code)
    {
        if (string.IsNullOrWhiteSpace(mobileNumber) || string.IsNullOrWhiteSpace(code))
        {
            return (false, "Invalid code");
        }

        var normalizedMobile = mobileNumber.Trim();
        var normalizedCode = code.Trim();

        var blocked = await _cacheServiceCaller.ExistsAsync(new ExistsInCacheRequest(BlockKey(normalizedMobile)));
        if (blocked.Error is { Count: > 0 })
        {
            return (false, CacheErrorMessage);
        }

        if (blocked.Success.Exists)
        {
            return (false, "You are temporarily blocked");
        }

        var otp = await _cacheServiceCaller.GetAsync(new GetFromCacheRequest(OtpKey(normalizedMobile)));
        if (otp.Error is { Count: > 0 })
        {
            return (false, CacheErrorMessage);
        }

        if (string.IsNullOrWhiteSpace(otp.Success.Value))
        {
            return (false, "Invalid code");
        }

        if (string.Equals(otp.Success.Value.Trim(), normalizedCode, StringComparison.Ordinal))
        {
            await _cacheServiceCaller.RemoveAsync(new RemoveFromCacheRequest(OtpKey(normalizedMobile)));
            await _cacheServiceCaller.RemoveAsync(new RemoveFromCacheRequest(AttemptsKey(normalizedMobile)));
            return (true, "Verified");
        }

        var attempts = await _cacheServiceCaller.IncrementAsync(new IncrementInCacheRequest(
            Key: AttemptsKey(normalizedMobile),
            Value: 1,
            AbsoluteExpirationMinutes: OtpExpirationMinutes));

        if (attempts.Error is { Count: > 0 })
        {
            return (false, CacheErrorMessage);
        }

        if (attempts.Success.Value >= MaxVerifyAttempts)
        {
            await _cacheServiceCaller.SetAsync(new SetToCacheRequest(
                Key: BlockKey(normalizedMobile),
                Value: DateTime.UtcNow.ToString("O"),
                AbsoluteExpirationMinutes: VerifyBlockMinutes));

            await _cacheServiceCaller.RemoveAsync(new RemoveFromCacheRequest(OtpKey(normalizedMobile)));
            await _cacheServiceCaller.RemoveAsync(new RemoveFromCacheRequest(AttemptsKey(normalizedMobile)));

            return (false, "Too many invalid attempts");
        }

        var remainingAttempts = MaxVerifyAttempts - attempts.Success.Value;
        return (false, $"Invalid code. {remainingAttempts} attempts remaining");
    }

    private static string OtpKey(string mobileNumber) => $"user-auth--otp--{mobileNumber}";

    private static string AttemptsKey(string mobileNumber) => $"user-auth--otp-attempts--{mobileNumber}";

    private static string SendLockKey(string mobileNumber) => $"user-auth--otp-send-lock--{mobileNumber}";

    private static string BlockKey(string mobileNumber) => $"user-auth--otp-block--{mobileNumber}";

    private static string GenerateOtpCode(int length)
    {
        Span<byte> randomBytes = stackalloc byte[length];
        RandomNumberGenerator.Fill(randomBytes);

        var chars = new char[length];
        for (var i = 0; i < length; i++)
        {
            chars[i] = (char)('0' + (randomBytes[i] % 10));
        }

        return new string(chars);
    }
}
