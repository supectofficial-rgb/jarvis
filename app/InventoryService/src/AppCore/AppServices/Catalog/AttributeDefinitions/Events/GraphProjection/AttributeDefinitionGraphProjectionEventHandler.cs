namespace Insurance.InventoryService.AppCore.AppServices.Catalog.AttributeDefinitions.Events.GraphProjection;

using Insurance.InventoryService.AppCore.AppServices.Catalog.Services;
using Insurance.InventoryService.AppCore.Domain.Catalog.Events;
using Insurance.InventoryService.AppCore.Shared.Catalog.Services;
using OysterFx.AppCore.Shared.Events;

public sealed class AttributeDefinitionGraphProjectionEventHandler :
    IDomainEventHandler<AttributeDefinitionCreatedEvent>,
    IDomainEventHandler<AttributeDefinitionUpdatedEvent>,
    IDomainEventHandler<AttributeDefinitionActivationChangedEvent>,
    IDomainEventHandler<AttributeDefinitionOptionAddedEvent>,
    IDomainEventHandler<AttributeDefinitionOptionUpdatedEvent>,
    IDomainEventHandler<AttributeDefinitionOptionActivationChangedEvent>,
    IDomainEventHandler<AttributeDefinitionOptionRemovedEvent>
{
    private readonly IGraphProjectionService _graphProjectionService;

    public AttributeDefinitionGraphProjectionEventHandler(IGraphProjectionService graphProjectionService)
    {
        _graphProjectionService = graphProjectionService;
    }

    public async Task Handle(AttributeDefinitionCreatedEvent @event)
    {
        var attributeKey = CatalogGraphProjectionSchema.ToNodeKey(@event.AttributeDefinitionBusinessKey);

        await UpsertAttributeDefinitionNodeAsync(
            @event.AttributeDefinitionBusinessKey,
            @event.Code,
            @event.Name,
            @event.DataType.ToString(),
            @event.Scope.ToString(),
            @event.IsActive,
            @event.Options,
            @event.OccurredOn);

        foreach (var option in @event.Options)
        {
            await UpsertAttributeOptionAsync(
                attributeKey,
                option.OptionBusinessKey,
                option.Name,
                option.Value,
                option.DisplayOrder,
                option.IsActive,
                @event.OccurredOn);
        }
    }

    public async Task Handle(AttributeDefinitionUpdatedEvent @event)
    {
        var attributeKey = CatalogGraphProjectionSchema.ToNodeKey(@event.AttributeDefinitionBusinessKey);

        await UpsertAttributeDefinitionNodeAsync(
            @event.AttributeDefinitionBusinessKey,
            @event.Code,
            @event.Name,
            @event.DataType.ToString(),
            @event.Scope.ToString(),
            @event.IsActive,
            @event.Options,
            @event.OccurredOn);

        foreach (var option in @event.Options)
        {
            await UpsertAttributeOptionAsync(
                attributeKey,
                option.OptionBusinessKey,
                option.Name,
                option.Value,
                option.DisplayOrder,
                option.IsActive,
                @event.OccurredOn);
        }
    }

    public Task Handle(AttributeDefinitionActivationChangedEvent @event)
    {
        return _graphProjectionService.UpsertNodeAsync(new GraphProjectionNodeRequest(
            CatalogGraphProjectionSchema.AttributeDefinitionNode,
            CatalogGraphProjectionSchema.ToNodeKey(@event.AttributeDefinitionBusinessKey),
            new Dictionary<string, object?>(StringComparer.Ordinal)
            {
                ["isActive"] = @event.IsActive,
                ["lastProjectedAt"] = @event.OccurredOn.ToString("O")
            }));
    }

    public async Task Handle(AttributeDefinitionOptionAddedEvent @event)
    {
        var attributeKey = CatalogGraphProjectionSchema.ToNodeKey(@event.AttributeDefinitionBusinessKey);

        await _graphProjectionService.UpsertNodeAsync(new GraphProjectionNodeRequest(
            CatalogGraphProjectionSchema.AttributeDefinitionNode,
            attributeKey,
            new Dictionary<string, object?>(StringComparer.Ordinal)
            {
                ["lastOptionChangeType"] = "added",
                ["lastOptionName"] = @event.Name,
                ["lastOptionValue"] = @event.Value,
                ["lastOptionDisplayOrder"] = @event.DisplayOrder,
                ["lastOptionIsActive"] = @event.IsActive,
                ["lastProjectedAt"] = @event.OccurredOn.ToString("O")
            }));

        await UpsertAttributeOptionAsync(
            attributeKey,
            @event.OptionBusinessKey,
            @event.Name,
            @event.Value,
            @event.DisplayOrder,
            @event.IsActive,
            @event.OccurredOn);
    }

    public async Task Handle(AttributeDefinitionOptionUpdatedEvent @event)
    {
        var attributeKey = CatalogGraphProjectionSchema.ToNodeKey(@event.AttributeDefinitionBusinessKey);

        await UpsertAttributeOptionAsync(
            attributeKey,
            @event.OptionBusinessKey,
            @event.Name,
            @event.Value,
            @event.DisplayOrder,
            isActive: true,
            occurredOn: @event.OccurredOn);
    }

    public async Task Handle(AttributeDefinitionOptionActivationChangedEvent @event)
    {
        var attributeKey = CatalogGraphProjectionSchema.ToNodeKey(@event.AttributeDefinitionBusinessKey);

        await UpsertAttributeOptionAsync(
            attributeKey,
            @event.OptionBusinessKey,
            name: null,
            value: null,
            displayOrder: null,
            isActive: @event.IsActive,
            occurredOn: @event.OccurredOn);
    }

    public async Task Handle(AttributeDefinitionOptionRemovedEvent @event)
    {
        var attributeKey = CatalogGraphProjectionSchema.ToNodeKey(@event.AttributeDefinitionBusinessKey);

        await _graphProjectionService.UpsertNodeAsync(new GraphProjectionNodeRequest(
            CatalogGraphProjectionSchema.AttributeDefinitionNode,
            attributeKey,
            new Dictionary<string, object?>(StringComparer.Ordinal)
            {
                ["lastOptionChangeType"] = "removed",
                ["lastOptionValue"] = @event.Value,
                ["lastProjectedAt"] = @event.OccurredOn.ToString("O")
            }));

        await UpsertAttributeOptionAsync(
            attributeKey,
            @event.OptionBusinessKey,
            name: null,
            value: @event.Value,
            displayOrder: null,
            isActive: false,
            occurredOn: @event.OccurredOn);
    }

    private Task UpsertAttributeDefinitionNodeAsync(
        OysterFx.AppCore.Domain.ValueObjects.BusinessKey businessKey,
        string code,
        string name,
        string dataType,
        string scope,
        bool isActive,
        IReadOnlyCollection<AttributeDefinitionOptionSnapshot> options,
        DateTime occurredOn)
    {
        var optionValues = options
            .Select(static x => x.Value)
            .Where(static x => !string.IsNullOrWhiteSpace(x))
            .Select(static x => x.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();
        var optionNames = options
            .Select(static x => x.Name)
            .Where(static x => !string.IsNullOrWhiteSpace(x))
            .Select(static x => x.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();

        return _graphProjectionService.UpsertNodeAsync(new GraphProjectionNodeRequest(
            CatalogGraphProjectionSchema.AttributeDefinitionNode,
            CatalogGraphProjectionSchema.ToNodeKey(businessKey),
            new Dictionary<string, object?>(StringComparer.Ordinal)
            {
                ["code"] = code,
                ["name"] = name,
                ["dataType"] = dataType,
                ["scope"] = scope,
                ["isActive"] = isActive,
                ["optionCount"] = options.Count,
                ["optionNames"] = optionNames,
                ["optionValues"] = optionValues,
                ["lastProjectedAt"] = occurredOn.ToString("O")
            }));
    }

    private async Task UpsertAttributeOptionAsync(
        string attributeKey,
        Guid optionBusinessKey,
        string? name,
        string? value,
        int? displayOrder,
        bool isActive,
        DateTime occurredOn)
    {
        var optionKey = CatalogGraphProjectionSchema.ToNodeKey(optionBusinessKey);

        await _graphProjectionService.UpsertNodeAsync(new GraphProjectionNodeRequest(
            CatalogGraphProjectionSchema.AttributeOptionNode,
            optionKey,
            new Dictionary<string, object?>(StringComparer.Ordinal)
            {
                ["attributeRef"] = attributeKey,
                ["name"] = name,
                ["value"] = value,
                ["displayOrder"] = displayOrder,
                ["isActive"] = isActive,
                ["lastProjectedAt"] = occurredOn.ToString("O")
            }));

        await _graphProjectionService.UpsertRelationAsync(new GraphProjectionRelationRequest(
            CatalogGraphProjectionSchema.AttributeDefinitionNode,
            attributeKey,
            CatalogGraphProjectionSchema.AttributeOptionNode,
            optionKey,
            CatalogGraphProjectionSchema.AttributeHasOptionRelation,
            new Dictionary<string, object?>(StringComparer.Ordinal)
            {
                ["isActive"] = isActive,
                ["lastProjectedAt"] = occurredOn.ToString("O")
            }));
    }
}
