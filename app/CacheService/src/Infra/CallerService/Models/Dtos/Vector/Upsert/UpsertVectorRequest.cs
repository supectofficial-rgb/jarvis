namespace Insurance.CacheService.Infra.CallerService.Models.Dtos.Vector.Upsert;

public sealed record UpsertVectorRequest(
    string IndexName,
    string Key,
    float[] Embedding,
    string? Text,
    string? Namespace,
    string? ActionName);
