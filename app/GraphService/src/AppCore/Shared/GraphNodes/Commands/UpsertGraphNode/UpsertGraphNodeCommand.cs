namespace Insurance.GraphService.AppCore.Shared.GraphNodes.Commands.UpsertGraphNode;

using OysterFx.AppCore.Shared.Commands;

public sealed class UpsertGraphNodeCommand : ICommand<UpsertGraphNodeCommandResult>
{
    public string NodeType { get; set; } = string.Empty;
    public string NodeKey { get; set; } = string.Empty;
    public Dictionary<string, object?> Properties { get; set; } = new(StringComparer.Ordinal);
}

public sealed class UpsertGraphNodeCommandResult
{
    public string NodeType { get; set; } = string.Empty;
    public string NodeKey { get; set; } = string.Empty;
}

