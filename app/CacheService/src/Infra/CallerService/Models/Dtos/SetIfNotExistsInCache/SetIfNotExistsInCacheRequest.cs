namespace Insurance.CacheService.Infra.CallerService.Models.Dtos.SetIfNotExistsInCache;

public sealed record SetIfNotExistsInCacheRequest(
    string Key,
    string Value,
    int? AbsoluteExpirationMinutes = null);
