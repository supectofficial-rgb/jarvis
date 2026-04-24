namespace Insurance.CacheService.Endpoints.Api.Models.Dtos.SetIfNotExistsInCache;

public sealed record SetIfNotExistsInCacheRequest(
    string Key,
    string Value,
    int? AbsoluteExpirationMinutes = null);
