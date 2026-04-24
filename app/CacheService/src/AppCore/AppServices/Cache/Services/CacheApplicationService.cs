using Insurance.CacheService.AppCore.Shared.Cache.Services;

namespace Insurance.CacheService.AppCore.AppServices.Cache.Services;

public sealed class CacheApplicationService : ICacheApplicationService
{
    private readonly ICacheStoreService _cacheStoreService;

    public CacheApplicationService(ICacheStoreService cacheStoreService)
    {
        _cacheStoreService = cacheStoreService;
    }

    public Task<string?> GetAsync(string key, CancellationToken cancellationToken)
        => _cacheStoreService.GetAsync(key, cancellationToken);

    public Task SetAsync(string key, string value, int? absoluteExpirationMinutes, int? slidingExpirationMinutes, CancellationToken cancellationToken)
        => _cacheStoreService.SetAsync(key, value, absoluteExpirationMinutes, slidingExpirationMinutes, cancellationToken);

    public Task RemoveAsync(string key, CancellationToken cancellationToken)
        => _cacheStoreService.RemoveAsync(key, cancellationToken);

    public Task<bool> ExistsAsync(string key, CancellationToken cancellationToken)
        => _cacheStoreService.ExistsAsync(key, cancellationToken);

    public Task<bool> SetIfNotExistsAsync(string key, string value, int? absoluteExpirationMinutes, CancellationToken cancellationToken)
        => _cacheStoreService.SetIfNotExistsAsync(key, value, absoluteExpirationMinutes, cancellationToken);

    public Task<long> IncrementAsync(string key, long value, int? absoluteExpirationMinutes, CancellationToken cancellationToken)
        => _cacheStoreService.IncrementAsync(key, value, absoluteExpirationMinutes, cancellationToken);
}
