namespace Insurance.InventoryService.Infra.InternalServices.GraphApiCaller.ServiceCallers;

using Insurance.InventoryService.AppCore.Shared.Catalog.Services;
using Insurance.InventoryService.Infra.InternalServices.GraphApiCaller.Models.Constants;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OysterFx.AppCore.Shared.DependencyInjections;
using OysterFx.Infra.EventBus.Abstractions;
using OysterFx.Infra.EventBus.Contract.Events;
using OysterFx.Infra.EventBus.Contract.Events.GraphProjection;
using System.Diagnostics;

public sealed class GraphProjectionService : IGraphProjectionService, ITransientLifetimeMarker
{
    private readonly IOptions<GraphApiOptions> _options;
    private readonly IEventBus _eventBus;
    private readonly ILogger<GraphProjectionService> _logger;

    public GraphProjectionService(
        IOptions<GraphApiOptions> options,
        IEventBus eventBus,
        ILogger<GraphProjectionService> logger)
    {
        _options = options;
        _eventBus = eventBus;
        _logger = logger;
    }

    public async Task UpsertNodeAsync(GraphProjectionNodeRequest request, CancellationToken cancellationToken = default)
    {
        var options = _options.Value;
        if (!options.Enabled)
            return;

        var payload = new GraphNodeUpsertRequestedIntegrationEvent
        {
            NodeType = request.NodeType,
            NodeKey = request.NodeKey,
            Properties = new Dictionary<string, object?>(request.Properties, StringComparer.Ordinal)
        };

        var metadata = BuildMetadata("inventory.graph.node.upsert");

        try
        {
            await _eventBus.PublishAsync(
                new EventEnvelope<GraphNodeUpsertRequestedIntegrationEvent>(
                    Guid.NewGuid(),
                    IntegrationEventTypeResolver.Resolve<GraphNodeUpsertRequestedIntegrationEvent>(),
                    metadata,
                    payload),
                cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Failed to publish graph node projection event for {NodeType}/{NodeKey}.",
                request.NodeType,
                request.NodeKey);
            throw;
        }
    }

    public async Task UpsertRelationAsync(GraphProjectionRelationRequest request, CancellationToken cancellationToken = default)
    {
        var options = _options.Value;
        if (!options.Enabled)
            return;

        var payload = new GraphRelationUpsertRequestedIntegrationEvent
        {
            FromNodeType = request.FromNodeType,
            FromNodeKey = request.FromNodeKey,
            ToNodeType = request.ToNodeType,
            ToNodeKey = request.ToNodeKey,
            RelationType = request.RelationType,
            Properties = new Dictionary<string, object?>(request.Properties, StringComparer.Ordinal)
        };

        var metadata = BuildMetadata("inventory.graph.relation.upsert");

        try
        {
            await _eventBus.PublishAsync(
                new EventEnvelope<GraphRelationUpsertRequestedIntegrationEvent>(
                    Guid.NewGuid(),
                    IntegrationEventTypeResolver.Resolve<GraphRelationUpsertRequestedIntegrationEvent>(),
                    metadata,
                    payload),
                cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Failed to publish graph relation projection event for {FromType}/{FromKey} -[{RelationType}]-> {ToType}/{ToKey}.",
                request.FromNodeType,
                request.FromNodeKey,
                request.RelationType,
                request.ToNodeType,
                request.ToNodeKey);
            throw;
        }
    }

    private static Dictionary<string, string> BuildMetadata(string projectionAction)
    {
        var metadata = new Dictionary<string, string>(StringComparer.Ordinal)
        {
            ["source"] = "InventoryService",
            ["projectionAction"] = projectionAction,
            ["publishedAtUtc"] = DateTime.UtcNow.ToString("O")
        };

        if (Activity.Current is not null)
        {
            metadata["traceId"] = Activity.Current.TraceId.ToHexString();
            metadata["spanId"] = Activity.Current.SpanId.ToHexString();
        }

        return metadata;
    }
}
