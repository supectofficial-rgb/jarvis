namespace Insurance.GraphService.Infra.InternalServices.Neo4j.ServiceCallers;

using Insurance.GraphService.AppCore.Shared.Graphs.Services;
using Insurance.GraphService.Infra.InternalServices.Neo4j.Models;
using Microsoft.Extensions.Options;
using System.Globalization;
using System.Text.Json;
using System.Text.RegularExpressions;

public sealed class Neo4jGraphStoreService : IGraphStoreService, IDisposable
{
    private static readonly Regex SafeNameRegex = new("^[A-Za-z_][A-Za-z0-9_]*$", RegexOptions.CultureInvariant);
    private static readonly Regex SafeRelRegex = new("^[A-Z_][A-Z0-9_]*$", RegexOptions.CultureInvariant);

    private readonly global::Neo4j.Driver.IDriver _driver;
    private bool _disposed;

    public Neo4jGraphStoreService(IOptions<Neo4jOptions> options)
    {
        var value = options.Value;
        if (string.IsNullOrWhiteSpace(value.Uri) || string.IsNullOrWhiteSpace(value.User) || string.IsNullOrWhiteSpace(value.Password))
            throw new InvalidOperationException("Neo4j configuration is missing. Set Neo4j:Uri, Neo4j:User, Neo4j:Password.");

        _driver = global::Neo4j.Driver.GraphDatabase.Driver(value.Uri, global::Neo4j.Driver.AuthTokens.Basic(value.User, value.Password));
    }

    public async Task UpsertNodeAsync(GraphNodeUpsertRequest request, CancellationToken cancellationToken = default)
    {
        var label = NormalizeLabel(request.NodeType);
        var nodeKey = NormalizeValue(request.NodeKey, nameof(request.NodeKey));

        var props = NormalizeProperties(request.Properties);
        props["nodeKey"] = nodeKey;
        props["nodeType"] = label;
        props["updatedAt"] = DateTimeOffset.UtcNow;

        var cypher = $@"
MERGE (n:{label} {{ nodeKey: $nodeKey }})
ON CREATE SET
    n.createdAt = $createdAt
SET
    n += $props
RETURN n";

        await using var session = _driver.AsyncSession();
        var cursor = await session.RunAsync(cypher, new
        {
            nodeKey,
            createdAt = DateTimeOffset.UtcNow,
            props
        });

        await cursor.FetchAsync();
    }

    public async Task UpsertRelationAsync(GraphRelationUpsertRequest request, CancellationToken cancellationToken = default)
    {
        var fromLabel = NormalizeLabel(request.FromNodeType);
        var toLabel = NormalizeLabel(request.ToNodeType);
        var relationType = NormalizeRelationType(request.RelationType);
        var fromKey = NormalizeValue(request.FromNodeKey, nameof(request.FromNodeKey));
        var toKey = NormalizeValue(request.ToNodeKey, nameof(request.ToNodeKey));

        var relationKey = $"{fromLabel}:{fromKey}|{relationType}|{toLabel}:{toKey}";
        var props = NormalizeProperties(request.Properties);
        props["relationKey"] = relationKey;
        props["relationType"] = relationType;
        props["updatedAt"] = DateTimeOffset.UtcNow;

        var cypher = $@"
MERGE (from:{fromLabel} {{ nodeKey: $fromKey }})
ON CREATE SET from.createdAt = $createdAt
SET from.nodeType = $fromNodeType
MERGE (to:{toLabel} {{ nodeKey: $toKey }})
ON CREATE SET to.createdAt = $createdAt
SET to.nodeType = $toNodeType
MERGE (from)-[r:{relationType} {{ relationKey: $relationKey }}]->(to)
ON CREATE SET r.createdAt = $createdAt
SET r += $props
RETURN r";

        await using var session = _driver.AsyncSession();
        var cursor = await session.RunAsync(cypher, new
        {
            fromKey,
            toKey,
            relationKey,
            fromNodeType = fromLabel,
            toNodeType = toLabel,
            createdAt = DateTimeOffset.UtcNow,
            props
        });

        await cursor.FetchAsync();
    }

