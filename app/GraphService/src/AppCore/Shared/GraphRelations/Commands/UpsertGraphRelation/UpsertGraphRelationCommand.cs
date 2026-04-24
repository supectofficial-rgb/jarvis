namespace Insurance.GraphService.AppCore.Shared.GraphRelations.Commands.UpsertGraphRelation;

using OysterFx.AppCore.Shared.Commands;

public sealed class UpsertGraphRelationCommand : ICommand<UpsertGraphRelationCommandResult>
{
    public string FromNodeType { get; set; } = string.Empty;
    public string FromNodeKey { get; set; } = string.Empty;
    public string ToNodeType { get; set; } = string.Empty;
    public string ToNodeKey { get; set; } = string.Empty;
    public string RelationType { get; set; } = "RELATED_TO";
    public Dictionary<string, object?> Properties { get; set; } = new(StringComparer.Ordinal);
}

public sealed class UpsertGraphRelationCommandResult
{
    public string RelationType { get; set; } = string.Empty;
    public string FromNodeKey { get; set; } = string.Empty;
    public string ToNodeKey { get; set; } = string.Empty;
}

