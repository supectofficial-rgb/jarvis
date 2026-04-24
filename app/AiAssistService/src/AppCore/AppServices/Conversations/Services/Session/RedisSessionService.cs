using Insurance.AiAssistService.AppCore.AppServices.Conversations.Services.Caching;
using Insurance.CacheService.Infra.CallerService.Abstractions;
using Insurance.CacheService.Infra.CallerService.Models.Dtos.GetFromCache;
using Insurance.CacheService.Infra.CallerService.Models.Dtos.SetToCache;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace Insurance.AiAssistService.AppCore.AppServices.Conversations.Services.Session;

public sealed class RedisSessionService : ISessionService
{
    private readonly ICacheServiceCaller _cacheServiceCaller;
    private readonly IOptions<AiAssistCacheOptions> _cacheOptions;
    private readonly ILogger<RedisSessionService> _logger;

    public RedisSessionService(
        ICacheServiceCaller cacheServiceCaller,
        IOptions<AiAssistCacheOptions> cacheOptions,
        ILogger<RedisSessionService> logger)
    {
        _cacheServiceCaller = cacheServiceCaller;
        _cacheOptions = cacheOptions;
        _logger = logger;
    }

    public async Task<AssistantSession> GetOrCreateAsync(string? sessionId, string? userId, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var normalizedId = string.IsNullOrWhiteSpace(sessionId) ? Guid.NewGuid().ToString("N") : sessionId.Trim();
        var normalizedUser = string.IsNullOrWhiteSpace(userId) ? "anonymous" : userId.Trim();

        var existing = await TryGetAsync(normalizedId, cancellationToken);
        if (existing is not null)
        {
            if (string.IsNullOrWhiteSpace(existing.UserId))
            {
                existing.UserId = normalizedUser;
                await SaveAsync(existing, cancellationToken);
            }

            return existing;
        }

        var now = DateTimeOffset.UtcNow;
        var created = new AssistantSession
        {
            SessionId = normalizedId,
            UserId = normalizedUser,
            CreatedAtUtc = now,
            UpdatedAtUtc = now,
            WasTimedOut = !string.IsNullOrWhiteSpace(sessionId)
        };

        await SaveAsync(created, cancellationToken);
        return created;
    }

    public async Task<AssistantSession?> TryGetAsync(string sessionId, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (string.IsNullOrWhiteSpace(sessionId))
        {
            return null;
        }

        var key = AiAssistCacheKeys.Session(sessionId.Trim());
        var response = await _cacheServiceCaller.GetAsync(new GetFromCacheRequest(key));

        if (response.Error is not null && response.Error.Any())
        {
            _logger.LogWarning("Cache get failed for assistant session key {CacheKey}: {Errors}", key, string.Join(" | ", response.Error));
            return null;
        }

        var payload = response.Success?.Value;
        if (string.IsNullOrWhiteSpace(payload))
        {
            return null;
        }

        try
        {
            return JsonSerializer.Deserialize<AssistantSession>(payload);
        }
        catch (JsonException ex)
        {
            _logger.LogWarning(ex, "Failed to deserialize assistant session from cache key {CacheKey}", key);
            return null;
        }
    }

    public async Task SaveAsync(AssistantSession session, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        session.UpdatedAtUtc = DateTimeOffset.UtcNow;
        session.WasTimedOut = false;

        var payload = JsonSerializer.Serialize(session);
        var request = new SetToCacheRequest(
            Key: AiAssistCacheKeys.Session(session.SessionId),
            Value: payload,
            AbsoluteExpirationMinutes: _cacheOptions.Value.SessionAbsoluteExpirationMinutes);

        var response = await _cacheServiceCaller.SetAsync(request);
        if (response.Error is not null && response.Error.Any())
        {
            _logger.LogWarning(
                "Cache set failed for assistant session {SessionId}: {Errors}",
                session.SessionId,
                string.Join(" | ", response.Error));
        }
    }
}

