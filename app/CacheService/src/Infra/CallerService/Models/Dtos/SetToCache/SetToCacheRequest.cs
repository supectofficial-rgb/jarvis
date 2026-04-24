namespace Insurance.CacheService.Infra.CallerService.Models.Dtos.SetToCache;

public sealed record SetToCacheRequest(
    string Key,
    string Value,
    int? AbsoluteExpirationMinutes = null,
    int? SlidingExpirationMinutes = null);