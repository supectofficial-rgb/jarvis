namespace Insurance.CacheService.AppCore.AppServices.Cache.Services;

public interface ICacheApplicationService
{
    Task<string?> GetAsync(string key, CancellationToken cancellationToken);
    Task SetAsync(string key, string value, int? absoluteExpirationMinutes, int? slidingExpirationMinutes, CancellationToken cancellationToken);
    Task RemoveAsync(string key, CancellationToken cancellationToken);
    Task<bool> ExistsAsync(string key, CancellationToken cancellationToken);
    Task<bool> SetIfNotExistsAsync(string key, string value, int? absoluteExpirationMinutes, CancellationToken cancellationToken);
    Task<long> IncrementAsync(string key, long value, int? absoluteExpirationMinutes, CancellationToken cancellationToken);
}
