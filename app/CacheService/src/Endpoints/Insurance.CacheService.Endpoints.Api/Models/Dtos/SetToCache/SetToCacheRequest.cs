namespace Insurance.CacheService.Endpoints.Api.Models.Dtos.SetToCache;

public sealed record SetToCacheRequest(
    string Key,
    string Value,
    int? AbsoluteExpirationMinutes = null,
    int? SlidingExpirationMinutes = null);