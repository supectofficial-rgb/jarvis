namespace Insurance.CacheService.Endpoints.Api.Models.Dtos.Vector.CreateIndex;

public sealed record CreateVectorIndexRequest(
    string IndexName,
    string Prefix,
    int Dimension,
    string DistanceMetric,
    string Algorithm);
