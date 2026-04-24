namespace Insurance.GraphService.AppCore.AppServices.GraphProjections.Events;

using Insurance.GraphService.AppCore.Shared.GraphRelations.Commands.UpsertGraphRelation;
using OysterFx.AppCore.Shared.Commands;
using OysterFx.Infra.EventBus.Abstractions;
using OysterFx.Infra.EventBus.Contract.Events;
using OysterFx.Infra.EventBus.Contract.Events.GraphProjection;

public sealed class GraphRelationUpsertRequestedEventHandler(ICommandBus commandBus)
    : IEventHandler<GraphRelationUpsertRequestedIntegrationEvent>
{
    private readonly ICommandBus _commandBus = commandBus;

    public async Task HandleAsync(EventEnvelope<GraphRelationUpsertRequestedIntegrationEvent> envelope, CancellationToken ct)
    {
        var payload = envelope.Payload;

        var result = await _commandBus.SendAsync<UpsertGraphRelationCommand, UpsertGraphRelationCommandResult>(
            new UpsertGraphRelationCommand
            {
                FromNodeType = payload.FromNodeType,
                FromNodeKey = payload.FromNodeKey,
                ToNodeType = payload.ToNodeType,
                ToNodeKey = payload.ToNodeKey,
                RelationType = payload.RelationType,
                Properties = new Dictionary<string, object?>(payload.Properties, StringComparer.Ordinal)
            });

        if (!result.IsSuccess)
            throw new InvalidOperationException(BuildError(payload, result.ErrorMessages));
    }

    private static string BuildError(
        GraphRelationUpsertRequestedIntegrationEvent payload,
        IEnumerable<string> errors)
    {
        var errorText = string.Join(" | ", errors.Where(static x => !string.IsNullOrWhiteSpace(x)));
        return $"Graph projection relation failed for {payload.FromNodeType}/{payload.FromNodeKey} -[{payload.RelationType}]-> {payload.ToNodeType}/{payload.ToNodeKey}. Errors: {errorText}";
    }
}
