namespace Insurance.CacheService.AppCore.Shared.Vector.Services;

public sealed record VectorItem(
    string Key,
    float[] Embedding,
    string? Text,
    string? Namespace,
    string? ActionName);
