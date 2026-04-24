namespace Insurance.CacheService.Endpoints.Api.Models.Dtos.Vector.Search;

public sealed record SearchVectorMatchResponse(
    string Key,
    double Score,
    string? Text,
    string? Namespace,
    string? ActionName);
