namespace Insurance.GraphService.AppCore.AppServices.GraphProjections.Events;

using Insurance.GraphService.AppCore.Shared.GraphNodes.Commands.UpsertGraphNode;
using OysterFx.AppCore.Shared.Commands;
using OysterFx.Infra.EventBus.Abstractions;
using OysterFx.Infra.EventBus.Contract.Events;
using OysterFx.Infra.EventBus.Contract.Events.GraphProjection;

public sealed class GraphNodeUpsertRequestedEventHandler(ICommandBus commandBus)
    : IEventHandler<GraphNodeUpsertRequestedIntegrationEvent>
{
    private readonly ICommandBus _commandBus = commandBus;

    public async Task HandleAsync(EventEnvelope<GraphNodeUpsertRequestedIntegrationEvent> envelope, CancellationToken ct)
    {
        var payload = envelope.Payload;

        var result = await _commandBus.SendAsync<UpsertGraphNodeCommand, UpsertGraphNodeCommandResult>(
            new UpsertGraphNodeCommand
            {
                NodeType = payload.NodeType,
                NodeKey = payload.NodeKey,
                Properties = new Dictionary<string, object?>(payload.Properties, StringComparer.Ordinal)
            });

        if (!result.IsSuccess)
            throw new InvalidOperationException(BuildError("node", payload.NodeType, payload.NodeKey, result.ErrorMessages));
    }

    private static string BuildError(string projectionType, string nodeType, string nodeKey, IEnumerable<string> errors)
    {
        var errorText = string.Join(" | ", errors.Where(static x => !string.IsNullOrWhiteSpace(x)));
        return $"Graph projection {projectionType} failed for {nodeType}/{nodeKey}. Errors: {errorText}";
    }
}
