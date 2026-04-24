namespace Insurance.GraphService.AppCore.AppServices.GraphNodes.Commands.UpsertGraphNode;

using Insurance.GraphService.AppCore.Shared.GraphNodes.Commands.UpsertGraphNode;
using Insurance.GraphService.AppCore.Shared.Graphs.Services;
using OysterFx.AppCore.AppServices.Commands;
using OysterFx.AppCore.Shared.Commands.Common;

public sealed class UpsertGraphNodeCommandHandler(IGraphStoreService graphStoreService)
    : CommandHandler<UpsertGraphNodeCommand, UpsertGraphNodeCommandResult>
{
    private readonly IGraphStoreService _graphStoreService = graphStoreService;

    public override async Task<CommandResult<UpsertGraphNodeCommandResult>> Handle(UpsertGraphNodeCommand command)
    {
        if (string.IsNullOrWhiteSpace(command.NodeType))
            return Fail("NodeType is required.");

        if (string.IsNullOrWhiteSpace(command.NodeKey))
            return Fail("NodeKey is required.");

        await _graphStoreService.UpsertNodeAsync(new GraphNodeUpsertRequest(
            command.NodeType.Trim(),
            command.NodeKey.Trim(),
            command.Properties));

        return await OkAsync(new UpsertGraphNodeCommandResult
        {
            NodeType = command.NodeType.Trim(),
            NodeKey = command.NodeKey.Trim()
        });
    }
}


