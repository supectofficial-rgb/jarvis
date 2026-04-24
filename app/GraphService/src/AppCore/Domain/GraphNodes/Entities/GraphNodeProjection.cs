using OysterFx.AppCore.Domain.Aggregates;

namespace Insurance.GraphService.AppCore.Domain.GraphNodes.Entities;

public sealed class GraphNodeProjection : AggregateRoot
{
    public string NodeType { get; private set; } = string.Empty;
    public string NodeKey { get; private set; } = string.Empty;

    private GraphNodeProjection()
    {
    }

    private GraphNodeProjection(string nodeType, string nodeKey)
    {
        NodeType = nodeType;
        NodeKey = nodeKey;
    }

    public static GraphNodeProjection Create(string nodeType, string nodeKey)
        => new(nodeType, nodeKey);
}

