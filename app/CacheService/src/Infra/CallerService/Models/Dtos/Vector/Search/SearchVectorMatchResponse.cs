namespace Insurance.CacheService.Infra.CallerService.Models.Dtos.Vector.Search;

public sealed record SearchVectorMatchResponse(
    string Key,
    double Score,
    string? Text,
    string? Namespace,
    string? ActionName);
