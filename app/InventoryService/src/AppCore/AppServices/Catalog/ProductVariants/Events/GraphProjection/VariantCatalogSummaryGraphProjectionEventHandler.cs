namespace Insurance.InventoryService.AppCore.AppServices.Catalog.ProductVariants.Events.GraphProjection;

using Insurance.InventoryService.AppCore.AppServices.Catalog.Services;
using Insurance.InventoryService.AppCore.Domain.Catalog.Events;
using Insurance.InventoryService.AppCore.Shared.Catalog.Services;
using OysterFx.AppCore.Shared.Events;

public sealed class VariantCatalogSummaryGraphProjectionEventHandler :
    IDomainEventHandler<ProductVariantCreatedEvent>,
    IDomainEventHandler<ProductVariantUpdatedEvent>,
    IDomainEventHandler<ProductVariantActivationChangedEvent>,
    IDomainEventHandler<ProductVariantTrackingPolicyChangedEvent>,
    IDomainEventHandler<ProductVariantBaseUomChangedEvent>,
    IDomainEventHandler<ProductVariantInventoryMovementLockedEvent>,
    IDomainEventHandler<ProductVariantAttributeValueUpsertedEvent>,
    IDomainEventHandler<ProductVariantAttributeValueRemovedEvent>,
    IDomainEventHandler<ProductVariantUomConversionUpsertedEvent>,
    IDomainEventHandler<ProductVariantUomConversionRemovedEvent>
{
    private readonly IGraphProjectionService _graphProjectionService;

    public VariantCatalogSummaryGraphProjectionEventHandler(IGraphProjectionService graphProjectionService)
    {
        _graphProjectionService = graphProjectionService;
    }

    public async Task Handle(ProductVariantCreatedEvent @event)
    {
        var variantKey = CatalogGraphProjectionSchema.ToNodeKey(@event.ProductVariantBusinessKey);
        var summaryKey = CatalogGraphProjectionSchema.ToVariantCatalogSummaryKey(@event.ProductVariantBusinessKey);

        await UpsertSummaryNodeAsync(
            summaryKey,
            variantKey,
            @event.ProductRef,
            @event.VariantSku,
            @event.Barcode,
            @event.TrackingPolicy.ToString(),
            @event.BaseUomRef,
            @event.IsActive,
            @event.InventoryMovementLocked,
            @event.AttributeValues.Count,
            @event.UomConversions.Count,
            "ProductVariantCreatedEvent",
            @event.OccurredOn);

        await UpsertSummaryOfVariantRelationAsync(summaryKey, variantKey, @event.OccurredOn);
    }

    public async Task Handle(ProductVariantUpdatedEvent @event)
    {
        var variantKey = CatalogGraphProjectionSchema.ToNodeKey(@event.ProductVariantBusinessKey);
        var summaryKey = CatalogGraphProjectionSchema.ToVariantCatalogSummaryKey(@event.ProductVariantBusinessKey);

        await UpsertSummaryNodeAsync(
            summaryKey,
            variantKey,
            @event.ProductRef,
            @event.VariantSku,
            @event.Barcode,
            @event.TrackingPolicy.ToString(),
            @event.BaseUomRef,
            @event.IsActive,
            @event.InventoryMovementLocked,
            @event.AttributeValues.Count,
            @event.UomConversions.Count,
            "ProductVariantUpdatedEvent",
            @event.OccurredOn);

        await UpsertSummaryOfVariantRelationAsync(summaryKey, variantKey, @event.OccurredOn);
    }

    public async Task Handle(ProductVariantActivationChangedEvent @event)
    {
        var variantKey = CatalogGraphProjectionSchema.ToNodeKey(@event.ProductVariantBusinessKey);
        var summaryKey = CatalogGraphProjectionSchema.ToVariantCatalogSummaryKey(@event.ProductVariantBusinessKey);

        await _graphProjectionService.UpsertNodeAsync(new GraphProjectionNodeRequest(
            CatalogGraphProjectionSchema.VariantCatalogSummaryNode,
            summaryKey,
            new Dictionary<string, object?>(StringComparer.Ordinal)
            {
                ["variantRef"] = variantKey,
                ["isActive"] = @event.IsActive,
                ["lastEvent"] = "ProductVariantActivationChangedEvent",
                ["lastProjectedAt"] = @event.OccurredOn.ToString("O")
            }));

        await UpsertSummaryOfVariantRelationAsync(summaryKey, variantKey, @event.OccurredOn);
    }

    public async Task Handle(ProductVariantTrackingPolicyChangedEvent @event)
    {
        var variantKey = CatalogGraphProjectionSchema.ToNodeKey(@event.ProductVariantBusinessKey);
        var summaryKey = CatalogGraphProjectionSchema.ToVariantCatalogSummaryKey(@event.ProductVariantBusinessKey);

        await _graphProjectionService.UpsertNodeAsync(new GraphProjectionNodeRequest(
            CatalogGraphProjectionSchema.VariantCatalogSummaryNode,
            summaryKey,
            new Dictionary<string, object?>(StringComparer.Ordinal)
            {
                ["variantRef"] = variantKey,
                ["trackingPolicy"] = @event.TrackingPolicy.ToString(),
                ["previousTrackingPolicy"] = @event.PreviousTrackingPolicy.ToString(),
                ["lastEvent"] = "ProductVariantTrackingPolicyChangedEvent",
                ["lastProjectedAt"] = @event.OccurredOn.ToString("O")
            }));

        await UpsertSummaryOfVariantRelationAsync(summaryKey, variantKey, @event.OccurredOn);
    }

    public async Task Handle(ProductVariantBaseUomChangedEvent @event)
    {
        var variantKey = CatalogGraphProjectionSchema.ToNodeKey(@event.ProductVariantBusinessKey);
        var summaryKey = CatalogGraphProjectionSchema.ToVariantCatalogSummaryKey(@event.ProductVariantBusinessKey);

        await _graphProjectionService.UpsertNodeAsync(new GraphProjectionNodeRequest(
            CatalogGraphProjectionSchema.VariantCatalogSummaryNode,
            summaryKey,
            new Dictionary<string, object?>(StringComparer.Ordinal)
            {
                ["variantRef"] = variantKey,
                ["baseUomRef"] = CatalogGraphProjectionSchema.ToNodeKey(@event.BaseUomRef),
                ["previousBaseUomRef"] = CatalogGraphProjectionSchema.ToNodeKey(@event.PreviousBaseUomRef),
                ["lastEvent"] = "ProductVariantBaseUomChangedEvent",
                ["lastProjectedAt"] = @event.OccurredOn.ToString("O")
            }));

        await UpsertSummaryOfVariantRelationAsync(summaryKey, variantKey, @event.OccurredOn);
    }

    public async Task Handle(ProductVariantInventoryMovementLockedEvent @event)
    {
        var variantKey = CatalogGraphProjectionSchema.ToNodeKey(@event.ProductVariantBusinessKey);
        var summaryKey = CatalogGraphProjectionSchema.ToVariantCatalogSummaryKey(@event.ProductVariantBusinessKey);

        await _graphProjectionService.UpsertNodeAsync(new GraphProjectionNodeRequest(
            CatalogGraphProjectionSchema.VariantCatalogSummaryNode,
            summaryKey,
            new Dictionary<string, object?>(StringComparer.Ordinal)
            {
                ["variantRef"] = variantKey,
                ["inventoryMovementLocked"] = @event.InventoryMovementLocked,
                ["lastEvent"] = "ProductVariantInventoryMovementLockedEvent",
                ["lastProjectedAt"] = @event.OccurredOn.ToString("O")
            }));

        await UpsertSummaryOfVariantRelationAsync(summaryKey, variantKey, @event.OccurredOn);
    }

    public async Task Handle(ProductVariantAttributeValueUpsertedEvent @event)
    {
        var variantKey = CatalogGraphProjectionSchema.ToNodeKey(@event.ProductVariantBusinessKey);
        var summaryKey = CatalogGraphProjectionSchema.ToVariantCatalogSummaryKey(@event.ProductVariantBusinessKey);

        await _graphProjectionService.UpsertNodeAsync(new GraphProjectionNodeRequest(
            CatalogGraphProjectionSchema.VariantCatalogSummaryNode,
            summaryKey,
            new Dictionary<string, object?>(StringComparer.Ordinal)
            {
                ["variantRef"] = variantKey,
                ["lastAttributeRef"] = CatalogGraphProjectionSchema.ToNodeKey(@event.AttributeRef),
                ["lastAttributeValue"] = @event.Value,
                ["lastAttributeOptionRef"] = @event.OptionRef.HasValue
                    ? CatalogGraphProjectionSchema.ToNodeKey(@event.OptionRef.Value)
                    : null,
                ["lastEvent"] = "ProductVariantAttributeValueUpsertedEvent",
                ["lastProjectedAt"] = @event.OccurredOn.ToString("O")
            }));

        await UpsertSummaryOfVariantRelationAsync(summaryKey, variantKey, @event.OccurredOn);
    }

    public async Task Handle(ProductVariantAttributeValueRemovedEvent @event)
    {
        var variantKey = CatalogGraphProjectionSchema.ToNodeKey(@event.ProductVariantBusinessKey);
        var summaryKey = CatalogGraphProjectionSchema.ToVariantCatalogSummaryKey(@event.ProductVariantBusinessKey);

        await _graphProjectionService.UpsertNodeAsync(new GraphProjectionNodeRequest(
            CatalogGraphProjectionSchema.VariantCatalogSummaryNode,
            summaryKey,
            new Dictionary<string, object?>(StringComparer.Ordinal)
            {
                ["variantRef"] = variantKey,
                ["lastAttributeRef"] = CatalogGraphProjectionSchema.ToNodeKey(@event.AttributeRef),
                ["lastAttributeRemoved"] = true,
                ["lastEvent"] = "ProductVariantAttributeValueRemovedEvent",
                ["lastProjectedAt"] = @event.OccurredOn.ToString("O")
            }));

        await UpsertSummaryOfVariantRelationAsync(summaryKey, variantKey, @event.OccurredOn);
    }

    public async Task Handle(ProductVariantUomConversionUpsertedEvent @event)
    {
        var variantKey = CatalogGraphProjectionSchema.ToNodeKey(@event.ProductVariantBusinessKey);
        var summaryKey = CatalogGraphProjectionSchema.ToVariantCatalogSummaryKey(@event.ProductVariantBusinessKey);

        await _graphProjectionService.UpsertNodeAsync(new GraphProjectionNodeRequest(
            CatalogGraphProjectionSchema.VariantCatalogSummaryNode,
            summaryKey,
            new Dictionary<string, object?>(StringComparer.Ordinal)
            {
                ["variantRef"] = variantKey,
                ["lastConversionFromUomRef"] = CatalogGraphProjectionSchema.ToNodeKey(@event.FromUomRef),
                ["lastConversionToUomRef"] = CatalogGraphProjectionSchema.ToNodeKey(@event.ToUomRef),
                ["lastConversionFactor"] = @event.Factor,
                ["lastConversionRoundingMode"] = @event.RoundingMode.ToString(),
                ["lastConversionIsBasePath"] = @event.IsBasePath,
                ["lastEvent"] = "ProductVariantUomConversionUpsertedEvent",
                ["lastProjectedAt"] = @event.OccurredOn.ToString("O")
            }));

        await UpsertSummaryOfVariantRelationAsync(summaryKey, variantKey, @event.OccurredOn);
    }

    public async Task Handle(ProductVariantUomConversionRemovedEvent @event)
    {
        var variantKey = CatalogGraphProjectionSchema.ToNodeKey(@event.ProductVariantBusinessKey);
        var summaryKey = CatalogGraphProjectionSchema.ToVariantCatalogSummaryKey(@event.ProductVariantBusinessKey);

        await _graphProjectionService.UpsertNodeAsync(new GraphProjectionNodeRequest(
            CatalogGraphProjectionSchema.VariantCatalogSummaryNode,
            summaryKey,
            new Dictionary<string, object?>(StringComparer.Ordinal)
            {
                ["variantRef"] = variantKey,
                ["lastConversionFromUomRef"] = CatalogGraphProjectionSchema.ToNodeKey(@event.FromUomRef),
                ["lastConversionToUomRef"] = CatalogGraphProjectionSchema.ToNodeKey(@event.ToUomRef),
                ["lastConversionRemoved"] = true,
                ["lastEvent"] = "ProductVariantUomConversionRemovedEvent",
                ["lastProjectedAt"] = @event.OccurredOn.ToString("O")
            }));

        await UpsertSummaryOfVariantRelationAsync(summaryKey, variantKey, @event.OccurredOn);
    }

    private Task UpsertSummaryNodeAsync(
        string summaryKey,
        string variantKey,
        Guid productRef,
        string variantSku,
        string? barcode,
        string trackingPolicy,
        Guid baseUomRef,
        bool isActive,
        bool inventoryMovementLocked,
        int attributeValueCount,
        int uomConversionCount,
        string lastEvent,
        DateTime occurredOn)
    {
        return _graphProjectionService.UpsertNodeAsync(new GraphProjectionNodeRequest(
            CatalogGraphProjectionSchema.VariantCatalogSummaryNode,
            summaryKey,
            new Dictionary<string, object?>(StringComparer.Ordinal)
            {
                ["variantRef"] = variantKey,
                ["productRef"] = CatalogGraphProjectionSchema.ToNodeKey(productRef),
                ["variantSku"] = variantSku,
                ["barcode"] = barcode,
                ["trackingPolicy"] = trackingPolicy,
                ["baseUomRef"] = CatalogGraphProjectionSchema.ToNodeKey(baseUomRef),
                ["isActive"] = isActive,
                ["inventoryMovementLocked"] = inventoryMovementLocked,
                ["attributeValueCount"] = attributeValueCount,
                ["uomConversionCount"] = uomConversionCount,
                ["lastEvent"] = lastEvent,
                ["lastProjectedAt"] = occurredOn.ToString("O")
            }));
    }

    private Task UpsertSummaryOfVariantRelationAsync(string summaryKey, string variantKey, DateTime occurredOn)
    {
        return _graphProjectionService.UpsertRelationAsync(new GraphProjectionRelationRequest(
            CatalogGraphProjectionSchema.VariantCatalogSummaryNode,
            summaryKey,
            CatalogGraphProjectionSchema.ProductVariantNode,
            variantKey,
            CatalogGraphProjectionSchema.SummaryOfVariantRelation,
            new Dictionary<string, object?>(StringComparer.Ordinal)
            {
                ["isActive"] = true,
                ["lastProjectedAt"] = occurredOn.ToString("O")
            }));
    }
}

