namespace Insurance.InventoryService.AppCore.AppServices.Catalog.ProductVariants.Events.GraphProjection;

using Insurance.InventoryService.AppCore.AppServices.Catalog.Services;
using Insurance.InventoryService.AppCore.Domain.Catalog.Events;
using Insurance.InventoryService.AppCore.Shared.Catalog.Services;
using OysterFx.AppCore.Shared.Events;

public sealed class ProductVariantGraphProjectionEventHandler :
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

    public ProductVariantGraphProjectionEventHandler(IGraphProjectionService graphProjectionService)
    {
        _graphProjectionService = graphProjectionService;
    }

    public async Task Handle(ProductVariantCreatedEvent @event)
    {
        var variantKey = CatalogGraphProjectionSchema.ToNodeKey(@event.ProductVariantBusinessKey);

        await UpsertVariantNodeAsync(
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
            @event.OccurredOn);

        await UpsertVariantProductRelationAsync(variantKey, @event.ProductRef, isCurrent: true, isActive: true, @event.OccurredOn);
        await UpsertVariantBaseUomRelationAsync(variantKey, @event.BaseUomRef, isCurrent: true, isActive: true, @event.OccurredOn);

        foreach (var attributeValue in @event.AttributeValues)
        {
            await UpsertVariantAttributeValueAsync(
                variantKey,
                attributeValue.AttributeRef,
                attributeValue.Value,
                attributeValue.OptionRef,
                isActive: true,
                @event.OccurredOn);
        }

        foreach (var conversion in @event.UomConversions)
        {
            await UpsertVariantUomConversionAsync(
                variantKey,
                conversion.FromUomRef,
                conversion.ToUomRef,
                isActive: true,
                @event.OccurredOn,
                conversion.Factor,
                conversion.RoundingMode.ToString(),
                conversion.IsBasePath);
        }
    }

    public async Task Handle(ProductVariantUpdatedEvent @event)
    {
        var variantKey = CatalogGraphProjectionSchema.ToNodeKey(@event.ProductVariantBusinessKey);

        await UpsertVariantNodeAsync(
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
            @event.OccurredOn);

        await UpsertVariantProductRelationAsync(variantKey, @event.ProductRef, isCurrent: true, isActive: true, @event.OccurredOn);
        await UpsertVariantBaseUomRelationAsync(variantKey, @event.BaseUomRef, isCurrent: true, isActive: true, @event.OccurredOn);

        foreach (var attributeValue in @event.AttributeValues)
        {
            await UpsertVariantAttributeValueAsync(
                variantKey,
                attributeValue.AttributeRef,
                attributeValue.Value,
                attributeValue.OptionRef,
                isActive: true,
                @event.OccurredOn);
        }

        foreach (var conversion in @event.UomConversions)
        {
            await UpsertVariantUomConversionAsync(
                variantKey,
                conversion.FromUomRef,
                conversion.ToUomRef,
                isActive: true,
                @event.OccurredOn,
                conversion.Factor,
                conversion.RoundingMode.ToString(),
                conversion.IsBasePath);
        }
    }

    public Task Handle(ProductVariantActivationChangedEvent @event)
    {
        return _graphProjectionService.UpsertNodeAsync(new GraphProjectionNodeRequest(
            CatalogGraphProjectionSchema.ProductVariantNode,
            CatalogGraphProjectionSchema.ToNodeKey(@event.ProductVariantBusinessKey),
            new Dictionary<string, object?>(StringComparer.Ordinal)
            {
                ["isActive"] = @event.IsActive,
                ["lastProjectedAt"] = @event.OccurredOn.ToString("O")
            }));
    }

    public Task Handle(ProductVariantTrackingPolicyChangedEvent @event)
    {
        return _graphProjectionService.UpsertNodeAsync(new GraphProjectionNodeRequest(
            CatalogGraphProjectionSchema.ProductVariantNode,
            CatalogGraphProjectionSchema.ToNodeKey(@event.ProductVariantBusinessKey),
            new Dictionary<string, object?>(StringComparer.Ordinal)
            {
                ["trackingPolicy"] = @event.TrackingPolicy.ToString(),
                ["previousTrackingPolicy"] = @event.PreviousTrackingPolicy.ToString(),
                ["lastProjectedAt"] = @event.OccurredOn.ToString("O")
            }));
    }

    public async Task Handle(ProductVariantBaseUomChangedEvent @event)
    {
        var variantKey = CatalogGraphProjectionSchema.ToNodeKey(@event.ProductVariantBusinessKey);

        await _graphProjectionService.UpsertNodeAsync(new GraphProjectionNodeRequest(
            CatalogGraphProjectionSchema.ProductVariantNode,
            variantKey,
            new Dictionary<string, object?>(StringComparer.Ordinal)
            {
                ["baseUomRef"] = CatalogGraphProjectionSchema.ToNodeKey(@event.BaseUomRef),
                ["lastProjectedAt"] = @event.OccurredOn.ToString("O")
            }));

        await UpsertVariantBaseUomRelationAsync(variantKey, @event.PreviousBaseUomRef, isCurrent: false, isActive: false, @event.OccurredOn);
        await UpsertVariantBaseUomRelationAsync(variantKey, @event.BaseUomRef, isCurrent: true, isActive: true, @event.OccurredOn);
    }

    public Task Handle(ProductVariantInventoryMovementLockedEvent @event)
    {
        return _graphProjectionService.UpsertNodeAsync(new GraphProjectionNodeRequest(
            CatalogGraphProjectionSchema.ProductVariantNode,
            CatalogGraphProjectionSchema.ToNodeKey(@event.ProductVariantBusinessKey),
            new Dictionary<string, object?>(StringComparer.Ordinal)
            {
                ["inventoryMovementLocked"] = @event.InventoryMovementLocked,
                ["lastProjectedAt"] = @event.OccurredOn.ToString("O")
            }));
    }

    public Task Handle(ProductVariantAttributeValueUpsertedEvent @event)
    {
        return UpsertVariantAttributeValueAsync(
            CatalogGraphProjectionSchema.ToNodeKey(@event.ProductVariantBusinessKey),
            @event.AttributeRef,
            @event.Value,
            @event.OptionRef,
            isActive: true,
            @event.OccurredOn);
    }

    public Task Handle(ProductVariantAttributeValueRemovedEvent @event)
    {
        return UpsertVariantAttributeValueAsync(
            CatalogGraphProjectionSchema.ToNodeKey(@event.ProductVariantBusinessKey),
            @event.AttributeRef,
            value: null,
            optionRef: null,
            isActive: false,
            @event.OccurredOn);
    }

    public Task Handle(ProductVariantUomConversionUpsertedEvent @event)
    {
        return UpsertVariantUomConversionAsync(
            CatalogGraphProjectionSchema.ToNodeKey(@event.ProductVariantBusinessKey),
            @event.FromUomRef,
            @event.ToUomRef,
            isActive: true,
            @event.OccurredOn,
            @event.Factor,
            @event.RoundingMode.ToString(),
            @event.IsBasePath);
    }

    public Task Handle(ProductVariantUomConversionRemovedEvent @event)
    {
        return UpsertVariantUomConversionAsync(
            CatalogGraphProjectionSchema.ToNodeKey(@event.ProductVariantBusinessKey),
            @event.FromUomRef,
            @event.ToUomRef,
            isActive: false,
            @event.OccurredOn,
            factor: null,
            roundingMode: null,
            isBasePath: null);
    }

    private Task UpsertVariantNodeAsync(
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
        DateTime occurredOn)
    {
        return _graphProjectionService.UpsertNodeAsync(new GraphProjectionNodeRequest(
            CatalogGraphProjectionSchema.ProductVariantNode,
            variantKey,
            new Dictionary<string, object?>(StringComparer.Ordinal)
            {
                ["productRef"] = CatalogGraphProjectionSchema.ToNodeKey(productRef),
                ["variantSku"] = variantSku,
                ["barcode"] = barcode,
                ["trackingPolicy"] = trackingPolicy,
                ["baseUomRef"] = CatalogGraphProjectionSchema.ToNodeKey(baseUomRef),
                ["isActive"] = isActive,
                ["inventoryMovementLocked"] = inventoryMovementLocked,
                ["attributeValueCount"] = attributeValueCount,
                ["uomConversionCount"] = uomConversionCount,
                ["lastProjectedAt"] = occurredOn.ToString("O")
            }));
    }

    private Task UpsertVariantProductRelationAsync(string variantKey, Guid productRef, bool isCurrent, bool isActive, DateTime occurredOn)
    {
        return _graphProjectionService.UpsertRelationAsync(new GraphProjectionRelationRequest(
            CatalogGraphProjectionSchema.ProductVariantNode,
            variantKey,
            CatalogGraphProjectionSchema.ProductNode,
            CatalogGraphProjectionSchema.ToNodeKey(productRef),
            CatalogGraphProjectionSchema.VariantOfProductRelation,
            new Dictionary<string, object?>(StringComparer.Ordinal)
            {
                ["isCurrent"] = isCurrent,
                ["isActive"] = isActive,
                ["lastProjectedAt"] = occurredOn.ToString("O")
            }));
    }

    private Task UpsertVariantBaseUomRelationAsync(string variantKey, Guid baseUomRef, bool isCurrent, bool isActive, DateTime occurredOn)
    {
        return _graphProjectionService.UpsertRelationAsync(new GraphProjectionRelationRequest(
            CatalogGraphProjectionSchema.ProductVariantNode,
            variantKey,
            CatalogGraphProjectionSchema.UnitOfMeasureNode,
            CatalogGraphProjectionSchema.ToNodeKey(baseUomRef),
            CatalogGraphProjectionSchema.VariantBaseUomRelation,
            new Dictionary<string, object?>(StringComparer.Ordinal)
            {
                ["isCurrent"] = isCurrent,
                ["isActive"] = isActive,
                ["lastProjectedAt"] = occurredOn.ToString("O")
            }));
    }

    private Task UpsertVariantAttributeValueAsync(
        string variantKey,
        Guid attributeRef,
        string? value,
        Guid? optionRef,
        bool isActive,
        DateTime occurredOn)
    {
        return _graphProjectionService.UpsertRelationAsync(new GraphProjectionRelationRequest(
            CatalogGraphProjectionSchema.ProductVariantNode,
            variantKey,
            CatalogGraphProjectionSchema.AttributeDefinitionNode,
            CatalogGraphProjectionSchema.ToNodeKey(attributeRef),
            CatalogGraphProjectionSchema.VariantHasAttributeValueRelation,
            new Dictionary<string, object?>(StringComparer.Ordinal)
            {
                ["value"] = value,
                ["optionRef"] = optionRef.HasValue ? CatalogGraphProjectionSchema.ToNodeKey(optionRef.Value) : null,
                ["isActive"] = isActive,
                ["lastProjectedAt"] = occurredOn.ToString("O")
            }));
    }

    private async Task UpsertVariantUomConversionAsync(
        string variantKey,
        Guid fromUomRef,
        Guid toUomRef,
        bool isActive,
        DateTime occurredOn,
        decimal? factor,
        string? roundingMode,
        bool? isBasePath)
    {
        var conversionNodeKey = $"{variantKey}:{CatalogGraphProjectionSchema.ToNodeKey(fromUomRef)}:{CatalogGraphProjectionSchema.ToNodeKey(toUomRef)}";

        await _graphProjectionService.UpsertNodeAsync(new GraphProjectionNodeRequest(
            CatalogGraphProjectionSchema.VariantUomConversionNode,
            conversionNodeKey,
            new Dictionary<string, object?>(StringComparer.Ordinal)
            {
                ["variantRef"] = variantKey,
                ["fromUomRef"] = CatalogGraphProjectionSchema.ToNodeKey(fromUomRef),
                ["toUomRef"] = CatalogGraphProjectionSchema.ToNodeKey(toUomRef),
                ["factor"] = factor,
                ["roundingMode"] = roundingMode,
                ["isBasePath"] = isBasePath,
                ["isActive"] = isActive,
                ["lastProjectedAt"] = occurredOn.ToString("O")
            }));

        await _graphProjectionService.UpsertRelationAsync(new GraphProjectionRelationRequest(
            CatalogGraphProjectionSchema.ProductVariantNode,
            variantKey,
            CatalogGraphProjectionSchema.VariantUomConversionNode,
            conversionNodeKey,
            CatalogGraphProjectionSchema.VariantHasUomConversionRelation,
            new Dictionary<string, object?>(StringComparer.Ordinal)
            {
                ["isActive"] = isActive,
                ["lastProjectedAt"] = occurredOn.ToString("O")
            }));

        await _graphProjectionService.UpsertRelationAsync(new GraphProjectionRelationRequest(
            CatalogGraphProjectionSchema.VariantUomConversionNode,
            conversionNodeKey,
            CatalogGraphProjectionSchema.UnitOfMeasureNode,
            CatalogGraphProjectionSchema.ToNodeKey(fromUomRef),
            CatalogGraphProjectionSchema.ConversionFromUomRelation,
            new Dictionary<string, object?>(StringComparer.Ordinal)
            {
                ["isActive"] = isActive,
                ["lastProjectedAt"] = occurredOn.ToString("O")
            }));

        await _graphProjectionService.UpsertRelationAsync(new GraphProjectionRelationRequest(
            CatalogGraphProjectionSchema.VariantUomConversionNode,
            conversionNodeKey,
            CatalogGraphProjectionSchema.UnitOfMeasureNode,
            CatalogGraphProjectionSchema.ToNodeKey(toUomRef),
            CatalogGraphProjectionSchema.ConversionToUomRelation,
            new Dictionary<string, object?>(StringComparer.Ordinal)
            {
                ["isActive"] = isActive,
                ["lastProjectedAt"] = occurredOn.ToString("O")
            }));
    }
}
