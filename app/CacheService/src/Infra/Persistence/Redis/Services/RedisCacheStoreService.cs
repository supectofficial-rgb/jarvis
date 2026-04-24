using Insurance.CacheService.AppCore.Shared.Cache.Services;
using Insurance.CacheService.AppCore.Shared.Configuration;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace Insurance.CacheService.Infra.Persistence.Redis.Services;

public sealed class RedisCacheStoreService : ICacheStoreService
{
    private readonly IDistributedCache _cache;
    private readonly IConnectionMultiplexer _connectionMultiplexer;
    private readonly IOptions<CacheStoreOptions> _cacheOptions;

    public RedisCacheStoreService(
        IDistributedCache cache,
        IConnectionMultiplexer connectionMultiplexer,
        IOptions<CacheStoreOptions> cacheOptions)
    {
        _cache = cache;
        _connectionMultiplexer = connectionMultiplexer;
        _cacheOptions = cacheOptions;
    }

    public async Task<string?> GetAsync(string key, CancellationToken cancellationToken)
        => await _cache.GetStringAsync(key, cancellationToken);

    public async Task SetAsync(string key, string value, int? absoluteExpirationMinutes, int? slidingExpirationMinutes, CancellationToken cancellationToken)
    {
        var options = new DistributedCacheEntryOptions();

        if (absoluteExpirationMinutes.HasValue)
        {
            options.AbsoluteExpiration = DateTimeOffset.UtcNow.AddMinutes(absoluteExpirationMinutes.Value);
        }
        else
        {
            options.AbsoluteExpiration = DateTimeOffset.UtcNow.AddMinutes(_cacheOptions.Value.DefaultExpirationMinutes);
        }

        if (slidingExpirationMinutes.HasValue)
        {
            options.SlidingExpiration = TimeSpan.FromMinutes(slidingExpirationMinutes.Value);
        }

        await _cache.SetStringAsync(key, value, options, cancellationToken);
    }

    public Task RemoveAsync(string key, CancellationToken cancellationToken)
        => _cache.RemoveAsync(key, cancellationToken);

    public async Task<bool> ExistsAsync(string key, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var db = _connectionMultiplexer.GetDatabase();
        return await db.KeyExistsAsync(key);
    }

    public async Task<bool> SetIfNotExistsAsync(string key, string value, int? absoluteExpirationMinutes, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var ttl = TimeSpan.FromMinutes(absoluteExpirationMinutes ?? _cacheOptions.Value.DefaultExpirationMinutes);
        var db = _connectionMultiplexer.GetDatabase();

        return await db.StringSetAsync(
            key,
            value,
            expiry: ttl,
            when: When.NotExists,
            flags: CommandFlags.None);
    }

    public async Task<long> IncrementAsync(string key, long value, int? absoluteExpirationMinutes, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var db = _connectionMultiplexer.GetDatabase();
        var newValue = await db.StringIncrementAsync(key, value);

        if (newValue == value)
        {
            var ttl = TimeSpan.FromMinutes(absoluteExpirationMinutes ?? _cacheOptions.Value.DefaultExpirationMinutes);
            await db.KeyExpireAsync(key, ttl);
        }

        return newValue;
    }
}
