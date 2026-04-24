namespace Insurance.GraphService.AppCore.AppServices.GraphRelations.Commands.UpsertGraphRelation;

using Insurance.GraphService.AppCore.Shared.GraphRelations.Commands.UpsertGraphRelation;
using Insurance.GraphService.AppCore.Shared.Graphs.Services;
using OysterFx.AppCore.AppServices.Commands;
using OysterFx.AppCore.Shared.Commands.Common;

public sealed class UpsertGraphRelationCommandHandler(IGraphStoreService graphStoreService)
    : CommandHandler<UpsertGraphRelationCommand, UpsertGraphRelationCommandResult>
{
    private readonly IGraphStoreService _graphStoreService = graphStoreService;

    public override async Task<CommandResult<UpsertGraphRelationCommandResult>> Handle(UpsertGraphRelationCommand command)
    {
        if (string.IsNullOrWhiteSpace(command.FromNodeType) || string.IsNullOrWhiteSpace(command.FromNodeKey))
            return Fail("From node is required.");

        if (string.IsNullOrWhiteSpace(command.ToNodeType) || string.IsNullOrWhiteSpace(command.ToNodeKey))
            return Fail("To node is required.");

        if (string.IsNullOrWhiteSpace(command.RelationType))
            return Fail("RelationType is required.");

        await _graphStoreService.UpsertRelationAsync(new GraphRelationUpsertRequest(
            command.FromNodeType.Trim(),
            command.FromNodeKey.Trim(),
            command.ToNodeType.Trim(),
            command.ToNodeKey.Trim(),
            command.RelationType.Trim(),
            command.Properties));

        return await OkAsync(new UpsertGraphRelationCommandResult
        {
            RelationType = command.RelationType.Trim(),
            FromNodeKey = command.FromNodeKey.Trim(),
            ToNodeKey = command.ToNodeKey.Trim()
        });
    }
}


