namespace Insurance.CacheService.AppCore.Shared.Vector.Services;

public sealed record VectorSearchResultItem(
    string Key,
    double Score,
    string? Text,
    string? Namespace,
    string? ActionName);
