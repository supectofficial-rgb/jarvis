namespace Insurance.UserService.AppCore.AppServices.AAA.Services;

using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Insurance.CacheService.Infra.CallerService.Abstractions;
using Insurance.CacheService.Infra.CallerService.Models.Dtos.GetFromCache;
using Insurance.CacheService.Infra.CallerService.Models.Dtos.RemoveFromCache;
using Insurance.CacheService.Infra.CallerService.Models.Dtos.SetToCache;
using Insurance.UserService.AppCore.Domain.Common;
using Insurance.UserService.AppCore.Shared.AAA.Services;
using Microsoft.Extensions.Logging;

public sealed class RedisAuthSessionService : IAuthSessionService
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private readonly ICacheServiceCaller _cacheServiceCaller;
    private readonly ILogger<RedisAuthSessionService> _logger;

    public RedisAuthSessionService(ICacheServiceCaller cacheServiceCaller, ILogger<RedisAuthSessionService> logger)
    {
        _cacheServiceCaller = cacheServiceCaller;
        _logger = logger;
    }

    public string BuildKey(string sessionId) => $"auth:sessions:{sessionId}";

    public string HashRefreshToken(string refreshToken)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(refreshToken));
        return Convert.ToHexString(bytes);
    }

    public bool VerifyRefreshToken(string refreshToken, string expectedHash)
        => string.Equals(HashRefreshToken(refreshToken), expectedHash, StringComparison.OrdinalIgnoreCase);

    public async Task CreateAsync(AuthSession session, int absoluteExpirationMinutes, CancellationToken cancellationToken = default)
    {
        var payload = JsonSerializer.Serialize(session, JsonOptions);
        var response = await _cacheServiceCaller.SetAsync(new SetToCacheRequest(
            BuildKey(session.SessionId),
            payload,
            AbsoluteExpirationMinutes: absoluteExpirationMinutes,
            SlidingExpirationMinutes: absoluteExpirationMinutes));

        if (response.Error.Count > 0)
        {
            _logger.LogWarning("Failed to store auth session {SessionId}: {Errors}", session.SessionId, string.Join(" | ", response.Error));
        }
    }

    public async Task<AuthSession?> GetAsync(string sessionId, CancellationToken cancellationToken = default)
    {
        var response = await _cacheServiceCaller.GetAsync(new GetFromCacheRequest(BuildKey(sessionId)));
        if (response.Error.Count > 0 || string.IsNullOrWhiteSpace(response.Success.Value))
        {
            return null;
        }

        try
        {
            return JsonSerializer.Deserialize<AuthSession>(response.Success.Value, JsonOptions);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to deserialize auth session {SessionId}", sessionId);
            return null;
        }
    }

    public async Task<bool> RevokeAsync(string sessionId, string reason, CancellationToken cancellationToken = default)
    {
        var session = await GetAsync(sessionId, cancellationToken);
        if (session is null)
        {
            return false;
        }

        session.IsRevoked = true;
        session.RevokedAt = DateTime.UtcNow;
        session.RevokedReason = reason;
        return await UpdateAsync(session, 10, cancellationToken);
    }

    public async Task<bool> UpdateAsync(AuthSession session, int absoluteExpirationMinutes, CancellationToken cancellationToken = default)
    {
        var payload = JsonSerializer.Serialize(session, JsonOptions);
        var response = await _cacheServiceCaller.SetAsync(new SetToCacheRequest(
            BuildKey(session.SessionId),
            payload,
            AbsoluteExpirationMinutes: absoluteExpirationMinutes,
            SlidingExpirationMinutes: absoluteExpirationMinutes));

        return response.Error.Count == 0;
    }
}
