namespace Insurance.CacheService.AppCore.Shared.Vector.Services;

public interface IVectorStoreService
{
    Task<VectorIndexCreateResult> EnsureIndexAsync(VectorIndexDefinition definition, CancellationToken cancellationToken);
    Task UpsertAsync(string indexName, VectorItem item, CancellationToken cancellationToken);
    Task<IReadOnlyList<VectorSearchResultItem>> SearchAsync(string indexName, float[] queryEmbedding, int topK, string? namespaceFilter, CancellationToken cancellationToken);
    Task DeleteAsync(string key, CancellationToken cancellationToken);
}
