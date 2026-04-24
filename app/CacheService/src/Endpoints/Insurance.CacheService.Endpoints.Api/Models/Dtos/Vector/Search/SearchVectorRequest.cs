namespace Insurance.CacheService.Endpoints.Api.Models.Dtos.Vector.Search;

public sealed record SearchVectorRequest(
    string IndexName,
    float[] QueryEmbedding,
    int TopK = 5,
    string? NamespaceFilter = null);
