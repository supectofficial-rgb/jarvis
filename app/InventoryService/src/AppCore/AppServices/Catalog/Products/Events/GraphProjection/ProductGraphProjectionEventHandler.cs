namespace Insurance.InventoryService.AppCore.AppServices.Catalog.Products.Events.GraphProjection;

using Insurance.InventoryService.AppCore.AppServices.Catalog.Services;
using Insurance.InventoryService.AppCore.Domain.Catalog.Events;
using Insurance.InventoryService.AppCore.Shared.Catalog.Services;
using OysterFx.AppCore.Shared.Events;

public sealed class ProductGraphProjectionEventHandler :
    IDomainEventHandler<ProductCreatedEvent>,
    IDomainEventHandler<ProductUpdatedEvent>,
    IDomainEventHandler<ProductActivationChangedEvent>,
    IDomainEventHandler<ProductCategoryChangedEvent>,
    IDomainEventHandler<ProductAttributeValueUpsertedEvent>,
    IDomainEventHandler<ProductAttributeValueRemovedEvent>
{
    private readonly IGraphProjectionService _graphProjectionService;

    public ProductGraphProjectionEventHandler(IGraphProjectionService graphProjectionService)
    {
        _graphProjectionService = graphProjectionService;
    }

    public async Task Handle(ProductCreatedEvent @event)
    {
        var productKey = CatalogGraphProjectionSchema.ToNodeKey(@event.ProductBusinessKey);

        await UpsertProductNodeAsync(
            productKey,
            @event.CategoryRef,
            @event.CategorySchemaVersionRef,
            @event.BaseSku,
            @event.Name,
            @event.DefaultUomRef,
            @event.TaxCategoryRef,
            @event.IsActive,
            @event.AttributeValues.Count,
            @event.OccurredOn);

        await UpsertProductCategoryRelationAsync(productKey, @event.CategoryRef, @event.CategorySchemaVersionRef, isCurrent: true, isActive: true, @event.OccurredOn);
        await UpsertProductDefaultUomRelationAsync(productKey, @event.DefaultUomRef, isCurrent: true, isActive: true, @event.OccurredOn);

        foreach (var attributeValue in @event.AttributeValues)
        {
            await UpsertProductAttributeValueAsync(
                productKey,
                attributeValue.AttributeRef,
                attributeValue.Value,
                attributeValue.OptionRef,
                isActive: true,
                @event.OccurredOn);
        }
    }

    public async Task Handle(ProductUpdatedEvent @event)
    {
        var productKey = CatalogGraphProjectionSchema.ToNodeKey(@event.ProductBusinessKey);

        await UpsertProductNodeAsync(
            productKey,
            @event.CategoryRef,
            @event.CategorySchemaVersionRef,
            @event.BaseSku,
            @event.Name,
            @event.DefaultUomRef,
            @event.TaxCategoryRef,
            @event.IsActive,
            @event.AttributeValues.Count,
            @event.OccurredOn);

        await UpsertProductCategoryRelationAsync(productKey, @event.CategoryRef, @event.CategorySchemaVersionRef, isCurrent: true, isActive: true, @event.OccurredOn);
        await UpsertProductDefaultUomRelationAsync(productKey, @event.DefaultUomRef, isCurrent: true, isActive: true, @event.OccurredOn);

        foreach (var attributeValue in @event.AttributeValues)
        {
            await UpsertProductAttributeValueAsync(
                productKey,
                attributeValue.AttributeRef,
                attributeValue.Value,
                attributeValue.OptionRef,
                isActive: true,
                @event.OccurredOn);
        }
    }

    public Task Handle(ProductActivationChangedEvent @event)
    {
        return _graphProjectionService.UpsertNodeAsync(new GraphProjectionNodeRequest(
            CatalogGraphProjectionSchema.ProductNode,
            CatalogGraphProjectionSchema.ToNodeKey(@event.ProductBusinessKey),
            new Dictionary<string, object?>(StringComparer.Ordinal)
            {
                ["isActive"] = @event.IsActive,
                ["lastProjectedAt"] = @event.OccurredOn.ToString("O")
            }));
    }

    public async Task Handle(ProductCategoryChangedEvent @event)
    {
        var productKey = CatalogGraphProjectionSchema.ToNodeKey(@event.ProductBusinessKey);

        await _graphProjectionService.UpsertNodeAsync(new GraphProjectionNodeRequest(
            CatalogGraphProjectionSchema.ProductNode,
            productKey,
            new Dictionary<string, object?>(StringComparer.Ordinal)
            {
                ["categoryRef"] = CatalogGraphProjectionSchema.ToNodeKey(@event.CategoryRef),
                ["categorySchemaVersionRef"] = CatalogGraphProjectionSchema.ToNodeKey(@event.CategorySchemaVersionRef),
                ["lastProjectedAt"] = @event.OccurredOn.ToString("O")
            }));

        await UpsertProductCategoryRelationAsync(productKey, @event.PreviousCategoryRef, @event.PreviousCategorySchemaVersionRef, isCurrent: false, isActive: false, @event.OccurredOn);
        await UpsertProductCategoryRelationAsync(productKey, @event.CategoryRef, @event.CategorySchemaVersionRef, isCurrent: true, isActive: true, @event.OccurredOn);
    }

    public Task Handle(ProductAttributeValueUpsertedEvent @event)
    {
        return UpsertProductAttributeValueAsync(
            CatalogGraphProjectionSchema.ToNodeKey(@event.ProductBusinessKey),
            @event.AttributeRef,
            @event.Value,
            @event.OptionRef,
            isActive: true,
            @event.OccurredOn);
    }

    public Task Handle(ProductAttributeValueRemovedEvent @event)
    {
        return UpsertProductAttributeValueAsync(
            CatalogGraphProjectionSchema.ToNodeKey(@event.ProductBusinessKey),
            @event.AttributeRef,
            value: null,
            optionRef: null,
            isActive: false,
            @event.OccurredOn);
    }

    private Task UpsertProductNodeAsync(
        string productKey,
        Guid categoryRef,
        Guid categorySchemaVersionRef,
        string baseSku,
        string name,
        Guid defaultUomRef,
        Guid? taxCategoryRef,
        bool isActive,
        int attributeValueCount,
        DateTime occurredOn)
    {
        return _graphProjectionService.UpsertNodeAsync(new GraphProjectionNodeRequest(
            CatalogGraphProjectionSchema.ProductNode,
            productKey,
            new Dictionary<string, object?>(StringComparer.Ordinal)
            {
                ["categoryRef"] = CatalogGraphProjectionSchema.ToNodeKey(categoryRef),
                ["categorySchemaVersionRef"] = CatalogGraphProjectionSchema.ToNodeKey(categorySchemaVersionRef),
                ["baseSku"] = baseSku,
                ["name"] = name,
                ["defaultUomRef"] = CatalogGraphProjectionSchema.ToNodeKey(defaultUomRef),
                ["taxCategoryRef"] = taxCategoryRef.HasValue ? CatalogGraphProjectionSchema.ToNodeKey(taxCategoryRef.Value) : null,
                ["isActive"] = isActive,
                ["attributeValueCount"] = attributeValueCount,
                ["lastProjectedAt"] = occurredOn.ToString("O")
            }));
    }

    private Task UpsertProductCategoryRelationAsync(
        string productKey,
        Guid categoryRef,
        Guid categorySchemaVersionRef,
        bool isCurrent,
        bool isActive,
        DateTime occurredOn)
    {
        return _graphProjectionService.UpsertRelationAsync(new GraphProjectionRelationRequest(
            CatalogGraphProjectionSchema.ProductNode,
            productKey,
            CatalogGraphProjectionSchema.CategoryNode,
            CatalogGraphProjectionSchema.ToNodeKey(categoryRef),
            CatalogGraphProjectionSchema.ProductInCategoryRelation,
            new Dictionary<string, object?>(StringComparer.Ordinal)
            {
                ["categorySchemaVersionRef"] = CatalogGraphProjectionSchema.ToNodeKey(categorySchemaVersionRef),
                ["isCurrent"] = isCurrent,
                ["isActive"] = isActive,
                ["lastProjectedAt"] = occurredOn.ToString("O")
            }));
    }

    private Task UpsertProductAttributeValueAsync(
        string productKey,
        Guid attributeRef,
        string? value,
        Guid? optionRef,
        bool isActive,
        DateTime occurredOn)
    {
        return _graphProjectionService.UpsertRelationAsync(new GraphProjectionRelationRequest(
            CatalogGraphProjectionSchema.ProductNode,
            productKey,
            CatalogGraphProjectionSchema.AttributeDefinitionNode,
            CatalogGraphProjectionSchema.ToNodeKey(attributeRef),
            CatalogGraphProjectionSchema.ProductHasAttributeValueRelation,
            new Dictionary<string, object?>(StringComparer.Ordinal)
            {
                ["value"] = value,
                ["optionRef"] = optionRef.HasValue ? CatalogGraphProjectionSchema.ToNodeKey(optionRef.Value) : null,
                ["isActive"] = isActive,
                ["lastProjectedAt"] = occurredOn.ToString("O")
            }));
    }

    private Task UpsertProductDefaultUomRelationAsync(string productKey, Guid defaultUomRef, bool isCurrent, bool isActive, DateTime occurredOn)
    {
        return _graphProjectionService.UpsertRelationAsync(new GraphProjectionRelationRequest(
            CatalogGraphProjectionSchema.ProductNode,
            productKey,
            CatalogGraphProjectionSchema.UnitOfMeasureNode,
            CatalogGraphProjectionSchema.ToNodeKey(defaultUomRef),
            CatalogGraphProjectionSchema.ProductDefaultUomRelation,
            new Dictionary<string, object?>(StringComparer.Ordinal)
            {
                ["isCurrent"] = isCurrent,
                ["isActive"] = isActive,
                ["lastProjectedAt"] = occurredOn.ToString("O")
            }));
    }
}
