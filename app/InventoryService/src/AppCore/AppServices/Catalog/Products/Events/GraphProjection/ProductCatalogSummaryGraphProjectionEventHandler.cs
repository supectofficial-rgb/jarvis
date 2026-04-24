namespace Insurance.InventoryService.AppCore.AppServices.Catalog.Products.Events.GraphProjection;

using Insurance.InventoryService.AppCore.AppServices.Catalog.Services;
using Insurance.InventoryService.AppCore.Domain.Catalog.Events;
using Insurance.InventoryService.AppCore.Shared.Catalog.Services;
using OysterFx.AppCore.Shared.Events;

public sealed class ProductCatalogSummaryGraphProjectionEventHandler :
    IDomainEventHandler<ProductCreatedEvent>,
    IDomainEventHandler<ProductUpdatedEvent>,
    IDomainEventHandler<ProductActivationChangedEvent>,
    IDomainEventHandler<ProductCategoryChangedEvent>,
    IDomainEventHandler<ProductAttributeValueUpsertedEvent>,
    IDomainEventHandler<ProductAttributeValueRemovedEvent>
{
    private readonly IGraphProjectionService _graphProjectionService;

    public ProductCatalogSummaryGraphProjectionEventHandler(IGraphProjectionService graphProjectionService)
    {
        _graphProjectionService = graphProjectionService;
    }

    public async Task Handle(ProductCreatedEvent @event)
    {
        var productKey = CatalogGraphProjectionSchema.ToNodeKey(@event.ProductBusinessKey);
        var summaryKey = CatalogGraphProjectionSchema.ToProductCatalogSummaryKey(@event.ProductBusinessKey);

        await UpsertSummaryNodeAsync(
            summaryKey,
            productKey,
            @event.CategoryRef,
            @event.CategorySchemaVersionRef,
            @event.BaseSku,
            @event.Name,
            @event.DefaultUomRef,
            @event.TaxCategoryRef,
            @event.IsActive,
            @event.AttributeValues.Count,
            "ProductCreatedEvent",
            @event.OccurredOn);

        await UpsertSummaryOfProductRelationAsync(summaryKey, productKey, @event.OccurredOn);
    }

    public async Task Handle(ProductUpdatedEvent @event)
    {
        var productKey = CatalogGraphProjectionSchema.ToNodeKey(@event.ProductBusinessKey);
        var summaryKey = CatalogGraphProjectionSchema.ToProductCatalogSummaryKey(@event.ProductBusinessKey);

        await UpsertSummaryNodeAsync(
            summaryKey,
            productKey,
            @event.CategoryRef,
            @event.CategorySchemaVersionRef,
            @event.BaseSku,
            @event.Name,
            @event.DefaultUomRef,
            @event.TaxCategoryRef,
            @event.IsActive,
            @event.AttributeValues.Count,
            "ProductUpdatedEvent",
            @event.OccurredOn);

        await UpsertSummaryOfProductRelationAsync(summaryKey, productKey, @event.OccurredOn);
    }

    public async Task Handle(ProductActivationChangedEvent @event)
    {
        var productKey = CatalogGraphProjectionSchema.ToNodeKey(@event.ProductBusinessKey);
        var summaryKey = CatalogGraphProjectionSchema.ToProductCatalogSummaryKey(@event.ProductBusinessKey);

        await _graphProjectionService.UpsertNodeAsync(new GraphProjectionNodeRequest(
            CatalogGraphProjectionSchema.ProductCatalogSummaryNode,
            summaryKey,
            new Dictionary<string, object?>(StringComparer.Ordinal)
            {
                ["productRef"] = productKey,
                ["isActive"] = @event.IsActive,
                ["lastEvent"] = "ProductActivationChangedEvent",
                ["lastProjectedAt"] = @event.OccurredOn.ToString("O")
            }));

        await UpsertSummaryOfProductRelationAsync(summaryKey, productKey, @event.OccurredOn);
    }

    public async Task Handle(ProductCategoryChangedEvent @event)
    {
        var productKey = CatalogGraphProjectionSchema.ToNodeKey(@event.ProductBusinessKey);
        var summaryKey = CatalogGraphProjectionSchema.ToProductCatalogSummaryKey(@event.ProductBusinessKey);

        await _graphProjectionService.UpsertNodeAsync(new GraphProjectionNodeRequest(
            CatalogGraphProjectionSchema.ProductCatalogSummaryNode,
            summaryKey,
            new Dictionary<string, object?>(StringComparer.Ordinal)
            {
                ["productRef"] = productKey,
                ["categoryRef"] = CatalogGraphProjectionSchema.ToNodeKey(@event.CategoryRef),
                ["categorySchemaVersionRef"] = CatalogGraphProjectionSchema.ToNodeKey(@event.CategorySchemaVersionRef),
                ["previousCategoryRef"] = CatalogGraphProjectionSchema.ToNodeKey(@event.PreviousCategoryRef),
                ["previousCategorySchemaVersionRef"] = CatalogGraphProjectionSchema.ToNodeKey(@event.PreviousCategorySchemaVersionRef),
                ["lastEvent"] = "ProductCategoryChangedEvent",
                ["lastProjectedAt"] = @event.OccurredOn.ToString("O")
            }));

        await UpsertSummaryOfProductRelationAsync(summaryKey, productKey, @event.OccurredOn);
    }

    public async Task Handle(ProductAttributeValueUpsertedEvent @event)
    {
        var productKey = CatalogGraphProjectionSchema.ToNodeKey(@event.ProductBusinessKey);
        var summaryKey = CatalogGraphProjectionSchema.ToProductCatalogSummaryKey(@event.ProductBusinessKey);

        await _graphProjectionService.UpsertNodeAsync(new GraphProjectionNodeRequest(
            CatalogGraphProjectionSchema.ProductCatalogSummaryNode,
            summaryKey,
            new Dictionary<string, object?>(StringComparer.Ordinal)
            {
                ["productRef"] = productKey,
                ["lastAttributeRef"] = CatalogGraphProjectionSchema.ToNodeKey(@event.AttributeRef),
                ["lastAttributeValue"] = @event.Value,
                ["lastAttributeOptionRef"] = @event.OptionRef.HasValue
                    ? CatalogGraphProjectionSchema.ToNodeKey(@event.OptionRef.Value)
                    : null,
                ["lastEvent"] = "ProductAttributeValueUpsertedEvent",
                ["lastProjectedAt"] = @event.OccurredOn.ToString("O")
            }));

        await UpsertSummaryOfProductRelationAsync(summaryKey, productKey, @event.OccurredOn);
    }

    public async Task Handle(ProductAttributeValueRemovedEvent @event)
    {
        var productKey = CatalogGraphProjectionSchema.ToNodeKey(@event.ProductBusinessKey);
        var summaryKey = CatalogGraphProjectionSchema.ToProductCatalogSummaryKey(@event.ProductBusinessKey);

        await _graphProjectionService.UpsertNodeAsync(new GraphProjectionNodeRequest(
            CatalogGraphProjectionSchema.ProductCatalogSummaryNode,
            summaryKey,
            new Dictionary<string, object?>(StringComparer.Ordinal)
            {
                ["productRef"] = productKey,
                ["lastAttributeRef"] = CatalogGraphProjectionSchema.ToNodeKey(@event.AttributeRef),
                ["lastAttributeRemoved"] = true,
                ["lastEvent"] = "ProductAttributeValueRemovedEvent",
                ["lastProjectedAt"] = @event.OccurredOn.ToString("O")
            }));

        await UpsertSummaryOfProductRelationAsync(summaryKey, productKey, @event.OccurredOn);
    }

    private Task UpsertSummaryNodeAsync(
        string summaryKey,
        string productKey,
        Guid categoryRef,
        Guid categorySchemaVersionRef,
        string baseSku,
        string name,
        Guid defaultUomRef,
        Guid? taxCategoryRef,
        bool isActive,
        int attributeValueCount,
        string lastEvent,
        DateTime occurredOn)
    {
        return _graphProjectionService.UpsertNodeAsync(new GraphProjectionNodeRequest(
            CatalogGraphProjectionSchema.ProductCatalogSummaryNode,
            summaryKey,
            new Dictionary<string, object?>(StringComparer.Ordinal)
            {
                ["productRef"] = productKey,
                ["categoryRef"] = CatalogGraphProjectionSchema.ToNodeKey(categoryRef),
                ["categorySchemaVersionRef"] = CatalogGraphProjectionSchema.ToNodeKey(categorySchemaVersionRef),
                ["baseSku"] = baseSku,
                ["name"] = name,
                ["defaultUomRef"] = CatalogGraphProjectionSchema.ToNodeKey(defaultUomRef),
                ["taxCategoryRef"] = taxCategoryRef.HasValue ? CatalogGraphProjectionSchema.ToNodeKey(taxCategoryRef.Value) : null,
                ["isActive"] = isActive,
                ["attributeValueCount"] = attributeValueCount,
                ["lastEvent"] = lastEvent,
                ["lastProjectedAt"] = occurredOn.ToString("O")
            }));
    }

    private Task UpsertSummaryOfProductRelationAsync(string summaryKey, string productKey, DateTime occurredOn)
    {
        return _graphProjectionService.UpsertRelationAsync(new GraphProjectionRelationRequest(
            CatalogGraphProjectionSchema.ProductCatalogSummaryNode,
            summaryKey,
            CatalogGraphProjectionSchema.ProductNode,
            productKey,
            CatalogGraphProjectionSchema.SummaryOfProductRelation,
            new Dictionary<string, object?>(StringComparer.Ordinal)
            {
                ["isActive"] = true,
                ["lastProjectedAt"] = occurredOn.ToString("O")
            }));
    }
}
