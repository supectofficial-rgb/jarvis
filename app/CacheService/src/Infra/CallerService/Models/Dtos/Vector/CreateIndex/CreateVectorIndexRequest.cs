namespace Insurance.CacheService.Infra.CallerService.Models.Dtos.Vector.CreateIndex;

public sealed record CreateVectorIndexRequest(
    string IndexName,
    string Prefix,
    int Dimension,
    string DistanceMetric,
    string Algorithm);
