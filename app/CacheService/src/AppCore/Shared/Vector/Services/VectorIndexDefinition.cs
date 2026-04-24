namespace Insurance.CacheService.AppCore.Shared.Vector.Services;

public sealed record VectorIndexDefinition(
    string IndexName,
    string Prefix,
    int Dimension,
    string DistanceMetric,
    string Algorithm);
