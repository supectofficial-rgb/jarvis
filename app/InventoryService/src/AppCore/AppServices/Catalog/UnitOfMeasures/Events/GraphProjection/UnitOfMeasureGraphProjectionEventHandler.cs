namespace Insurance.InventoryService.AppCore.AppServices.Catalog.UnitOfMeasures.Events.GraphProjection;

using Insurance.InventoryService.AppCore.AppServices.Catalog.Services;
using Insurance.InventoryService.AppCore.Domain.Catalog.Events;
using Insurance.InventoryService.AppCore.Shared.Catalog.Services;
using OysterFx.AppCore.Shared.Events;

public sealed class UnitOfMeasureGraphProjectionEventHandler :
    IDomainEventHandler<UnitOfMeasureCreatedEvent>,
    IDomainEventHandler<UnitOfMeasureUpdatedEvent>,
    IDomainEventHandler<UnitOfMeasureActivationChangedEvent>
{
    private readonly IGraphProjectionService _graphProjectionService;

    public UnitOfMeasureGraphProjectionEventHandler(IGraphProjectionService graphProjectionService)
    {
        _graphProjectionService = graphProjectionService;
    }

    public Task Handle(UnitOfMeasureCreatedEvent @event)
    {
        return UpsertUnitOfMeasureNodeAsync(
            @event.UnitOfMeasureBusinessKey,
            @event.Code,
            @event.Name,
            @event.Precision,
            @event.IsActive,
            @event.OccurredOn);
    }

    public Task Handle(UnitOfMeasureUpdatedEvent @event)
    {
        return UpsertUnitOfMeasureNodeAsync(
            @event.UnitOfMeasureBusinessKey,
            @event.Code,
            @event.Name,
            @event.Precision,
            @event.IsActive,
            @event.OccurredOn);
    }

    public Task Handle(UnitOfMeasureActivationChangedEvent @event)
    {
        return _graphProjectionService.UpsertNodeAsync(new GraphProjectionNodeRequest(
            CatalogGraphProjectionSchema.UnitOfMeasureNode,
            CatalogGraphProjectionSchema.ToNodeKey(@event.UnitOfMeasureBusinessKey),
            new Dictionary<string, object?>(StringComparer.Ordinal)
            {
                ["isActive"] = @event.IsActive,
                ["lastProjectedAt"] = @event.OccurredOn.ToString("O")
            }));
    }

    private Task UpsertUnitOfMeasureNodeAsync(
        OysterFx.AppCore.Domain.ValueObjects.BusinessKey businessKey,
        string code,
        string name,
        int precision,
        bool isActive,
        DateTime occurredOn)
    {
        return _graphProjectionService.UpsertNodeAsync(new GraphProjectionNodeRequest(
            CatalogGraphProjectionSchema.UnitOfMeasureNode,
            CatalogGraphProjectionSchema.ToNodeKey(businessKey),
            new Dictionary<string, object?>(StringComparer.Ordinal)
            {
                ["code"] = code,
                ["name"] = name,
                ["precision"] = precision,
                ["isActive"] = isActive,
                ["lastProjectedAt"] = occurredOn.ToString("O")
            }));
    }
}
