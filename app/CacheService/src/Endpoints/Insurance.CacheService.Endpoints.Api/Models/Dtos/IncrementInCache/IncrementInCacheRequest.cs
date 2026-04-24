namespace Insurance.CacheService.Endpoints.Api.Models.Dtos.IncrementInCache;

public sealed record IncrementInCacheRequest(
    string Key,
    long Value = 1,
    int? AbsoluteExpirationMinutes = null);