    public async Task<IReadOnlyList<GraphNodeView>> GetNodesByTypeAsync(
        string nodeType,
        int limit,
        IReadOnlyDictionary<string, string>? equalsFilters,
        CancellationToken cancellationToken = default)
    {
        var label = NormalizeLabel(nodeType);
        var safeLimit = limit <= 0 ? 100 : Math.Min(limit, 1000);

        var whereParts = new List<string>();
        var parameters = new Dictionary<string, object?>
        {
            ["limit"] = safeLimit
        };

        if (equalsFilters is not null)
        {
            var idx = 0;
            foreach (var filter in equalsFilters)
            {
                var key = NormalizeProperty(filter.Key);
                var paramName = $"f{idx++}";
                whereParts.Add($"n.{key} = ${paramName}");
                parameters[paramName] = filter.Value;
            }
        }

        var whereClause = whereParts.Count == 0 ? string.Empty : "WHERE " + string.Join(" AND ", whereParts);
        var cypher = $@"
MATCH (n:{label})
{whereClause}
RETURN n.nodeKey AS nodeKey, properties(n) AS props
LIMIT $limit";

        var results = new List<GraphNodeView>();

        await using var session = _driver.AsyncSession();
        var cursor = await session.RunAsync(cypher, parameters);

        while (await cursor.FetchAsync())
        {
            var nodeKeyObj = cursor.Current["nodeKey"];
            var nodeKey = nodeKeyObj?.ToString() ?? string.Empty;

            var props = new Dictionary<string, object?>(StringComparer.Ordinal);
            if (cursor.Current["props"] is IDictionary<string, object?> dict)
            {
                foreach (var kv in dict)
                    props[kv.Key] = kv.Value;
            }
            else if (cursor.Current["props"] is IReadOnlyDictionary<string, object?> ro)
            {
                foreach (var kv in ro)
                    props[kv.Key] = kv.Value;
            }

            results.Add(new GraphNodeView(label, nodeKey, props));
        }

        return results;
    }

    private static Dictionary<string, object?> NormalizeProperties(IReadOnlyDictionary<string, object?> source)
    {
        var result = new Dictionary<string, object?>(StringComparer.Ordinal);

        foreach (var item in source)
        {
            var key = NormalizeProperty(item.Key);
            result[key] = NormalizePropertyValue(item.Value);
        }

        return result;
    }

    private static object? NormalizePropertyValue(object? value)
    {
        if (value is null)
            return null;

        if (value is JsonElement element)
            return FromJsonElement(element);

        if (value is JsonDocument document)
            return FromJsonElement(document.RootElement);

        if (value is Guid guid)
            return guid.ToString("D");

        if (value is Enum enumValue)
            return enumValue.ToString();

        if (value is DateTime dt)
            return dt.Kind == DateTimeKind.Unspecified ? DateTime.SpecifyKind(dt, DateTimeKind.Utc) : dt.ToUniversalTime();

        if (value is DateOnly dateOnly)
            return dateOnly.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc);

        if (value is TimeOnly timeOnly)
            return timeOnly.ToString("O", CultureInfo.InvariantCulture);

        if (value is IDictionary<string, object?> map)
            return NormalizeProperties(new Dictionary<string, object?>(map, StringComparer.Ordinal));

        if (value is IReadOnlyDictionary<string, object?> roMap)
            return NormalizeProperties(roMap);

        if (value is IEnumerable<object?> objectList && value is not string)
            return objectList.Select(NormalizePropertyValue).ToList();

        if (value is System.Collections.IEnumerable anyList && value is not string)
        {
            var list = new List<object?>();
            foreach (var item in anyList)
                list.Add(NormalizePropertyValue(item));

            return list;
        }

        return value;
    }

    private static object? FromJsonElement(JsonElement element)
    {
        return element.ValueKind switch
        {
            JsonValueKind.Null => null,
            JsonValueKind.Undefined => null,
            JsonValueKind.True => true,
            JsonValueKind.False => false,
            JsonValueKind.String => element.GetString(),
            JsonValueKind.Number => ConvertJsonNumber(element),
            JsonValueKind.Object => element.EnumerateObject().ToDictionary(
                p => NormalizeProperty(p.Name),
                p => FromJsonElement(p.Value),
                StringComparer.Ordinal),
            JsonValueKind.Array => element.EnumerateArray().Select(FromJsonElement).ToList(),
            _ => element.ToString()
        };
    }

    private static object ConvertJsonNumber(JsonElement element)
    {
        if (element.TryGetInt64(out var l))
            return l;

        if (element.TryGetDecimal(out var dec))
            return dec;

        if (element.TryGetDouble(out var dbl))
            return dbl;

        return element.GetRawText();
    }

    private static string NormalizeLabel(string label)
    {
        var normalized = NormalizeValue(label, nameof(label));
        if (!SafeNameRegex.IsMatch(normalized))
            throw new ArgumentException($"Invalid label '{label}'.", nameof(label));

        return normalized;
    }

    private static string NormalizeProperty(string property)
    {
        var normalized = NormalizeValue(property, nameof(property));
        if (!SafeNameRegex.IsMatch(normalized))
            throw new ArgumentException($"Invalid property '{property}'.", nameof(property));

        return normalized;
    }

    private static string NormalizeRelationType(string relationType)
    {
        var normalized = NormalizeValue(relationType, nameof(relationType)).ToUpperInvariant();
        normalized = Regex.Replace(normalized, "[^A-Z0-9_]", "_");

        if (!SafeRelRegex.IsMatch(normalized))
            throw new ArgumentException($"Invalid relation type '{relationType}'.", nameof(relationType));

        return normalized;
    }

    private static string NormalizeValue(string value, string paramName)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Value is required.", paramName);

        return value.Trim();
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    private void Dispose(bool disposing)
    {
        if (_disposed)
            return;

        if (disposing)
            _driver.Dispose();

        _disposed = true;
    }
}
