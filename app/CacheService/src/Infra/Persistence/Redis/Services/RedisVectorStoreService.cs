using Insurance.CacheService.AppCore.Shared.Vector.Services;
using StackExchange.Redis;
using System.Globalization;

namespace Insurance.CacheService.Infra.Persistence.Redis.Services;

public sealed class RedisVectorStoreService : IVectorStoreService
{
    private readonly IConnectionMultiplexer _connectionMultiplexer;

    public RedisVectorStoreService(IConnectionMultiplexer connectionMultiplexer)
    {
        _connectionMultiplexer = connectionMultiplexer;
    }

    public async Task<VectorIndexCreateResult> EnsureIndexAsync(VectorIndexDefinition definition, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var db = _connectionMultiplexer.GetDatabase();
        var args = new object[]
        {
            definition.IndexName,
            "ON", "HASH",
            "PREFIX", "1", definition.Prefix,
            "SCHEMA",
            "text", "TEXT",
            "namespace", "TAG",
            "actionName", "TAG",
            "embedding", "VECTOR", definition.Algorithm.ToUpperInvariant(), "10",
            "TYPE", "FLOAT32",
            "DIM", definition.Dimension.ToString(CultureInfo.InvariantCulture),
            "DISTANCE_METRIC", definition.DistanceMetric.ToUpperInvariant()
        };

        try
        {
            await db.ExecuteAsync("FT.CREATE", args);
            return new VectorIndexCreateResult(true, false, "Vector index created.");
        }
        catch (RedisServerException ex) when (ex.Message.Contains("Index already exists", StringComparison.OrdinalIgnoreCase))
        {
            return new VectorIndexCreateResult(false, true, "Vector index already exists.");
        }
    }

    public async Task UpsertAsync(string indexName, VectorItem item, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        _ = indexName;

        var db = _connectionMultiplexer.GetDatabase();
        var embeddingBytes = ToByteArray(item.Embedding);

        var hashEntries = new[]
        {
            new HashEntry("embedding", embeddingBytes),
            new HashEntry("text", item.Text ?? string.Empty),
            new HashEntry("namespace", item.Namespace ?? string.Empty),
            new HashEntry("actionName", item.ActionName ?? string.Empty)
        };

        await db.HashSetAsync(item.Key, hashEntries);
    }

    public async Task<IReadOnlyList<VectorSearchResultItem>> SearchAsync(string indexName, float[] queryEmbedding, int topK, string? namespaceFilter, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var db = _connectionMultiplexer.GetDatabase();
        var filter = string.IsNullOrWhiteSpace(namespaceFilter)
            ? "*"
            : $"@namespace:{{{EscapeTagValue(namespaceFilter)}}}";

        var query = $"{filter}=>[KNN {topK} @embedding $vec AS score]";

        var result = await db.ExecuteAsync(
            "FT.SEARCH",
            indexName,
            query,
            "PARAMS", "2", "vec", ToByteArray(queryEmbedding),
            "SORTBY", "score",
            "RETURN", "8", "score", "text", "namespace", "actionName",
            "DIALECT", "2");

        return ParseSearchResult(result);
    }

    public async Task DeleteAsync(string key, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var db = _connectionMultiplexer.GetDatabase();
        await db.KeyDeleteAsync(key);
    }

    private static IReadOnlyList<VectorSearchResultItem> ParseSearchResult(RedisResult result)
    {
        if (result.IsNull || result.Resp2Type != ResultType.Array)
        {
            return Array.Empty<VectorSearchResultItem>();
        }

        var arr = (RedisResult[])result;
        if (arr.Length <= 1)
        {
            return Array.Empty<VectorSearchResultItem>();
        }

        var items = new List<VectorSearchResultItem>();

        for (var i = 1; i < arr.Length; i += 2)
        {
            if (i + 1 >= arr.Length)
            {
                break;
            }

            var key = arr[i].ToString() ?? string.Empty;
            if (arr[i + 1].Resp2Type != ResultType.Array)
            {
                continue;
            }

            var fields = (RedisResult[])arr[i + 1];
            var dict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            for (var j = 0; j + 1 < fields.Length; j += 2)
            {
                var fieldName = fields[j].ToString() ?? string.Empty;
                var fieldValue = fields[j + 1].ToString() ?? string.Empty;
                dict[fieldName] = fieldValue;
            }

            var score = dict.TryGetValue("score", out var scoreText) && double.TryParse(scoreText, NumberStyles.Any, CultureInfo.InvariantCulture, out var parsedScore)
                ? parsedScore
                : double.NaN;

            dict.TryGetValue("text", out var text);
            dict.TryGetValue("namespace", out var itemNamespace);
            dict.TryGetValue("actionName", out var actionName);

            items.Add(new VectorSearchResultItem(key, score, text, itemNamespace, actionName));
        }

        return items;
    }

    private static byte[] ToByteArray(float[] values)
    {
        var bytes = new byte[values.Length * sizeof(float)];
        Buffer.BlockCopy(values, 0, bytes, 0, bytes.Length);
        return bytes;
    }

    private static string EscapeTagValue(string value)
        => value.Replace("-", "\\-");
}


