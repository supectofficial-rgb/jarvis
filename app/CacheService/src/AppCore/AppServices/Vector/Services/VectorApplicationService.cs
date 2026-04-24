using Insurance.CacheService.AppCore.Shared.Vector.Services;

namespace Insurance.CacheService.AppCore.AppServices.Vector.Services;

public sealed class VectorApplicationService : IVectorApplicationService
{
    private readonly IVectorStoreService _vectorStoreService;

    public VectorApplicationService(IVectorStoreService vectorStoreService)
    {
        _vectorStoreService = vectorStoreService;
    }

    public Task<VectorIndexCreateResult> EnsureIndexAsync(VectorIndexDefinition definition, CancellationToken cancellationToken)
        => _vectorStoreService.EnsureIndexAsync(definition, cancellationToken);

    public Task UpsertAsync(string indexName, VectorItem item, CancellationToken cancellationToken)
        => _vectorStoreService.UpsertAsync(indexName, item, cancellationToken);

    public Task<IReadOnlyList<VectorSearchResultItem>> SearchAsync(string indexName, float[] queryEmbedding, int topK, string? namespaceFilter, CancellationToken cancellationToken)
        => _vectorStoreService.SearchAsync(indexName, queryEmbedding, topK, namespaceFilter, cancellationToken);

    public Task DeleteAsync(string key, CancellationToken cancellationToken)
        => _vectorStoreService.DeleteAsync(key, cancellationToken);
}
