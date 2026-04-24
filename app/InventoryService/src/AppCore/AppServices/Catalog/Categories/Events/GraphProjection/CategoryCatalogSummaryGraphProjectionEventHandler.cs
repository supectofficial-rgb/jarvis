namespace Insurance.InventoryService.AppCore.AppServices.Catalog.Categories.Events.GraphProjection;

using Insurance.InventoryService.AppCore.AppServices.Catalog.Services;
using Insurance.InventoryService.AppCore.Domain.Catalog.Events;
using Insurance.InventoryService.AppCore.Shared.Catalog.Services;
using OysterFx.AppCore.Shared.Events;

public sealed class CategoryCatalogSummaryGraphProjectionEventHandler :
    IDomainEventHandler<CategoryCreatedEvent>,
    IDomainEventHandler<CategoryUpdatedEvent>,
    IDomainEventHandler<CategoryMovedEvent>,
    IDomainEventHandler<CategoryActivationChangedEvent>,
    IDomainEventHandler<CategoryAttributeRuleUpsertedEvent>,
    IDomainEventHandler<CategoryAttributeRuleRemovedEvent>
{
    private readonly IGraphProjectionService _graphProjectionService;

    public CategoryCatalogSummaryGraphProjectionEventHandler(IGraphProjectionService graphProjectionService)
    {
        _graphProjectionService = graphProjectionService;
    }

    public async Task Handle(CategoryCreatedEvent @event)
    {
        var categoryKey = CatalogGraphProjectionSchema.ToNodeKey(@event.CategoryBusinessKey);
        var summaryKey = CatalogGraphProjectionSchema.ToCategoryCatalogSummaryKey(@event.CategoryBusinessKey);

        await UpsertSummaryNodeAsync(
            summaryKey,
            categoryKey,
            @event.Code,
            @event.Name,
            @event.DisplayOrder,
            @event.ParentCategoryRef,
            @event.IsActive,
            @event.AttributeRules.Count,
            "CategoryCreatedEvent",
            @event.OccurredOn);

        await UpsertSummaryOfCategoryRelationAsync(summaryKey, categoryKey, @event.OccurredOn);
    }

    public async Task Handle(CategoryUpdatedEvent @event)
    {
        var categoryKey = CatalogGraphProjectionSchema.ToNodeKey(@event.CategoryBusinessKey);
        var summaryKey = CatalogGraphProjectionSchema.ToCategoryCatalogSummaryKey(@event.CategoryBusinessKey);

        await UpsertSummaryNodeAsync(
            summaryKey,
            categoryKey,
            @event.Code,
            @event.Name,
            @event.DisplayOrder,
            @event.ParentCategoryRef,
            @event.IsActive,
            @event.AttributeRules.Count,
            "CategoryUpdatedEvent",
            @event.OccurredOn);

        await UpsertSummaryOfCategoryRelationAsync(summaryKey, categoryKey, @event.OccurredOn);
    }

    public async Task Handle(CategoryMovedEvent @event)
    {
        var categoryKey = CatalogGraphProjectionSchema.ToNodeKey(@event.CategoryBusinessKey);
        var summaryKey = CatalogGraphProjectionSchema.ToCategoryCatalogSummaryKey(@event.CategoryBusinessKey);

        await _graphProjectionService.UpsertNodeAsync(new GraphProjectionNodeRequest(
            CatalogGraphProjectionSchema.CategoryCatalogSummaryNode,
            summaryKey,
            new Dictionary<string, object?>(StringComparer.Ordinal)
            {
                ["categoryRef"] = categoryKey,
                ["parentCategoryRef"] = @event.ParentCategoryRef.HasValue
                    ? CatalogGraphProjectionSchema.ToNodeKey(@event.ParentCategoryRef.Value)
                    : null,
                ["previousParentCategoryRef"] = @event.PreviousParentCategoryRef.HasValue
                    ? CatalogGraphProjectionSchema.ToNodeKey(@event.PreviousParentCategoryRef.Value)
                    : null,
                ["lastEvent"] = "CategoryMovedEvent",
                ["lastProjectedAt"] = @event.OccurredOn.ToString("O")
            }));

        await UpsertSummaryOfCategoryRelationAsync(summaryKey, categoryKey, @event.OccurredOn);
    }

    public async Task Handle(CategoryActivationChangedEvent @event)
    {
        var categoryKey = CatalogGraphProjectionSchema.ToNodeKey(@event.CategoryBusinessKey);
        var summaryKey = CatalogGraphProjectionSchema.ToCategoryCatalogSummaryKey(@event.CategoryBusinessKey);

        await _graphProjectionService.UpsertNodeAsync(new GraphProjectionNodeRequest(
            CatalogGraphProjectionSchema.CategoryCatalogSummaryNode,
            summaryKey,
            new Dictionary<string, object?>(StringComparer.Ordinal)
            {
                ["categoryRef"] = categoryKey,
                ["isActive"] = @event.IsActive,
                ["lastEvent"] = "CategoryActivationChangedEvent",
                ["lastProjectedAt"] = @event.OccurredOn.ToString("O")
            }));

        await UpsertSummaryOfCategoryRelationAsync(summaryKey, categoryKey, @event.OccurredOn);
    }

    public async Task Handle(CategoryAttributeRuleUpsertedEvent @event)
    {
        var categoryKey = CatalogGraphProjectionSchema.ToNodeKey(@event.CategoryBusinessKey);
        var summaryKey = CatalogGraphProjectionSchema.ToCategoryCatalogSummaryKey(@event.CategoryBusinessKey);

        await _graphProjectionService.UpsertNodeAsync(new GraphProjectionNodeRequest(
            CatalogGraphProjectionSchema.CategoryCatalogSummaryNode,
            summaryKey,
            new Dictionary<string, object?>(StringComparer.Ordinal)
            {
                ["categoryRef"] = categoryKey,
                ["lastRuleAttributeRef"] = CatalogGraphProjectionSchema.ToNodeKey(@event.AttributeRef),
                ["lastRuleCategorySchemaVersionRef"] = CatalogGraphProjectionSchema.ToNodeKey(@event.CategorySchemaVersionRef),
                ["lastRuleIsRequired"] = @event.IsRequired,
                ["lastRuleIsVariant"] = @event.IsVariant,
                ["lastRuleDisplayOrder"] = @event.DisplayOrder,
                ["lastRuleIsOverridden"] = @event.IsOverridden,
                ["lastRuleIsActive"] = @event.IsActive,
                ["lastEvent"] = "CategoryAttributeRuleUpsertedEvent",
                ["lastProjectedAt"] = @event.OccurredOn.ToString("O")
            }));

        await UpsertSummaryOfCategoryRelationAsync(summaryKey, categoryKey, @event.OccurredOn);
    }

    public async Task Handle(CategoryAttributeRuleRemovedEvent @event)
    {
        var categoryKey = CatalogGraphProjectionSchema.ToNodeKey(@event.CategoryBusinessKey);
        var summaryKey = CatalogGraphProjectionSchema.ToCategoryCatalogSummaryKey(@event.CategoryBusinessKey);

        await _graphProjectionService.UpsertNodeAsync(new GraphProjectionNodeRequest(
            CatalogGraphProjectionSchema.CategoryCatalogSummaryNode,
            summaryKey,
            new Dictionary<string, object?>(StringComparer.Ordinal)
            {
                ["categoryRef"] = categoryKey,
                ["lastRuleAttributeRef"] = CatalogGraphProjectionSchema.ToNodeKey(@event.AttributeRef),
                ["lastRuleCategorySchemaVersionRef"] = CatalogGraphProjectionSchema.ToNodeKey(@event.CategorySchemaVersionRef),
                ["lastRuleIsActive"] = false,
                ["lastEvent"] = "CategoryAttributeRuleRemovedEvent",
                ["lastProjectedAt"] = @event.OccurredOn.ToString("O")
            }));

        await UpsertSummaryOfCategoryRelationAsync(summaryKey, categoryKey, @event.OccurredOn);
    }

    private Task UpsertSummaryNodeAsync(
        string summaryKey,
        string categoryKey,
        string code,
        string name,
        int displayOrder,
        Guid? parentCategoryRef,
        bool isActive,
        int attributeRuleCount,
        string lastEvent,
        DateTime occurredOn)
    {
        return _graphProjectionService.UpsertNodeAsync(new GraphProjectionNodeRequest(
            CatalogGraphProjectionSchema.CategoryCatalogSummaryNode,
            summaryKey,
            new Dictionary<string, object?>(StringComparer.Ordinal)
            {
                ["categoryRef"] = categoryKey,
                ["code"] = code,
                ["name"] = name,
                ["displayOrder"] = displayOrder,
                ["parentCategoryRef"] = parentCategoryRef.HasValue
                    ? CatalogGraphProjectionSchema.ToNodeKey(parentCategoryRef.Value)
                    : null,
                ["isActive"] = isActive,
                ["attributeRuleCount"] = attributeRuleCount,
                ["lastEvent"] = lastEvent,
                ["lastProjectedAt"] = occurredOn.ToString("O")
            }));
    }

    private Task UpsertSummaryOfCategoryRelationAsync(string summaryKey, string categoryKey, DateTime occurredOn)
    {
        return _graphProjectionService.UpsertRelationAsync(new GraphProjectionRelationRequest(
            CatalogGraphProjectionSchema.CategoryCatalogSummaryNode,
            summaryKey,
            CatalogGraphProjectionSchema.CategoryNode,
            categoryKey,
            CatalogGraphProjectionSchema.SummaryOfCategoryRelation,
            new Dictionary<string, object?>(StringComparer.Ordinal)
            {
                ["isActive"] = true,
                ["lastProjectedAt"] = occurredOn.ToString("O")
            }));
    }
}
