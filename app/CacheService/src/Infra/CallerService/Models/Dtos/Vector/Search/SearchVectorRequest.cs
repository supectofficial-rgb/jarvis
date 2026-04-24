namespace Insurance.CacheService.Infra.CallerService.Models.Dtos.Vector.Search;

public sealed record SearchVectorRequest(
    string IndexName,
    float[] QueryEmbedding,
    int TopK = 5,
    string? NamespaceFilter = null);
