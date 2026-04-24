namespace Insurance.CacheService.Infra.CallerService.Models.Dtos.IncrementInCache;

public sealed record IncrementInCacheRequest(
    string Key,
    long Value = 1,
    int? AbsoluteExpirationMinutes = null);
